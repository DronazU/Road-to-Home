using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using static Kisti;

// Кадры вступления и концовок
static class Kartinki
{
    const int W = Risovanie.Shirina, H = Risovanie.Vysota;

    public static void Risovat(Graphics g, Replika replika, Gorod gorod)
    {
        double vremya = gorod.vremyaRepliki;
        double priblizhenie = 1 + Math.Min(vremya, 20) * 0.006;

        // Камера медленно наезжает на кадр
        GraphicsState sostoyanie = g.Save();
        g.TranslateTransform(W / 2, H / 2);
        g.ScaleTransform((float)priblizhenie, (float)priblizhenie);
        g.TranslateTransform(-W / 2, -H / 2);

        switch (replika.kartinka)
        {
            case "zavod": Zavod(g, gorod.vremya); break;
            case "kvartira": Kvartira(g); break;
            case "vokzal": Vokzal(g); break;
            case "podval": Podval(g, gorod.vremya); break;
            case "sneg": Sneg(g, gorod.vremya); break;
            case "dom": Dom(g, gorod.vremya); break;
            default: Bolnica(g, gorod.vremya); break;
        }

        g.Restore(sostoyanie);

        if (replika.tekst != "") Interfeys.Subtitry(g, gorod, replika);

        // Кадр плавно появляется из темноты
        double zatemnenie = Math.Max(0, 1 - vremya) * 255;
        Pryamougolnik(g, Alfa(Color.Black, zatemnenie), 0, 0, W, H);
    }

    static Vneshnost Silhuet(Color cvet)
    {
        return Vneshnost.Muzhchina(cvet, cvet, cvet);
    }

    public static void Titul(Graphics g, double vremya, bool sPodskazkoy)
    {
        Gradient(g, Color.FromArgb(8, 10, 28), Color.FromArgb(74, 44, 84), 0, 0, W, 300);
        Gradient(g, Color.FromArgb(74, 44, 84), Color.FromArgb(226, 124, 80), 0, 300, W, 150);

        for (int i = 0; i < 80; i++)
        {
            Krug(g, Alfa(Color.White, 120 + Math.Sin(vremya * 2 + i) * 90), i * 173 % W, i * 67 % 260, 0.8 + i % 3 * 0.5);
        }

        for (int i = 0; i < 22; i++)
        {
            int vysota = 90 + i * 71 % 160;
            Pryamougolnik(g, Color.FromArgb(16, 14, 30), i * 50 - 10, 440 - vysota, 54, vysota);
            for (int okno = 0; okno < 6; okno++)
            {
                if ((i + okno * 3) % 4 == 0) Pryamougolnik(g, Color.FromArgb(246, 204, 120), i * 50 + 6 + okno % 2 * 20, 440 - vysota + 14 + (int)(okno / 2) * 24, 10, 13);
            }
        }

        // Одно тёплое окно вдали: туда он и идёт
        Svechenie(g, Color.FromArgb(235, 255, 214, 130), 760, 330, 150);
        Pryamougolnik(g, Color.FromArgb(255, 234, 170), 744, 306, 34, 46);

        Pryamougolnik(g, Color.FromArgb(10, 10, 20), 0, 440, W, 120);
        Mnogougolnik(g, Color.FromArgb(46, 34, 44), 340, 560, 660, 560, 530, 440, 492, 440);
        if (sPodskazkoy) Risovanie.Chelovek(g, 500, 520, 1.3, true, vremya * 4, false, Silhuet(Color.FromArgb(10, 10, 18)));
        Risovanie.Sneg(g, vremya, 90);

        double yNazvaniya = sPodskazkoy ? 110 : 62;
        Tekst(g, "ДОРОГА ДОМОЙ", Interfeys.zagolovok, Color.FromArgb(120, 0, 0, 0), 3, yNazvaniya + 3, W, true);
        Tekst(g, "ДОРОГА ДОМОЙ", Interfeys.zagolovok, Risovanie.yantar, 0, yNazvaniya, W, true);
        if (sPodskazkoy) Tekst(g, "История одного возвращения", Interfeys.krupniy, Color.FromArgb(240, 230, 214), 0, 192, W, true);
        if (sPodskazkoy) Tekst(g, "Enter - начать", Interfeys.sredniy, Alfa(Color.White, 150 + Math.Sin(vremya * 3) * 100), 0, 530, W, true);
    }

    static void Zavod(Graphics g, double vremya)
    {
        Gradient(g, Color.FromArgb(20, 22, 30), Color.FromArgb(70, 56, 46), 0, 0, W, 420);
        Gradient(g, Color.FromArgb(44, 40, 38), Color.FromArgb(20, 18, 18), 0, 420, W, 140);

        for (int i = 0; i < 6; i++)
        {
            Pryamougolnik(g, Color.FromArgb(30, 32, 40), 40 + i * 190, 0, 26, 420);
            Svechenie(g, Color.FromArgb(70, 255, 220, 170), 120 + i * 190, 40, 170);
        }

        Pryamougolnik(g, Color.FromArgb(52, 84, 96), 190, 320, 560, 46);
        Skruglennyy(g, Color.FromArgb(60, 100, 114), 190, 230, 150, 100, 10);
        Skruglennyy(g, Color.FromArgb(60, 100, 114), 620, 270, 120, 56, 10);
        Pryamougolnik(g, Color.Silver, 340, 272, 280, 16);
        Pryamougolnik(g, Color.FromArgb(40, 64, 76), 230, 366, 60, 100);
        Pryamougolnik(g, Color.FromArgb(40, 64, 76), 640, 366, 60, 100);

        Svechenie(g, Color.FromArgb(235, 255, 170, 70), 480, 280, 150);
        for (int i = 0; i < 40; i++)
        {
            double faza = (vremya * 0.9 + i / 40.0) % 1;
            double x = 480 + (i - 20) * 9 * faza;
            double y = 280 + faza * faza * 220 - Math.Sin(faza * 3) * 60;
            Krug(g, Alfa(Color.Gold, (1 - faza) * 255), x, y, 2.2);
        }

        Vneshnost rabochiy = Vneshnost.Muzhchina(Color.FromArgb(44, 62, 92), Color.FromArgb(40, 34, 30), Color.FromArgb(226, 190, 160));
        Risovanie.Chelovek(g, 400, 500, 2.05, true, 0.4, false, rabochiy);
    }

    static void Kvartira(Graphics g)
    {
        Gradient(g, Color.FromArgb(62, 66, 84), Color.FromArgb(120, 112, 112), 0, 0, W, 410);
        Gradient(g, Color.FromArgb(80, 60, 48), Color.FromArgb(44, 32, 26), 0, 410, W, 150);

        // Светлый след на стене от снятой картины
        Pryamougolnik(g, Color.FromArgb(150, 150, 146), 140, 120, 150, 110);

        Pryamougolnik(g, Color.FromArgb(226, 226, 220), 610, 70, 270, 240);
        Gradient(g, Color.FromArgb(120, 134, 160), Color.FromArgb(206, 200, 196), 620, 80, 250, 220);
        Pryamougolnik(g, Color.FromArgb(226, 226, 220), 742, 80, 6, 220);
        Mnogougolnik(g, Color.FromArgb(40, 255, 250, 230), 620, 300, 870, 300, 620, 560, 200, 560);

        // Коробки и табличка в руках
        for (int i = 0; i < 4; i++)
        {
            double x = 170 + i * 104;
            double y = 430 - i % 2 * 30;
            Pryamougolnik(g, Color.FromArgb(166, 124, 80), x, y, 100, 80 + i % 2 * 30);
            Pryamougolnik(g, Color.FromArgb(214, 196, 150), x + 42, y, 16, 80 + i % 2 * 30);
        }

        Risovanie.Chelovek(g, 640, 520, 2.0, false, 0, false, Vneshnost.Muzhchina(Color.FromArgb(70, 76, 92), Color.FromArgb(44, 40, 38), Color.FromArgb(226, 190, 160)));
        Skruglennyy(g, Color.Crimson, 40, 34, 400, 84, 8);
        Tekst(g, "ПРОДАНО", Interfeys.zagolovok, Color.White, 40, 38, 400, true);
    }

    static void Vokzal(Graphics g)
    {
        Gradient(g, Color.FromArgb(32, 34, 46), Color.FromArgb(96, 84, 74), 0, 0, W, 420);
        Gradient(g, Color.FromArgb(68, 62, 58), Color.FromArgb(28, 26, 26), 0, 420, W, 140);

        for (int i = 0; i < 5; i++)
        {
            double x = 60 + i * 190;
            Oval(g, Color.FromArgb(120, 140, 170), x, 60, 150, 120);
            Pryamougolnik(g, Color.FromArgb(120, 140, 170), x, 120, 150, 230);
            Mnogougolnik(g, Color.FromArgb(40, 255, 255, 240), x, 350, x + 150, 350, x + 240, 560, x + 40, 560);
        }

        // Толпа спешит мимо, а сумка уже далеко
        for (int i = 0; i < 9; i++)
        {
            Risovanie.Chelovek(g, 40 + i * 118, 430 + i % 3 * 22, 1.3 + i % 3 * 0.1, i % 2 == 0, i * 1.3, false, Silhuet(Color.FromArgb(24, 24, 32)));
        }

        Risovanie.Chelovek(g, 500, 530, 2.15, false, 0.6, false, Vneshnost.Muzhchina(Color.FromArgb(82, 76, 70), Color.FromArgb(44, 40, 38), Color.FromArgb(82, 72, 66)));
        Svechenie(g, Color.FromArgb(160, 255, 80, 60), 770, 370, 80);
        Skruglennyy(g, Color.FromArgb(110, 60, 44), 738, 340, 66, 48, 8);
        Tekst(g, "?", Interfeys.zagolovok, Color.Tomato, 744, 262, 60, true);
    }

    static void Podval(Graphics g, double vremya)
    {
        Gradient(g, Color.FromArgb(12, 12, 16), Color.FromArgb(52, 46, 42), 0, 0, W, 430);
        Gradient(g, Color.FromArgb(40, 36, 34), Color.FromArgb(14, 12, 12), 0, 430, W, 130);

        Skruglennyy(g, Color.FromArgb(60, 46, 38), 0, 60, W, 20, 9);
        double drozh = 200 + Math.Sin(vremya * 9) * 24;
        Svechenie(g, Alfa(Color.FromArgb(255, 180, 90), drozh), 610, 400, 330);

        Skruglennyy(g, Color.FromArgb(96, 88, 78), 180, 450, 520, 48, 14);
        Skruglennyy(g, Color.FromArgb(120, 50, 44), 340, 436, 360, 44, 14);

        Pryamougolnik(g, Color.FromArgb(240, 230, 200), 604, 410, 12, 44);
        Oval(g, Color.FromArgb(255, 226, 130), 603, 390 + Math.Sin(vremya * 11), 14, 24);

        Risovanie.Chelovek(g, 300, 480, 2.0, true, 0, true, Risovanie.viktor);

        // Фотография дочери в руке
        Pryamougolnik(g, Color.WhiteSmoke, 352, 286, 66, 82);
        Gradient(g, Color.FromArgb(150, 180, 200), Color.FromArgb(220, 210, 180), 358, 292, 54, 58);
        Krug(g, Color.FromArgb(226, 190, 160), 385, 316, 13);
        Skruglennyy(g, Color.FromArgb(120, 76, 50), 370, 300, 30, 12, 6);
        Mnogougolnik(g, Color.FromArgb(200, 110, 110), 372, 350, 398, 350, 392, 330, 378, 330);
    }

    static void Sneg(Graphics g, double vremya)
    {
        Gradient(g, Color.FromArgb(40, 54, 84), Color.FromArgb(170, 186, 206), 0, 0, W, 440);
        Gradient(g, Color.FromArgb(226, 234, 244), Color.FromArgb(190, 200, 216), 0, 440, W, 120);

        for (int i = 0; i < 16; i++)
        {
            int vysota = 90 + i * 83 % 170;
            Pryamougolnik(g, Color.FromArgb(70, 84, 112), i * 72 - 20, 440 - vysota, 66, vysota);
        }

        Liniya(g, Color.FromArgb(50, 50, 60), 6, 840, 440, 840, 190);
        Liniya(g, Color.FromArgb(50, 50, 60), 4, 840, 192, 890, 182);
        Svechenie(g, Color.FromArgb(210, 255, 220, 150), 886, 196, 120);

        Oval(g, Color.FromArgb(120, 70, 80, 110), 380, 494, 240, 22);
        Risovanie.Chelovek(g, 500, 506, 2.2, true, 0.5, false, Risovanie.viktor);
        Risovanie.Sneg(g, vremya * 1.6, 170);
    }

    static void Dom(Graphics g, double vremya)
    {
        Gradient(g, Color.FromArgb(142, 92, 60), Color.FromArgb(214, 166, 112), 0, 0, W, 410);
        Gradient(g, Color.FromArgb(120, 76, 52), Color.FromArgb(74, 44, 32), 0, 410, W, 150);

        Pryamougolnik(g, Color.FromArgb(232, 222, 200), 590, 60, 300, 270);
        Gradient(g, Color.FromArgb(24, 26, 62), Color.FromArgb(120, 76, 110), 600, 70, 280, 250);
        Pryamougolnik(g, Color.FromArgb(232, 222, 200), 736, 70, 8, 250);
        for (int i = 0; i < 26; i++)
        {
            double y = 70 + (i * 97 + vremya * 26) % 250;
            Krug(g, Color.White, 600 + i * 41 % 280, y, 1.6);
        }

        Svechenie(g, Color.FromArgb(200, 255, 220, 140), 220, 170, 240);
        Pryamougolnik(g, Color.FromArgb(40, 40, 44), 219, 0, 2, 130);
        Oval(g, Color.FromArgb(255, 236, 170), 196, 128, 48, 22);

        // Стол, чай и диван
        Skruglennyy(g, Color.FromArgb(120, 70, 60), 80, 400, 250, 82, 20);
        Skruglennyy(g, Color.FromArgb(146, 86, 72), 80, 346, 250, 72, 20);
        Pryamougolnik(g, Color.FromArgb(150, 104, 70), 390, 410, 240, 12);
        Pryamougolnik(g, Color.FromArgb(110, 74, 48), 406, 422, 10, 74);
        Pryamougolnik(g, Color.FromArgb(110, 74, 48), 604, 422, 10, 74);
        Skruglennyy(g, Color.WhiteSmoke, 470, 386, 30, 26, 5);
        Skruglennyy(g, Color.WhiteSmoke, 526, 386, 30, 26, 5);

        Vneshnost chistiy = Vneshnost.Muzhchina(Color.FromArgb(70, 110, 125), Color.FromArgb(55, 50, 50), Color.FromArgb(82, 72, 66));
        Risovanie.Chelovek(g, 720, 510, 2.0, false, 0, false, chistiy);
    }

    static void Bolnica(Graphics g, double vremya)
    {
        Gradient(g, Color.FromArgb(8, 10, 26), Color.FromArgb(30, 44, 70), 0, 0, W, 440);
        Gradient(g, Color.FromArgb(30, 34, 46), Color.FromArgb(10, 10, 14), 0, 440, W, 120);

        Pryamougolnik(g, Color.FromArgb(120, 130, 146), 220, 120, 560, 320);
        for (int ryad = 0; ryad < 4; ryad++)
        {
            for (int i = 0; i < 8; i++)
            {
                bool gorit = (ryad * 3 + i) % 3 != 0;
                Pryamougolnik(g, gorit ? Color.FromArgb(190, 220, 236) : Color.FromArgb(40, 54, 76), 244 + i * 66, 140 + ryad * 70, 44, 48);
            }
        }

        Pryamougolnik(g, Color.WhiteSmoke, 460, 76, 80, 44);
        Pryamougolnik(g, Color.Crimson, 492, 82, 16, 32);
        Pryamougolnik(g, Color.Crimson, 484, 90, 32, 16);

        // Мигалка скорой
        double migalka = (Math.Sin(vremya * 7) + 1) / 2;
        Svechenie(g, Alfa(Color.DodgerBlue, 60 + migalka * 170), 200, 436, 150);
        Skruglennyy(g, Color.WhiteSmoke, 110, 440, 190, 76, 10);
        Pryamougolnik(g, Color.Crimson, 110, 474, 190, 8);
        Krug(g, Color.FromArgb(20, 20, 24), 150, 516, 14);
        Krug(g, Color.FromArgb(20, 20, 24), 262, 516, 14);
        Risovanie.Sneg(g, vremya, 100);
    }
}
