using System;
using System.Collections.Generic;
using System.Drawing;
using static Kisti;

// Всё, что рисуется поверх игры: шкалы, карта, меню и диалоги
static class Interfeys
{
    public static Font melkiy = new Font("Segoe UI", 15, GraphicsUnit.Pixel);
    public static Font sredniy = new Font("Segoe UI", 18, FontStyle.Bold, GraphicsUnit.Pixel);
    public static Font krupniy = new Font("Segoe UI", 22, GraphicsUnit.Pixel);
    public static Font zagolovok = new Font("Georgia", 60, FontStyle.Bold, GraphicsUnit.Pixel);

    static Color temnaya = Color.FromArgb(18, 21, 29);
    static Color steklo = Color.FromArgb(222, 18, 21, 29);
    static Color yantar = Risovanie.yantar;
    static Vneshnost katya = Vneshnost.Zhenshina(Color.FromArgb(90, 130, 160), Color.FromArgb(120, 76, 50));

    public static void Risovat(Graphics g, Gorod gorod, Mesto otkrytoe)
    {
        Panel(g, gorod);
        if (gorod.vnutri == null) Minikarta(g, gorod);

        if (gorod.ochered.Count > 0)
        {
            Dialog(g, gorod);
            return;
        }

        Podskazka(g, gorod, otkrytoe);
        Vsplyvashka(g, gorod);
        if (otkrytoe != null) Menu(g, gorod, otkrytoe);
        if (gorod.vremyaSoobsheniya > 0) Soobshenie(g, gorod.soobshenie);
        if (gorod.vremyaBannera > 0) Banner(g, gorod);
    }

    static void Panel(Graphics g, Gorod gorod)
    {
        Igrok ig = gorod.ig;
        Gradient(g, Color.FromArgb(240, 14, 17, 24), Color.FromArgb(205, 22, 26, 36), 0, 0, Risovanie.Shirina, 62);
        Pryamougolnik(g, yantar, 0, 62, Risovanie.Shirina, 2);

        Shkala(g, "Еда", ig.eda, Color.FromArgb(242, 165, 60), 14, 8);
        Shkala(g, "Тепло", ig.teplo, Color.FromArgb(235, 95, 75), 14, 33);
        Shkala(g, "Силы", ig.sily, Color.FromArgb(90, 200, 110), 270, 8);
        Shkala(g, "Надежда", ig.nadezhda, Color.FromArgb(80, 160, 240), 270, 33);

        string minuty = ((int)(ig.chas % 1 * 60)).ToString("00");
        string stroka = "День " + ig.den + "    " + (int)(ig.chas) + ":" + minuty + "    " + ig.Temperatura() + " °C";
        Tekst(g, stroka, sredniy, Color.White, 530, 5, 270, false);
        Tekst(g, ig.dengi + " р.", sredniy, yantar, 530, 33, 90, false);
        Tekst(g, "Бутылки " + ig.butylki + "   Банки " + ig.banki + "   Металл " + ig.metall, melkiy, Color.Gainsboro, 620, 35, 300, false);

        string mesto = gorod.vnutri != null ? gorod.vnutri.nazvanie : gorod.Rayon();
        Skruglennyy(g, Color.FromArgb(50, 54, 66), 826, 7, 160, 24, 7);
        Tekst(g, mesto, melkiy, Color.White, 826, 10, 160, true);

        Skruglennyy(g, steklo, 8, 70, 620, 28, 8);
        Skruglennyy(g, yantar, 16, 77, 5, 14, 2);
        Tekst(g, gorod.Tsel(), melkiy, Color.Khaki, 28, 75, 594, false);
    }

    static void Shkala(Graphics g, string nazvanie, double znachenie, Color cvet, double x, double y)
    {
        // Подпись краснеет, когда значение опасно низкое
        Color podpis = znachenie < 25 ? Color.FromArgb(255, 120, 110) : Color.White;
        Tekst(g, nazvanie, melkiy, podpis, x, y - 2, 86, false);
        Skruglennyy(g, Color.FromArgb(52, 55, 66), x + 88, y + 3, 150, 15, 7);
        Skruglennyy(g, cvet, x + 88, y + 3, znachenie * 1.5, 15, 7);
        Pryamougolnik(g, Color.FromArgb(55, 255, 255, 255), x + 96, y + 5, Math.Max(0, znachenie * 1.5 - 16), 2);
    }

    public static void Pauza(Graphics g)
    {
        Pryamougolnik(g, Color.FromArgb(170, 0, 0, 0), 0, 0, Risovanie.Shirina, Risovanie.Vysota);
        Skruglennyy(g, Color.FromArgb(245, 22, 25, 34), 280, 160, 440, 240, 16);
        Tekst(g, "Пауза", sredniy, Color.White, 280, 182, 440, true);
        Tekst(g, "Игра сохранена", melkiy, Color.FromArgb(90, 200, 110), 280, 214, 440, true);
        Tekst(g, "Esc - продолжить", melkiy, Color.White, 280, 262, 440, true);
        Tekst(g, "Q - выйти в главное меню", melkiy, Color.White, 280, 292, 440, true);
        Tekst(g, Zvuk.vyklyucheno ? "M - включить звук" : "M - выключить звук", melkiy, Color.White, 280, 322, 440, true);
        Tekst(g, "WASD - ходить, E - действие", melkiy, Color.Silver, 280, 360, 440, true);
    }

    // Полоска внизу показывает всю улицу, героя и цель задания
    static void Minikarta(Graphics g, Gorod gorod)
    {
        double levo = 30, shirina = 940, y = 548;

        Skruglennyy(g, Color.FromArgb(190, 16, 18, 26), levo - 14, y - 11, shirina + 28, 20, 9);
        Pryamougolnik(g, Color.FromArgb(120, 150, 150, 160), levo, y - 1, shirina, 2);

        foreach (Mesto mesto in gorod.mesta)
        {
            double x = levo + shirina * mesto.x / Gorod.Dlina;
            if (mesto.vid == "ostanovka") Pryamougolnik(g, Color.DeepSkyBlue, x - 3, y - 3, 6, 6);
            else if (mesto.vid == "zdanie") Pryamougolnik(g, mesto.cvet, x - 2, y - 6, 4, 8);
            else Krug(g, Color.Gray, x, y, 2);

            bool tsel = mesto.nazvanie == gorod.NazvanieTseli();
            if (tsel)
            {
                double puls = 5 + Math.Sin(gorod.vremya * 5) * 1.6;
                Svechenie(g, Alfa(yantar, 170), x, y, puls * 3);
                Krug(g, yantar, x, y, puls);
            }
        }

        double xGeroya = levo + shirina * gorod.ig.x / Gorod.Dlina;
        Mnogougolnik(g, Color.White, xGeroya, y + 1, xGeroya - 6, y - 10, xGeroya + 6, y - 10);
    }

    static void Podskazka(Graphics g, Gorod gorod, Mesto otkrytoe)
    {
        if (otkrytoe != null) return;

        Igrok ig = gorod.ig;
        string tekst = "";
        double x = 500, y = 200;

        if (gorod.vnutri != null)
        {
            if (gorod.RyadomSVyhodom()) tekst = "Выйти на улицу";
            else if (gorod.RyadomSRabotnikom()) tekst = gorod.vnutri.rabotnik;
            x = ig.xKomnaty;
            y = ig.yKomnaty - 240;
        }
        else
        {
            Mesto ryadom = gorod.MestoRyadom();
            if (ryadom != null) tekst = ryadom.vid == "zdanie" ? "Войти: " + ryadom.nazvanie : ryadom.nazvanie;
            x = ig.x - Math.Clamp(ig.x - Risovanie.Shirina / 2, 0, Gorod.Dlina - Risovanie.Shirina);
            y = ig.y - 190;
        }

        if (tekst == "") return;

        x = Math.Clamp(x, 130, Risovanie.Shirina - 130);
        Skruglennyy(g, steklo, x - 120, y, 240, 32, 9);
        Skruglennyy(g, yantar, x - 113, y + 5, 22, 22, 6);
        Tekst(g, "E", melkiy, temnaya, x - 113, y + 6, 22, true);
        Tekst(g, tekst, melkiy, Color.White, x - 86, y + 6, 200, true);
    }

    static void Vsplyvashka(Graphics g, Gorod gorod)
    {
        if (gorod.vremyaVsplyvashki <= 0 || gorod.vnutri != null) return;

        Igrok ig = gorod.ig;
        double kamera = Math.Clamp(ig.x - Risovanie.Shirina / 2, 0, Gorod.Dlina - Risovanie.Shirina);
        double podyom = (1.5 - gorod.vremyaVsplyvashki) * 30;
        Color cvet = Alfa(yantar, gorod.vremyaVsplyvashki / 1.5 * 255);
        Tekst(g, gorod.vsplyvashka, sredniy, cvet, ig.x - kamera - 100, ig.y - 170 - podyom, 200, true);
    }

    static void Banner(Graphics g, Gorod gorod)
    {
        double alfa = Math.Min(1, gorod.vremyaBannera) * 245;
        Skruglennyy(g, Alfa(temnaya, alfa), 200, 152, 600, 70, 12);
        Skruglennyy(g, Alfa(yantar, alfa), 200, 152, 600, 4, 2);
        Tekst(g, "НОВОЕ ЗАДАНИЕ", melkiy, Alfa(yantar, alfa), 200, 162, 600, true);
        Tekst(g, gorod.banner, melkiy, Alfa(Color.White, alfa), 216, 188, 568, true);
    }

    static void Menu(Graphics g, Gorod gorod, Mesto mesto)
    {
        List<Deystvie> spisok = gorod.VidimyeDeystviya(mesto);
        double vysota = 102 + spisok.Count * 42;
        double x = 150, y = 108;

        Skruglennyy(g, Color.FromArgb(120, 0, 0, 0), x + 6, y + 8, 700, vysota, 16);
        Skruglennyy(g, Color.FromArgb(246, 22, 25, 34), x, y, 700, vysota, 16);
        Skruglennyy(g, mesto.cvet, x + 22, y + 18, 6, 26, 3);
        Tekst(g, mesto.rabotnik != "" ? mesto.rabotnik : mesto.nazvanie, sredniy, Color.White, x + 38, y + 18, 400, false);
        Tekst(g, "открыто до " + mesto.otkrytoDo + ":00", melkiy, Color.Silver, x + 440, y + 20, 238, false);

        for (int i = 0; i < spisok.Count; i++)
        {
            bool dostupno = gorod.Dostupno(spisok[i]);
            double strokaY = y + 58 + i * 42;
            Skruglennyy(g, Color.FromArgb(42, 46, 58), x + 22, strokaY, 656, 36, 8);
            Skruglennyy(g, dostupno ? yantar : Color.FromArgb(78, 80, 90), x + 28, strokaY + 6, 24, 24, 6);
            Tekst(g, (i + 1).ToString(), melkiy, temnaya, x + 28, strokaY + 8, 24, true);
            Tekst(g, spisok[i].tekst, melkiy, dostupno ? Color.White : Color.Gray, x + 62, strokaY + 8, 610, false);
        }

        Tekst(g, "Цифра - выбрать, Esc - закрыть", melkiy, Color.Silver, x + 22, y + vysota - 34, 656, false);
    }

    static void Soobshenie(Graphics g, string tekst)
    {
        Skruglennyy(g, steklo, 90, 104, 820, 40, 10);
        Tekst(g, tekst, melkiy, Color.White, 100, 114, 800, true);
    }

    static Vneshnost KtoGovorit(Gorod gorod, string kto)
    {
        if (kto == "Виктор") return Risovanie.viktor;
        if (kto == "Катя") return katya;

        foreach (Mesto mesto in gorod.mesta)
        {
            if (mesto.rabotnik == kto) return mesto.vneshnost;
        }
        return null;
    }

    // Окно разговора с портретом и текстом, который печатается по буквам
    static void Dialog(Graphics g, Gorod gorod)
    {
        Replika replika = gorod.ochered[0];
        int bukv = (int)(gorod.vremyaRepliki * 36);
        string vidno = bukv >= replika.tekst.Length ? replika.tekst : replika.tekst.Substring(0, bukv);
        double y = 406;

        Gradient(g, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(190, 0, 0, 0), 0, 320, Risovanie.Shirina, 240);
        Skruglennyy(g, Color.FromArgb(242, 20, 23, 32), 30, y, 940, 140, 14);
        Skruglennyy(g, yantar, 30, y, 940, 3, 1);

        Vneshnost vid = KtoGovorit(gorod, replika.kto);
        if (vid != null)
        {
            Skruglennyy(g, Color.FromArgb(58, 66, 82), 46, y + 16, 108, 108, 12);
            g.SetClip(new RectangleF(46, (float)(y + 16), 108, 108));
            Risovanie.Chelovek(g, 98, y + 268, 2.1, true, 0, false, vid);
            g.ResetClip();
        }

        double x = vid != null ? 172 : 56;
        Tekst(g, replika.kto, sredniy, yantar, x, y + 15, 760, false);
        Tekst(g, vidno, melkiy, Color.White, x, y + 46, 770 + (172 - x), false);
        Tekst(g, "Enter - дальше", melkiy, Color.Silver, 810, y + 112, 150, true);
    }

    // Субтитры поверх кадра рассказа
    public static void Subtitry(Graphics g, Gorod gorod, Replika replika)
    {
        int bukv = (int)(gorod.vremyaRepliki * 36);
        string vidno = bukv >= replika.tekst.Length ? replika.tekst : replika.tekst.Substring(0, bukv);

        // Длинный текст начинается выше, чтобы поместиться на экране
        double y = replika.tekst.Length > 170 ? 352 : 440;

        Gradient(g, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(235, 0, 0, 0), 0, y - 90, Risovanie.Shirina, 110);
        Pryamougolnik(g, Color.FromArgb(235, 0, 0, 0), 0, y + 20, Risovanie.Shirina, Risovanie.Vysota - y - 20);
        Tekst(g, vidno, krupniy, Color.FromArgb(245, 242, 232), 80, y, 840, true);
        Tekst(g, "Enter - дальше", melkiy, Color.Silver, 0, 532, Risovanie.Shirina, true);
    }
}
