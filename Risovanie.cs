using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using static Kisti;

// Рисование улицы: небо, погода, дорога, машины, прохожие и люди
static class Risovanie
{
    public const int Shirina = 1000, Vysota = 560;
    public const double Masshtab = 1.28;
    public const int Stena = 370;

    public static Color yantar = Color.FromArgb(240, 190, 90);
    static Color svetFonarya = Color.FromArgb(255, 216, 140);
    public static Vneshnost viktor = Vneshnost.Muzhchina(Color.FromArgb(112, 92, 72), Color.FromArgb(55, 50, 50), Color.FromArgb(82, 72, 66));

    public static void Vse(Graphics g, Gorod gorod, Mesto otkrytoe, double shag, bool pauza)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAlias;
        g.ScaleTransform((float)Masshtab, (float)Masshtab);

        if (gorod == null)
        {
            Ekrany.Risovat(g);
            return;
        }

        Replika replika = gorod.ochered.Count > 0 ? gorod.ochered[0] : null;

        if (replika != null && replika.kartinka != "")
        {
            Kartinki.Risovat(g, replika, gorod);
            return;
        }

        if (gorod.vnutri != null) Interery.Risovat(g, gorod, shag);
        else Ulica(g, gorod, shag);

        Interfeys.Risovat(g, gorod, otkrytoe);
        if (pauza) Interfeys.Pauza(g);
    }

    public static double Svet(double chas)
    {
        return Math.Clamp(1 - Math.Abs(chas - 13) / 9.5, 0.04, 1);
    }

    static void Ulica(Graphics g, Gorod gorod, double shag)
    {
        Igrok ig = gorod.ig;
        double svet = Svet(ig.chas);
        double zima = gorod.Zima();
        double kamera = Math.Clamp(ig.x - Shirina / 2, 0, Gorod.Dlina - Shirina);
        double vremya = gorod.vremya;
        bool dozhd = gorod.Dozhd();

        Nebo(g, ig.chas, svet, dozhd, vremya);
        DalniyGorod(g, svet, kamera);
        Doroga(g, svet, zima, dozhd, kamera);

        for (double x = 40; x < Gorod.Dlina; x += 400)
        {
            double naEkrane = x - kamera;
            if (naEkrane > -160 && naEkrane < Shirina + 160 && MestoSvobodno(gorod, x)) Derevo(g, svet, zima, naEkrane);
        }

        foreach (Mesto mesto in gorod.mesta)
        {
            double x = mesto.x - kamera;
            if (mesto.vid == "zdanie" && x > -340 && x < Shirina + 340) Zdaniya.Risovat(g, gorod, mesto, x, svet, zima);
        }

        Fonari(g, svet, kamera, dozhd);
        Prohozhie(g, gorod, kamera);

        Sloy(g, gorod, svet, kamera, false);
        Geroy(g, ig, ig.x - kamera, ig.y, Masshtabglubiny(ig.y), shag);
        Sloy(g, gorod, svet, kamera, true);

        if (dozhd) Dozhd(g, vremya);
        if (zima > 0) Sneg(g, vremya, 110);

        // Края экрана немного темнее, взгляд идёт в центр
        Color krai = Color.FromArgb(0, 0, 0, 0);
        Gradient(g, Color.FromArgb((int)(120 - svet * 80), 0, 0, 12), krai, 0, 0, Shirina, 150);
        Gradient(g, krai, Color.FromArgb((int)(130 - svet * 100), 0, 0, 12), 0, 450, Shirina, 110);
    }

    public static double Masshtabglubiny(double y)
    {
        return 0.82 + (y - Gorod.Verh) / (Gorod.Niz - Gorod.Verh) * 0.36;
    }

    static bool MestoSvobodno(Gorod gorod, double x)
    {
        foreach (Mesto mesto in gorod.mesta)
        {
            if (mesto.vid == "zdanie" && Math.Abs(mesto.x - x) < 215) return false;
        }
        return true;
    }

    // Рисует всё, что стоит дальше героя или ближе к нам
    static void Sloy(Graphics g, Gorod gorod, double svet, double kamera, bool blizhe)
    {
        double yGeroya = gorod.ig.y;

        foreach (Nahodka nahodka in gorod.nahodki)
        {
            double x = nahodka.x - kamera;
            if (x > -30 && x < Shirina + 30 && (nahodka.y > yGeroya) == blizhe) NarisovatNahodku(g, nahodka, x, gorod.vremya);
        }

        for (int i = 0; i < Gorod.KolichestvoMashin; i++)
        {
            double x = gorod.MashinaX(i) - kamera;
            double y = gorod.MashinaY(i);
            if (x > -240 && x < Shirina + 240 && (y > yGeroya) == blizhe) Mashina(g, x, y, i, svet, i % 2 == 0);
        }

        foreach (Mesto mesto in gorod.mesta)
        {
            double x = mesto.x - kamera;
            if (mesto.vid == "zdanie" || x < -200 || x > Shirina + 200 || (mesto.y > yGeroya) != blizhe) continue;

            if (mesto.vid == "musorka") Musorka(g, mesto, svet, x);
            else if (mesto.vid == "ostanovka") Ostanovka(g, mesto, svet, x);
            else Skameyka(g, gorod.ig, mesto, svet, x);
        }
    }

    static void Nebo(Graphics g, double chas, double svet, bool dozhd, double vremya)
    {
        Color verh = Smes(Color.FromArgb(7, 10, 28), Color.FromArgb(70, 125, 190), svet);
        Color niz = Smes(Color.FromArgb(34, 34, 70), Color.FromArgb(214, 228, 232), svet);

        // Закат и рассвет красят низ неба
        double zakat = Math.Max(0, 1 - Math.Abs(chas - 18) / 2.8) * 0.85;
        double rassvet = Math.Max(0, 1 - Math.Abs(chas - 7.5) / 1.8) * 0.55;
        niz = Smes(niz, Color.FromArgb(246, 132, 84), Math.Max(zakat, rassvet));
        verh = Smes(verh, Color.FromArgb(92, 62, 118), zakat * 0.6);

        if (dozhd)
        {
            verh = Smes(verh, Color.FromArgb(70, 76, 90), 0.7);
            niz = Smes(niz, Color.FromArgb(120, 128, 138), 0.7);
        }

        Gradient(g, verh, niz, 0, 0, Shirina, Stena);

        if (svet < 0.32)
        {
            for (int i = 0; i < 70; i++)
            {
                double migaet = 120 + Math.Sin(vremya * 2 + i) * 70;
                Krug(g, Alfa(Color.AliceBlue, migaet), i * 211 % Shirina, 70 + i * 97 % 220, 0.8 + i % 3 * 0.4);
            }

            Svechenie(g, Color.FromArgb(70, 215, 225, 255), 830, 150, 110);
            Krug(g, Color.FromArgb(240, 240, 226), 830, 150, 28);
            Krug(g, Color.FromArgb(216, 216, 204), 820, 142, 6);
            Krug(g, Color.FromArgb(218, 218, 206), 840, 160, 8);
            Krug(g, Color.FromArgb(220, 220, 208), 838, 136, 4);
        }
        else if (!dozhd)
        {
            double doliaDnya = (chas - 6.5) / 14;
            double x = doliaDnya * Shirina;
            double y = 330 - Math.Sin(doliaDnya * Math.PI) * 235;
            Svechenie(g, Color.FromArgb(190, 255, 236, 180), x, y, 150);
            Krug(g, Color.FromArgb(255, 247, 214), x, y, 26);
        }

        // Облака медленно плывут по небу
        Color oblako = Smes(Color.FromArgb(32, 34, 62), Color.WhiteSmoke, svet);
        oblako = Smes(oblako, Color.FromArgb(250, 170, 130), zakat * 0.7);
        int kolichestvoOblakov = dozhd ? 10 : 6;
        for (int i = 0; i < kolichestvoOblakov; i++)
        {
            double x = (i * 277 + vremya * (5 + i)) % (Shirina + 300) - 150;
            double y = 100 + i * 43 % 120;
            Oval(g, Alfa(oblako, 150), x, y, 170, 34);
            Oval(g, Alfa(oblako, 170), x + 36, y - 16, 96, 42);
            Oval(g, Alfa(oblako, 130), x + 90, y - 6, 76, 32);
        }
    }

    static void DalniyGorod(Graphics g, double svet, double kamera)
    {
        Color dalniy = Smes(Color.FromArgb(22, 25, 50), Color.FromArgb(166, 182, 198), svet);
        Color sredniy = Smes(Color.FromArgb(18, 20, 40), Color.FromArgb(134, 152, 170), svet);
        Color blizhniy = Smes(Color.FromArgb(14, 16, 32), Color.FromArgb(104, 120, 138), svet);

        for (int i = 0; i < 70; i++)
        {
            double x = i * 95 - kamera / 7;
            int vysota = 170 + i * 53 % 120;
            if (x > -100 && x < Shirina) Pryamougolnik(g, dalniy, x, Stena - vysota, 88, vysota);
        }

        for (int i = 0; i < 80; i++)
        {
            double x = i * 120 - kamera / 4;
            int vysota = 120 + i * 37 % 120;
            if (x < -120 || x > Shirina) continue;
            Pryamougolnik(g, sredniy, x, Stena - vysota, 104, vysota);
            if (i % 5 == 0) Pryamougolnik(g, sredniy, x + 46, Stena - vysota - 26, 3, 26);
        }

        for (int i = 0; i < 90; i++)
        {
            double x = i * 145 - kamera / 2.5;
            int vysota = 70 + i * 73 % 110;
            if (x < -130 || x > Shirina) continue;

            Pryamougolnik(g, blizhniy, x, Stena - vysota, 128, vysota);

            // Окна далёких домов светятся вечером
            if (svet > 0.5) continue;
            for (int okno = 0; okno < 16; okno++)
            {
                int ryad = (int)(okno / 4);
                int stolbec = okno % 4;
                if ((i * 3 + okno * 5) % 7 < 2 && ryad * 22 + 26 < vysota)
                {
                    Pryamougolnik(g, Color.FromArgb(200, 246, 204, 120), x + 12 + stolbec * 28, Stena - vysota + 12 + ryad * 22, 12, 13);
                }
            }
        }
    }

    static void Doroga(Graphics g, double svet, double zima, bool dozhd, double kamera)
    {
        Color sneg = Color.FromArgb(226, 234, 244);
        Color trotuar = Smes(Temno(Color.FromArgb(158, 156, 150), svet), Temno(sneg, svet + 0.2), zima * 0.8);
        Color asfalt = Smes(Temno(Color.FromArgb(70, 72, 80), svet), Temno(Color.FromArgb(150, 156, 168), svet), zima * 0.3);
        Color bordyur = Smes(trotuar, Color.Black, 0.35);
        Color razmetka = Smes(asfalt, Color.White, 0.72);

        Gradient(g, Smes(trotuar, Color.Black, 0.18), trotuar, 0, Stena, Shirina, 55);
        Pryamougolnik(g, bordyur, 0, 425, Shirina, 7);
        Gradient(g, Smes(asfalt, Color.Black, 0.2), asfalt, 0, 432, Shirina, 64);
        Pryamougolnik(g, bordyur, 0, 496, Shirina, 7);
        Gradient(g, trotuar, Smes(trotuar, Color.White, 0.08), 0, 503, Shirina, 57);

        // Плитка на тротуарах
        Color styk = Smes(trotuar, Color.Black, 0.14);
        double sdvig = -(kamera % 70);
        for (double x = sdvig; x < Shirina; x += 70)
        {
            Pryamougolnik(g, styk, x, Stena, 2, 55);
            Mnogougolnik(g, styk, x + 34, 503, x + 36, 503, x + 22, 560, x + 18, 560);
        }
        Pryamougolnik(g, styk, 0, 396, Shirina, 2);
        Pryamougolnik(g, styk, 0, 530, Shirina, 2);

        for (double x = -(kamera % 130); x < Shirina; x += 130) Pryamougolnik(g, razmetka, x, 463, 66, 4);

        // Пешеходные переходы
        for (int perehod = 0; perehod < 4; perehod++)
        {
            double x = 1660 + perehod * 1800 - kamera;
            if (x < -100 || x > Shirina + 100) continue;
            for (int i = 0; i < 7; i++) Pryamougolnik(g, razmetka, x, 434 + i * 9, 86, 5);
        }

        // Лужи отражают небо, зимой их нет
        if (zima > 0) return;
        Color luzha = Smes(asfalt, Color.FromArgb(150, 180, 210), 0.45 + svet * 0.2);
        for (int i = 0; i < 26; i++)
        {
            double x = i * 317 + 90 - kamera;
            if (x < -120 || x > Shirina + 120) continue;
            double y = i % 2 == 0 ? 448 : 482;
            Oval(g, Alfa(luzha, dozhd ? 190 : 110), x, y, 92 + i % 3 * 24, 9);
        }
    }

    static void Derevo(Graphics g, double svet, double zima, double x)
    {
        Color cvet = Temno(Color.FromArgb(84, 62, 48), svet);
        Oval(g, Color.FromArgb(70, 0, 0, 0), x - 46, Stena + 12, 92, 12);
        Liniya(g, cvet, 11, x, Stena + 18, x, 235);

        for (int i = 0; i < 8; i++)
        {
            int storona = i % 2 == 0 ? 1 : -1;
            double y = 322 - i * 14;
            double konecX = x + storona * (86 - i * 8);
            Liniya(g, cvet, 5 - i * 0.4, x, y, konecX, y - 44);
            Liniya(g, cvet, 2.2, x + storona * (52 - i * 5), y - 27, x + storona * (84 - i * 8), y - 70);
            Liniya(g, cvet, 1.6, konecX, y - 44, konecX + storona * 22, y - 54);
            if (zima > 0.2) Oval(g, Temno(Color.White, svet + 0.2), konecX - 14, y - 50, 26, 8);
        }

        if (zima > 0.2) Oval(g, Temno(Color.White, svet + 0.2), x - 26, Stena + 8, 52, 12);
    }

    static void Fonari(Graphics g, double svet, double kamera, bool dozhd)
    {
        for (int i = 0; i < 24; i++)
        {
            double x = 20 + i * 340 - kamera;
            if (x < -120 || x > Shirina + 120) continue;

            bool gorit = svet < 0.55;
            Color metall = Temno(Color.FromArgb(72, 78, 90), svet);

            Oval(g, Color.FromArgb(70, 0, 0, 0), x - 14, 414, 28, 7);
            Pryamougolnik(g, metall, x - 2.5, 248, 5, 170);
            Pryamougolnik(g, metall, x - 5, 402, 10, 16);
            Liniya(g, metall, 4, x, 250, x + 28, 242);
            Skruglennyy(g, metall, x + 12, 238, 38, 9, 4);

            if (!gorit) continue;
            Oval(g, svetFonarya, x + 16, 245, 30, 7);
            Svechenie(g, Alfa(svetFonarya, 160), x + 30, 252, 95);
            Mnogougolnik(g, Alfa(svetFonarya, 26), x + 16, 250, x + 44, 250, x + 105, 420, x - 45, 420);
            Oval(g, Alfa(svetFonarya, 54), x - 66, 396, 192, 38);

            // Отражение фонаря на дороге
            Gradient(g, Alfa(svetFonarya, 58), Alfa(svetFonarya, 0), x + 8, 434, 44, dozhd ? 62 : 40);
        }
    }

    static void Prohozhie(Graphics g, Gorod gorod, double kamera)
    {
        if (gorod.ig.chas > 21 || gorod.ig.chas < 7.5) return;

        Color[] pal = { Color.FromArgb(120, 60, 70), Color.FromArgb(50, 70, 110), Color.FromArgb(70, 80, 60), Color.FromArgb(90, 80, 100) };

        for (int i = 0; i < 9; i++)
        {
            bool vpravo = i % 2 == 0;
            double put = (gorod.vremya * (34 + i * 7) + i * 977) % Gorod.Dlina;
            double x = (vpravo ? put : Gorod.Dlina - put) - kamera;
            if (x < -60 || x > Shirina + 60) continue;

            Vneshnost vid = i % 3 == 0
                ? Vneshnost.Zhenshina(pal[i % 4], Color.FromArgb(60, 40, 30))
                : Vneshnost.Muzhchina(pal[i % 4], Color.FromArgb(40, 34, 30), Color.FromArgb(226, 190, 160));
            Chelovek(g, x, 396 + i % 3 * 7, 0.86, vpravo, gorod.vremya * 7 + i, false, vid);
        }
    }

    static void Mashina(Graphics g, double x, double y, int nomer, double svet, bool vpravo)
    {
        Color[] cveta = { Color.FromArgb(170, 40, 40), Color.FromArgb(40, 90, 150), Color.FromArgb(205, 205, 212), Color.FromArgb(40, 42, 50), Color.FromArgb(230, 170, 30) };
        Color kuzov = Temno(cveta[nomer], svet + 0.08);
        double napr = vpravo ? 1 : -1;
        Color steklo = svet < 0.45 ? Color.FromArgb(70, 80, 95) : Color.FromArgb(120, 165, 195);

        Oval(g, Color.FromArgb(95, 0, 0, 0), x - 100, y - 7, 200, 15);

        if (nomer == 4)
        {
            // Автобус
            Skruglennyy(g, kuzov, x - 128, y - 86, 256, 80, 10);
            for (int i = 0; i < 6; i++) Skruglennyy(g, steklo, x - 118 + i * 40, y - 76, 34, 30, 3);
            Pryamougolnik(g, Smes(kuzov, Color.Black, 0.3), x - 128, y - 30, 256, 5);
            Krug(g, Color.FromArgb(22, 22, 26), x - 74, y - 6, 13);
            Krug(g, Color.FromArgb(22, 22, 26), x + 74, y - 6, 13);
            Krug(g, Color.Gray, x - 74, y - 6, 5);
            Krug(g, Color.Gray, x + 74, y - 6, 5);
            Fary(g, x + napr * 124, y - 22, napr, svet);
            return;
        }

        Mnogougolnik(g, kuzov, x - 44 * napr, y - 34, x - 20 * napr, y - 56, x + 28 * napr, y - 56, x + 54 * napr, y - 34);
        Mnogougolnik(g, steklo, x - 34 * napr, y - 35, x - 16 * napr, y - 52, x + 2 * napr, y - 52, x + 2 * napr, y - 35);
        Mnogougolnik(g, steklo, x + 8 * napr, y - 35, x + 8 * napr, y - 52, x + 25 * napr, y - 52, x + 42 * napr, y - 35);
        Skruglennyy(g, kuzov, x - 96, y - 37, 192, 28, 10);
        Pryamougolnik(g, Smes(kuzov, Color.White, 0.25), x - 86, y - 29, 172, 2);
        Krug(g, Color.FromArgb(22, 22, 26), x - 56, y - 8, 12);
        Krug(g, Color.FromArgb(22, 22, 26), x + 56, y - 8, 12);
        Krug(g, Color.Silver, x - 56, y - 8, 5);
        Krug(g, Color.Silver, x + 56, y - 8, 5);
        Fary(g, x + napr * 92, y - 24, napr, svet);
    }

    static void Fary(Graphics g, double x, double y, double napr, double svet)
    {
        Krug(g, Color.FromArgb(255, 245, 200), x, y, 4);
        if (svet < 0.6)
        {
            Svechenie(g, Color.FromArgb(120, 255, 240, 190), x, y, 42);
            Mnogougolnik(g, Color.FromArgb(46, 255, 240, 190), x, y - 3, x + napr * 200, y - 16, x + napr * 200, y + 26, x, y + 4);
        }
    }

    static void Musorka(Graphics g, Mesto mesto, double svet, double x)
    {
        double niz = mesto.y;
        Color cvet = Temno(mesto.cvet, svet);
        Oval(g, Color.FromArgb(85, 0, 0, 0), x - 72, niz - 8, 144, 18);

        Pryamougolnik(g, Temno(Color.FromArgb(90, 150, 110), svet), x - 28, niz - 100, 9, 26);
        Pryamougolnik(g, Temno(Color.FromArgb(150, 110, 70), svet), x + 14, niz - 95, 9, 22);
        Pryamougolnik(g, Temno(Color.FromArgb(190, 180, 150), svet), x - 6, niz - 92, 24, 16);

        Skruglennyy(g, cvet, x - 58, niz - 82, 116, 78, 6);
        for (int i = 1; i < 6; i++) Pryamougolnik(g, Smes(cvet, Color.Black, 0.26), x - 58 + i * 19.3, niz - 77, 3, 68);
        Skruglennyy(g, Smes(cvet, Color.Black, 0.38), x - 65, niz - 90, 130, 14, 4);
        Krug(g, Color.FromArgb(22, 22, 26), x - 40, niz - 3, 7);
        Krug(g, Color.FromArgb(22, 22, 26), x + 40, niz - 3, 7);
    }

    static void Skameyka(Graphics g, Igrok ig, Mesto mesto, double svet, double x)
    {
        double niz = mesto.y;
        Color cvet = Temno(mesto.cvet, svet);
        Oval(g, Color.FromArgb(85, 0, 0, 0), x - 86, niz - 8, 172, 16);

        for (int i = 0; i < 3; i++) Skruglennyy(g, cvet, x - 78, niz - 84 + i * 12, 156, 8, 2);
        Skruglennyy(g, Smes(cvet, Color.White, 0.1), x - 80, niz - 42, 160, 10, 3);
        Pryamougolnik(g, Color.FromArgb(36, 36, 42), x - 70, niz - 36, 9, 36);
        Pryamougolnik(g, Color.FromArgb(36, 36, 42), x + 61, niz - 36, 9, 36);
        if (!ig.mihalychSpasen) Chelovek(g, x - 12, niz - 5, 1.1, true, 0, true, mesto.vneshnost);
    }

    static void Ostanovka(Graphics g, Mesto mesto, double svet, double x)
    {
        double niz = mesto.y;
        Color metall = Temno(Color.FromArgb(70, 80, 95), svet);
        Oval(g, Color.FromArgb(85, 0, 0, 0), x - 92, niz - 8, 184, 16);

        Pryamougolnik(g, Alfa(Color.LightBlue, 70), x - 84, niz - 112, 168, 110);
        Pryamougolnik(g, metall, x - 88, niz - 118, 4, 118);
        Pryamougolnik(g, metall, x + 84, niz - 118, 4, 118);
        Skruglennyy(g, metall, x - 100, niz - 126, 200, 10, 4);
        Pryamougolnik(g, Temno(Color.Sienna, svet), x - 60, niz - 42, 120, 8);

        Skruglennyy(g, Color.FromArgb(235, 25, 110, 180), x + 44, niz - 110, 34, 34, 7);
        Tekst(g, "А", Interfeys.sredniy, Color.White, x + 44, niz - 104, 34, true);
        Tekst(g, mesto.nazvanie, Interfeys.melkiy, Color.White, x - 80, niz - 106, 120, false);
    }

    static void NarisovatNahodku(Graphics g, Nahodka nahodka, double x, double vremya)
    {
        double y = nahodka.y;
        double blesk = 70 + Math.Sin(vremya * 4 + nahodka.x) * 60;

        Svechenie(g, Alfa(Color.LightYellow, blesk), x, y - 7, 25);
        Oval(g, Color.FromArgb(80, 0, 0, 0), x - 12, y - 2, 24, 6);

        if (nahodka.vid == "butylka")
        {
            Skruglennyy(g, Color.FromArgb(70, 150, 90), x - 12, y - 11, 21, 9, 4);
            Pryamougolnik(g, Color.FromArgb(55, 125, 76), x + 7, y - 9, 9, 4);
            Pryamougolnik(g, Color.FromArgb(160, 215, 175), x - 8, y - 9, 11, 2);
        }
        else if (nahodka.vid == "banka")
        {
            Skruglennyy(g, Color.FromArgb(200, 60, 50), x - 8, y - 18, 15, 19, 3);
            Pryamougolnik(g, Color.Silver, x - 7, y - 19, 13, 3);
            Pryamougolnik(g, Color.FromArgb(240, 200, 190), x - 4, y - 14, 3, 11);
        }
        else if (nahodka.vid == "moneta")
        {
            Krug(g, Color.FromArgb(210, 165, 50), x, y - 9, 8);
            Krug(g, Color.FromArgb(250, 215, 110), x, y - 9, 5);
        }
        else if (nahodka.vid == "metall")
        {
            Krug(g, Color.FromArgb(120, 125, 135), x, y - 11, 11);
            Krug(g, Color.FromArgb(60, 62, 70), x, y - 11, 4);
            for (int i = 0; i < 6; i++)
            {
                double ugol = i * Math.PI / 3;
                Krug(g, Color.FromArgb(120, 125, 135), x + Math.Cos(ugol) * 12, y - 11 + Math.Sin(ugol) * 12, 3);
            }
        }
        else
        {
            Krug(g, Color.FromArgb(205, 50, 55), x, y - 9, 8);
            Liniya(g, Color.FromArgb(80, 50, 30), 2, x, y - 16, x + 2, y - 21);
            Oval(g, Color.FromArgb(70, 140, 60), x + 2, y - 22, 9, 4);
        }
    }

    static void Geroy(Graphics g, Igrok ig, double x, double y, double masshtab, double shag)
    {
        viktor.ryukzak = true;
        viktor.ushanka = !ig.odezhda;
        viktor.odezhda = Color.FromArgb(112, 92, 72);
        if (ig.odezhda) viktor.odezhda = Color.FromArgb(70, 110, 125);
        if (ig.kurtka) viktor.odezhda = Color.FromArgb(50, 84, 128);

        Chelovek(g, x, y, masshtab, ig.smotritVpravo, shag, false, viktor);
    }

    // Рисует человека ногами в точке x, y. Рисунок всегда смотрит вправо,
    // а отрицательный масштаб отражает его влево
    public static void Chelovek(Graphics g, double x, double y, double masshtab, bool vpravo, double shag, bool sidit, Vneshnost vid)
    {
        GraphicsState sostoyanie = g.Save();
        g.TranslateTransform((float)x, (float)y);
        g.ScaleTransform((float)(vpravo ? masshtab : -masshtab), (float)masshtab);

        double noga = Math.Sin(shag) * 13;
        Color kozha = Color.FromArgb(226, 190, 160);
        Color shtany = Color.FromArgb(44, 46, 56);
        Color botinki = Color.FromArgb(28, 26, 26);

        if (sidit)
        {
            Liniya(g, shtany, 11, -2, -46, 26, -44);
            Liniya(g, shtany, 10, 26, -44, 27, -9);
            Skruglennyy(g, botinki, 22, -9, 22, 9, 3);
        }
        else
        {
            Oval(g, Color.FromArgb(85, 0, 0, 0), -28, -6, 56, 12);
            Liniya(g, shtany, 10, -3, -46, -3 + noga, -5);
            Liniya(g, shtany, 10, 3, -46, 3 - noga, -5);
            Skruglennyy(g, botinki, -8 + noga, -9, 20, 9, 3);
            Skruglennyy(g, botinki, -2 - noga, -9, 20, 9, 3);
        }

        if (vid.ryukzak)
        {
            Skruglennyy(g, Color.FromArgb(122, 92, 58), -30, -98, 17, 44, 5);
            Skruglennyy(g, Color.FromArgb(84, 96, 70), -30, -106, 20, 10, 4);
        }

        if (vid.zhenshina)
        {
            Skruglennyy(g, vid.volosy, -15, -127, 24, 34, 8);
            Mnogougolnik(g, vid.odezhda, -14, -98, 14, -98, 21, -36, -21, -36);
        }
        else
        {
            Skruglennyy(g, vid.odezhda, -17, -100, 34, 62, 8);
        }

        Liniya(g, Smes(vid.odezhda, Color.Black, 0.32), 9, 0, -90, noga * 0.8, -52);
        Krug(g, kozha, noga * 0.8, -50, 5);
        Skruglennyy(g, Smes(vid.odezhda, Color.White, 0.2), -13, -103, 26, 8, 3);

        Krug(g, kozha, 1, -115, 14);
        if (!vid.zhenshina) Oval(g, vid.boroda, -6, -109, 20, 15);
        Krug(g, Color.FromArgb(30, 30, 35), 8, -119, 1.8);
        Liniya(g, Color.FromArgb(70, 50, 40), 1.4, 5, -124, 11, -124);

        if (vid.ushanka)
        {
            Skruglennyy(g, vid.volosy, -15, -134, 31, 15, 6);
            Skruglennyy(g, Smes(vid.volosy, Color.Black, 0.3), -14, -125, 9, 17, 3);
        }
        else
        {
            Skruglennyy(g, vid.volosy, -13, -131, 27, 11, 5);
            Skruglennyy(g, vid.volosy, -14, -126, 7, 11, 3);
        }

        g.Restore(sostoyanie);
    }

    public static void Sneg(Graphics g, double vremya, int kolichestvo)
    {
        for (int i = 0; i < kolichestvo; i++)
        {
            double veter = Math.Sin(vremya * 0.8 + i) * 16;
            double x = (i * 137 + vremya * 28 + veter) % Shirina;
            double y = (i * 89 + vremya * (36 + i % 28)) % Vysota;
            Krug(g, Color.FromArgb(225, 255, 255, 255), x, y, 1.3 + i % 3 * 0.7);
        }
    }

    static void Dozhd(Graphics g, double vremya)
    {
        for (int i = 0; i < 130; i++)
        {
            double x = (i * 151 - vremya * 90) % Shirina;
            if (x < 0) x += Shirina;
            double y = (i * 97 + vremya * 620) % Vysota;
            Liniya(g, Color.FromArgb(120, 190, 210, 235), 1.2, x, y, x - 4, y + 18);
        }
    }
}
