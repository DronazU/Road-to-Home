using System;
using System.Drawing;
using static Kisti;

// Внешний вид зданий. У каждого стиля своя форма
static class Zdaniya
{
    const int Stena = Risovanie.Stena;

    public static void Risovat(Graphics g, Gorod gorod, Mesto mesto, double x, double svet, double zima)
    {
        Oval(g, Color.FromArgb(60, 0, 0, 0), x - 165, Stena - 6, 330, 22);

        switch (mesto.stil)
        {
            case "panelka": Panelka(g, gorod, mesto, x, svet, zima); break;
            case "kirpich": Kirpich(g, gorod, mesto, x, svet, zima); break;
            case "magazin": Magazin(g, gorod, mesto, x, svet, zima); break;
            case "rynok": Rynok(g, gorod, mesto, x, svet, zima); break;
            case "klassika": Klassika(g, gorod, mesto, x, svet, zima); break;
            case "cerkov": Cerkov(g, gorod, mesto, x, svet, zima); break;
            case "vokzal": Vokzal(g, gorod, mesto, x, svet, zima); break;
            case "garazh": Garazh(g, gorod, mesto, x, svet, zima); break;
            default: Zavod(g, gorod, mesto, x, svet, zima); break;
        }
    }

    static bool Gorit(Mesto mesto, int ryad, int stolbec, int den)
    {
        return (mesto.x + ryad * 7 + stolbec * 13 + den) % 5 < 3;
    }

    static void Okoshko(Graphics g, double x, double y, double shirina, double vysota, Color rama, double svet, bool gorit)
    {
        Pryamougolnik(g, rama, x - 2, y - 2, shirina + 4, vysota + 4);

        if (svet < 0.5 && gorit)
        {
            Gradient(g, Color.FromArgb(255, 226, 150), Color.FromArgb(240, 170, 80), x, y, shirina, vysota);
            Svechenie(g, Color.FromArgb(70, 255, 210, 130), x + shirina / 2, y + vysota / 2, shirina * 1.6);
        }
        else
        {
            Gradient(g, Temno(Color.FromArgb(150, 195, 225), svet), Temno(Color.FromArgb(60, 90, 120), svet), x, y, shirina, vysota);
            Mnogougolnik(g, Color.FromArgb(50, 255, 255, 255), x + 3, y + vysota, x + shirina * 0.5, y, x + shirina * 0.75, y, x + shirina * 0.3, y + vysota);
        }

        Pryamougolnik(g, rama, x + shirina / 2 - 1, y, 2, vysota);
        Pryamougolnik(g, rama, x, y + vysota * 0.4, shirina, 2);
    }

    static void Dver(Graphics g, double x, double shirina, double vysota, Color cvet, double svet)
    {
        Pryamougolnik(g, Temno(Color.FromArgb(150, 150, 146), svet), x - shirina / 2 - 12, Stena - 6, shirina + 24, 6);
        Pryamougolnik(g, Smes(cvet, Color.White, 0.25), x - shirina / 2 - 4, Stena - vysota - 8, shirina + 8, vysota + 2);
        Gradient(g, Temno(cvet, svet), Temno(Smes(cvet, Color.Black, 0.3), svet), x - shirina / 2, Stena - vysota - 4, shirina, vysota - 2);
        Pryamougolnik(g, Smes(cvet, Color.Black, 0.5), x - 1, Stena - vysota - 4, 2, vysota - 2);
        Krug(g, Risovanie.yantar, x + shirina / 2 - 7, Stena - vysota / 2, 3);
    }

    // Вывеска: ночью светится, закрытое здание гаснет
    static void Vyveska(Graphics g, Gorod gorod, Mesto mesto, double x, double y, double shirina, double svet)
    {
        bool otkryto = mesto.Otkryto(gorod.ig.chas);
        Color neon = Smes(mesto.cvet, Color.White, 0.35);
        bool sveti = otkryto && svet < 0.55;

        if (sveti) Svechenie(g, Alfa(neon, 120), x, y + 16, shirina * 0.75);
        Skruglennyy(g, sveti ? neon : Temno(Smes(mesto.cvet, Color.White, 0.5), svet), x - shirina / 2, y, shirina, 34, 8);
        Skruglennyy(g, Color.FromArgb(18, 20, 28), x - shirina / 2 + 2.5, y + 2.5, shirina - 5, 29, 6);
        Tekst(g, mesto.nazvanie, Interfeys.melkiy, otkryto ? Smes(neon, Color.White, 0.3) : Color.Gray, x - shirina / 2, y + 7, shirina, true);
    }

    static void SnegNaKryshe(Graphics g, double x, double shirina, double y, double zima, double svet)
    {
        if (zima <= 0) return;
        Skruglennyy(g, Temno(Color.FromArgb(240, 245, 252), svet + 0.25), x - shirina / 2, y - 6 - zima * 8, shirina, 12 + zima * 8, 6);
    }

    static void Panelka(Graphics g, Gorod gorod, Mesto mesto, double x, double svet, double zima)
    {
        double vysota = 258, verh = Stena - vysota;
        Color cvet = Temno(mesto.cvet, svet);
        Color rama = Smes(cvet, Color.White, 0.3);

        Pryamougolnik(g, cvet, x - 150, verh, 300, vysota);
        Pryamougolnik(g, Smes(cvet, Color.Black, 0.2), x + 122, verh, 28, vysota);
        for (int i = 1; i < 5; i++) Pryamougolnik(g, Smes(cvet, Color.Black, 0.14), x - 150, verh + i * 50, 300, 2);
        for (int i = 0; i < 3; i++) Pryamougolnik(g, Smes(cvet, Color.Black, 0.3), x - 100 + i * 100, verh, 3, vysota);
        Pryamougolnik(g, Smes(cvet, Color.Black, 0.4), x - 156, verh - 6, 312, 8);

        for (int ryad = 0; ryad < 4; ryad++)
        {
            for (int stolbec = 0; stolbec < 6; stolbec++)
            {
                if (ryad == 3 && stolbec > 1 && stolbec < 4) continue;
                double oknoX = x - 138 + stolbec * 47, oknoY = verh + 46 + ryad * 50;
                Okoshko(g, oknoX, oknoY, 28, 30, rama, svet, Gorit(mesto, ryad, stolbec, gorod.ig.den));
                if (ryad < 3 && stolbec % 2 == 1)
                {
                    Pryamougolnik(g, Smes(cvet, Color.Black, 0.25), oknoX - 5, oknoY + 28, 38, 8);
                }
            }
        }

        Mnogougolnik(g, Smes(cvet, Color.Black, 0.35), x - 42, Stena - 72, x + 42, Stena - 72, x + 50, Stena - 64, x - 50, Stena - 64);
        Dver(g, x, 46, 62, Color.FromArgb(70, 60, 56), svet);
        Vyveska(g, gorod, mesto, x, verh + 8, 200, svet);
        SnegNaKryshe(g, x, 318, verh - 2, zima, svet);
    }

    static void Kirpich(Graphics g, Gorod gorod, Mesto mesto, double x, double svet, double zima)
    {
        double vysota = 216, verh = Stena - vysota;
        Color cvet = Temno(mesto.cvet, svet);
        Color shov = Smes(cvet, Color.Black, 0.22);
        Color rama = Temno(Color.FromArgb(226, 214, 196), svet);

        Pryamougolnik(g, cvet, x - 150, verh, 300, vysota);

        for (int ryad = 0; ryad < 21; ryad++)
        {
            Pryamougolnik(g, shov, x - 150, verh + ryad * 10.3, 300, 1.4);
            double sdvig = ryad % 2 == 0 ? 0 : 12;
            for (int i = 0; i < 13; i++) Pryamougolnik(g, shov, x - 150 + sdvig + i * 24, verh + ryad * 10.3, 1.4, 10.3);
        }

        Pryamougolnik(g, Smes(cvet, Color.Black, 0.4), x - 158, verh - 10, 316, 13);
        Pryamougolnik(g, Smes(cvet, Color.White, 0.18), x - 150, verh + 3, 300, 5);

        for (int stolbec = 0; stolbec < 4; stolbec++)
        {
            double oknoX = x - 122 + stolbec * 68;
            Oval(g, rama, oknoX - 3, verh + 42, 44, 34);
            Okoshko(g, oknoX, verh + 58, 38, 44, rama, svet, Gorit(mesto, 0, stolbec, gorod.ig.den));
        }

        Pryamougolnik(g, Smes(cvet, Color.Black, 0.3), x - 150, Stena - 14, 300, 14);
        Dver(g, x, 54, 72, Color.FromArgb(92, 58, 40), svet);
        Mnogougolnik(g, Temno(Color.FromArgb(60, 60, 66), svet), x - 50, Stena - 86, x + 50, Stena - 86, x + 62, Stena - 72, x - 62, Stena - 72);
        Vyveska(g, gorod, mesto, x, verh + 2, 190, svet);

        if (mesto.nazvanie == "Баня") Dym(g, gorod, x + 104, verh - 40, svet);
        if (mesto.nazvanie == "Баня") Pryamougolnik(g, Smes(cvet, Color.Black, 0.3), x + 90, verh - 44, 26, 38);
        SnegNaKryshe(g, x, 322, verh - 8, zima, svet);
    }

    static void Dym(Graphics g, Gorod gorod, double x, double y, double svet)
    {
        for (int i = 0; i < 7; i++)
        {
            double faza = (gorod.vremya * 0.13 + i / 7.0) % 1;
            Krug(g, Alfa(Temno(Color.FromArgb(205, 205, 210), svet + 0.2), (1 - faza) * 120), x + faza * 62, y - faza * 118, 9 + faza * 26);
        }
    }

    static void Magazin(Graphics g, Gorod gorod, Mesto mesto, double x, double svet, double zima)
    {
        double vysota = 196, verh = Stena - vysota;
        Color cvet = Temno(mesto.cvet, svet);
        Color rama = Temno(Color.FromArgb(232, 228, 220), svet);
        bool otkryto = mesto.Otkryto(gorod.ig.chas);

        Pryamougolnik(g, cvet, x - 150, verh, 300, vysota);
        Pryamougolnik(g, Smes(cvet, Color.Black, 0.2), x + 128, verh, 22, vysota);
        Pryamougolnik(g, Smes(cvet, Color.White, 0.2), x - 156, verh - 8, 312, 10);

        for (int stolbec = 0; stolbec < 5; stolbec++)
        {
            Okoshko(g, x - 132 + stolbec * 54, verh + 50, 36, 34, rama, svet, Gorit(mesto, 1, stolbec, gorod.ig.den));
        }

        // Большие витрины с товаром
        foreach (double sdvig in new double[] { -140, 42 })
        {
            double vitrinaX = x + sdvig;
            Pryamougolnik(g, rama, vitrinaX - 3, Stena - 92, 104, 82);
            if (otkryto) Gradient(g, Color.FromArgb(255, 238, 190), Color.FromArgb(248, 200, 120), vitrinaX, Stena - 89, 98, 76);
            else Gradient(g, Temno(Color.FromArgb(70, 90, 110), svet), Temno(Color.FromArgb(40, 50, 66), svet), vitrinaX, Stena - 89, 98, 76);

            for (int polka = 0; polka < 2; polka++)
            {
                Pryamougolnik(g, Color.FromArgb(120, 60, 50, 40), vitrinaX, Stena - 56 + polka * 32, 98, 3);
                for (int i = 0; i < 6; i++)
                {
                    Color tovar = Temno(mesto.cvet, 0.4 + i * 0.1);
                    Skruglennyy(g, tovar, vitrinaX + 6 + i * 15, Stena - 76 + polka * 32, 11, 20, 3);
                }
            }

            Mnogougolnik(g, Color.FromArgb(46, 255, 255, 255), vitrinaX + 8, Stena - 13, vitrinaX + 44, Stena - 89, vitrinaX + 64, Stena - 89, vitrinaX + 28, Stena - 13);
            if (otkryto && svet < 0.55) Oval(g, Color.FromArgb(55, 255, 220, 150), vitrinaX - 18, Stena + 2, 136, 26);
        }

        Dver(g, x, 44, 76, Color.FromArgb(58, 72, 86), svet);

        // Полосатый навес с фестонами
        for (int i = 0; i < 14; i++)
        {
            Color polosa = i % 2 == 0 ? Temno(Smes(mesto.cvet, Color.Black, 0.3), svet) : Temno(Color.WhiteSmoke, svet);
            double levo = x - 154 + i * 22;
            Mnogougolnik(g, polosa, levo + 4, Stena - 118, levo + 26, Stena - 118, levo + 22, Stena - 94, levo, Stena - 94);
            Oval(g, polosa, levo, Stena - 101, 22, 14);
        }

        Vyveska(g, gorod, mesto, x, verh + 8, 200, svet);
        SnegNaKryshe(g, x, 322, verh - 6, zima, svet);
    }

    static void Rynok(Graphics g, Gorod gorod, Mesto mesto, double x, double svet, double zima)
    {
        Color cvet = Temno(mesto.cvet, svet);
        double verh = Stena - 210;

        Pryamougolnik(g, Temno(Color.FromArgb(150, 150, 150), svet), x - 150, Stena - 118, 300, 118);

        // Большой шатровый навес
        for (int i = 0; i < 10; i++)
        {
            Color polosa = i % 2 == 0 ? cvet : Temno(Color.WhiteSmoke, svet);
            double levo = x - 170 + i * 34;
            Mnogougolnik(g, polosa, x, verh, x, verh, levo + 34, Stena - 118, levo, Stena - 118);
            Oval(g, polosa, levo, Stena - 128, 34, 22);
        }

        // Прилавки с товаром
        for (int i = 0; i < 3; i++)
        {
            double prilavokX = x - 138 + i * 94;
            if (i == 1) continue;
            Pryamougolnik(g, Temno(Color.Peru, svet), prilavokX, Stena - 38, 90, 38);
            for (int t = 0; t < 8; t++) Krug(g, Temno(t % 2 == 0 ? Color.Tomato : Color.Orange, svet), prilavokX + 8 + t * 11, Stena - 44, 7);
        }

        Dver(g, x, 54, 76, Color.FromArgb(70, 80, 100), svet);
        Vyveska(g, gorod, mesto, x, Stena - 112, 170, svet);
        if (zima > 0) Mnogougolnik(g, Temno(Color.White, svet + 0.25), x, verh, x + 66, verh + 62, x - 66, verh + 62);
    }

    static void Klassika(Graphics g, Gorod gorod, Mesto mesto, double x, double svet, double zima)
    {
        double vysota = 232, verh = Stena - vysota;
        Color cvet = Temno(mesto.cvet, svet);
        Color kolonna = Temno(Color.FromArgb(238, 232, 220), svet);
        Color ten = Temno(Color.FromArgb(172, 166, 156), svet);

        Pryamougolnik(g, cvet, x - 152, verh + 30, 304, vysota - 30);

        // Треугольный фронтон
        Mnogougolnik(g, kolonna, x - 164, verh + 34, x, verh - 34, x + 164, verh + 34);
        Mnogougolnik(g, cvet, x - 130, verh + 26, x, verh - 18, x + 130, verh + 26);
        Pryamougolnik(g, kolonna, x - 164, verh + 32, 328, 14);
        Krug(g, kolonna, x, verh + 7, 13);
        Krug(g, cvet, x, verh + 7, 8);

        for (int stolbec = 0; stolbec < 5; stolbec++)
        {
            double oknoX = x - 124 + stolbec * 54;
            if (stolbec != 2) Okoshko(g, oknoX, verh + 104, 32, 52, kolonna, svet, Gorit(mesto, 0, stolbec, gorod.ig.den));
            if (stolbec != 2) Okoshko(g, oknoX, verh + 170, 32, 44, kolonna, svet, Gorit(mesto, 1, stolbec, gorod.ig.den));
        }

        for (int i = 0; i < 6; i++)
        {
            double kolonnaX = x - 142 + i * 54;
            if (i == 0 || i == 5)
            {
                Gradient(g, kolonna, ten, kolonnaX, verh + 46, 14, vysota - 58);
                Pryamougolnik(g, kolonna, kolonnaX - 4, verh + 46, 22, 6);
            }
        }

        // Широкая лестница
        for (int i = 0; i < 3; i++) Pryamougolnik(g, Temno(Color.FromArgb(182, 178, 170), svet + i * 0.05), x - 74 - i * 8, Stena - 16 + i * 5, 148 + i * 16, 6);

        double dverVerh = Stena - 92;
        Oval(g, kolonna, x - 34, dverVerh - 26, 68, 60);
        Pryamougolnik(g, Temno(Color.FromArgb(96, 60, 42), svet), x - 28, dverVerh + 2, 56, 76);
        Pryamougolnik(g, kolonna, x - 1, dverVerh + 2, 2, 76);
        Krug(g, Risovanie.yantar, x + 8, dverVerh + 42, 3);

        Vyveska(g, gorod, mesto, x, verh + 58, 200, svet);
        SnegNaKryshe(g, x, 340, verh + 34, zima, svet);
    }

    static void Cerkov(Graphics g, Gorod gorod, Mesto mesto, double x, double svet, double zima)
    {
        double vysota = 200, verh = Stena - vysota;
        Color stena = Temno(Color.FromArgb(240, 234, 224), svet);
        Color ten = Smes(stena, Color.Black, 0.14);
        Color zoloto = Temno(Color.FromArgb(232, 180, 70), svet + 0.15);

        // Барабан, купол-луковка и крест
        Pryamougolnik(g, stena, x - 44, verh - 62, 88, 66);
        Oval(g, zoloto, x - 54, verh - 136, 108, 96);
        Mnogougolnik(g, zoloto, x - 18, verh - 124, x, verh - 170, x + 18, verh - 124);
        Mnogougolnik(g, Color.FromArgb(70, 255, 255, 255), x - 30, verh - 74, x - 8, verh - 136, x + 2, verh - 136, x - 12, verh - 66);
        Pryamougolnik(g, zoloto, x - 2, verh - 208, 4, 44);
        Pryamougolnik(g, zoloto, x - 13, verh - 196, 26, 3.5);
        Pryamougolnik(g, zoloto, x - 9, verh - 184, 18, 3);
        Liniya(g, zoloto, 3, x - 9, verh - 171, x + 9, verh - 177);
        if (zima > 0) Oval(g, Temno(Color.White, svet + 0.25), x - 42, verh - 138, 84, 26);

        Pryamougolnik(g, stena, x - 150, verh, 300, vysota);
        Pryamougolnik(g, ten, x + 126, verh, 24, vysota);
        Pryamougolnik(g, Temno(Color.FromArgb(66, 86, 110), svet), x - 158, verh - 10, 316, 12);
        for (int i = 0; i < 7; i++) Pryamougolnik(g, ten, x - 145 + i * 48, verh + 8, 16, 7);

        // Витражи в арках
        Color[] steklo = { Color.Crimson, Color.DodgerBlue, Color.Gold, Color.MediumSeaGreen };
        foreach (double sdvig in new double[] { -106, 66 })
        {
            double oknoX = x + sdvig;
            Oval(g, ten, oknoX - 4, verh + 34, 48, 44);
            Pryamougolnik(g, ten, oknoX - 4, verh + 54, 48, 84);
            for (int i = 0; i < 8; i++) Pryamougolnik(g, Temno(steklo[i % 4], svet + 0.3), oknoX + (i % 2) * 20, verh + 62 + (int)(i / 2) * 18, 19, 17);
            if (svet < 0.5) Svechenie(g, Color.FromArgb(90, 255, 190, 120), oknoX + 20, verh + 90, 62);
        }

        Oval(g, ten, x - 42, Stena - 118, 84, 64);
        Pryamougolnik(g, ten, x - 42, Stena - 86, 84, 80);
        Oval(g, Temno(Color.FromArgb(104, 62, 40), svet), x - 34, Stena - 110, 68, 52);
        Pryamougolnik(g, Temno(Color.FromArgb(104, 62, 40), svet), x - 34, Stena - 84, 68, 78);
        Pryamougolnik(g, Temno(Color.FromArgb(70, 40, 28), svet), x - 1, Stena - 110, 2, 104);
        Krug(g, zoloto, x + 10, Stena - 44, 3);
        Pryamougolnik(g, Temno(Color.FromArgb(170, 166, 158), svet), x - 56, Stena - 7, 112, 7);

        Vyveska(g, gorod, mesto, x, verh + 20, 120, svet);
        SnegNaKryshe(g, x, 322, verh - 8, zima, svet);
    }

    static void Vokzal(Graphics g, Gorod gorod, Mesto mesto, double x, double svet, double zima)
    {
        double vysota = 212, verh = Stena - vysota;
        Color cvet = Temno(mesto.cvet, svet);
        Color kant = Temno(Color.FromArgb(240, 232, 214), svet);
        double chas = gorod.ig.chas;

        // Башня с настоящими часами
        Pryamougolnik(g, cvet, x - 50, verh - 96, 100, 110);
        Pryamougolnik(g, kant, x - 56, verh - 102, 112, 9);
        Mnogougolnik(g, Temno(Color.FromArgb(52, 86, 96), svet), x - 58, verh - 102, x, verh - 170, x + 58, verh - 102);
        Pryamougolnik(g, kant, x - 2, verh - 192, 4, 26);
        Krug(g, kant, x, verh - 52, 36);
        Krug(g, Color.FromArgb(250, 248, 236), x, verh - 52, 31);
        double ugolChasa = chas / 12 * Math.PI * 2 - Math.PI / 2;
        double ugolMinuty = chas % 1 * Math.PI * 2 - Math.PI / 2;
        Liniya(g, Color.Black, 3, x, verh - 52, x + Math.Cos(ugolChasa) * 17, verh - 52 + Math.Sin(ugolChasa) * 17);
        Liniya(g, Color.Black, 2, x, verh - 52, x + Math.Cos(ugolMinuty) * 26, verh - 52 + Math.Sin(ugolMinuty) * 26);

        Pryamougolnik(g, cvet, x - 176, verh, 352, vysota);
        Pryamougolnik(g, kant, x - 184, verh - 8, 368, 12);
        Pryamougolnik(g, kant, x - 176, Stena - 16, 352, 16);

        for (int i = 0; i < 5; i++)
        {
            double arkaX = x - 158 + i * 66;
            Oval(g, kant, arkaX - 3, verh + 56, 56, 52);
            Pryamougolnik(g, kant, arkaX - 3, verh + 82, 56, 112);
            Oval(g, Temno(Color.FromArgb(80, 110, 135), svet), arkaX, verh + 59, 50, 46);
            Okoshko(g, arkaX, verh + 84, 50, 110, kant, svet, i != 2);
        }

        Dver(g, x, 60, 104, Color.FromArgb(70, 56, 46), svet);
        Vyveska(g, gorod, mesto, x, verh + 12, 170, svet);
        SnegNaKryshe(g, x, 376, verh - 6, zima, svet);
    }

    static void Garazh(Graphics g, Gorod gorod, Mesto mesto, double x, double svet, double zima)
    {
        double vysota = 158, verh = Stena - vysota;
        Color cvet = Temno(mesto.cvet, svet);
        Color serye = Temno(Color.FromArgb(116, 122, 132), svet);

        Pryamougolnik(g, serye, x - 150, verh, 300, vysota);
        for (int i = 0; i < 16; i++) Pryamougolnik(g, Smes(serye, Color.Black, 0.2), x - 150 + i * 20, verh, 2, vysota);
        Pryamougolnik(g, cvet, x - 150, verh, 300, 18);
        Pryamougolnik(g, Smes(cvet, Color.Black, 0.4), x - 156, verh - 8, 312, 10);

        // Рулонные ворота и дверь
        Pryamougolnik(g, Temno(Color.FromArgb(60, 64, 72), svet), x - 136, Stena - 100, 170, 100);
        for (int i = 0; i < 12; i++) Pryamougolnik(g, Temno(Color.FromArgb(160, 164, 172), svet), x - 132, Stena - 96 + i * 8, 162, 5);
        Dver(g, x + 90, 46, 74, Color.FromArgb(60, 70, 84), svet);

        // Покрышки и бочка
        Krug(g, Color.FromArgb(24, 24, 28), x + 134, Stena - 14, 14);
        Krug(g, Color.FromArgb(60, 60, 66), x + 134, Stena - 14, 6);
        Krug(g, Color.FromArgb(24, 24, 28), x + 134, Stena - 34, 14);
        Krug(g, Color.FromArgb(60, 60, 66), x + 134, Stena - 34, 6);

        Vyveska(g, gorod, mesto, x, verh + 6, 200, svet);
        SnegNaKryshe(g, x, 322, verh - 6, zima, svet);
    }

    static void Zavod(Graphics g, Gorod gorod, Mesto mesto, double x, double svet, double zima)
    {
        double vysota = 186, verh = Stena - vysota;
        Color cvet = Temno(mesto.cvet, svet);
        Color truba = Temno(Color.FromArgb(150, 80, 60), svet);
        Color stal = Temno(Color.FromArgb(120, 128, 138), svet);

        // Высокие трубы с дымом
        foreach (double sdvig in new double[] { -130, -70, 118 })
        {
            Pryamougolnik(g, truba, x + sdvig, verh - 112, 32, 120);
            Pryamougolnik(g, Temno(Color.WhiteSmoke, svet), x + sdvig, verh - 92, 32, 8);
            Pryamougolnik(g, Temno(Color.WhiteSmoke, svet), x + sdvig, verh - 72, 32, 8);
            Pryamougolnik(g, Smes(truba, Color.Black, 0.4), x + sdvig - 3, verh - 118, 38, 7);
            Dym(g, gorod, x + sdvig + 16, verh - 122, svet);
        }

        // Пилообразная крыша цеха
        for (int i = 0; i < 6; i++)
        {
            double levo = x - 186 + i * 62;
            Mnogougolnik(g, stal, levo, verh, levo, verh - 36, levo + 62, verh);
            Mnogougolnik(g, Color.FromArgb(120, 255, 240, 200), levo + 3, verh - 3, levo + 3, verh - 28, levo + 26, verh - 3);
        }

        Pryamougolnik(g, cvet, x - 190, verh, 380, vysota);
        Pryamougolnik(g, Smes(cvet, Color.Black, 0.3), x - 190, verh, 380, 7);
        for (int i = 0; i < 7; i++)
        {
            Okoshko(g, x - 176 + i * 52, verh + 54, 42, 50, Temno(Color.FromArgb(70, 80, 90), svet), svet, i % 3 != 2);
        }

        // Большие ворота в жёлто-чёрную полосу
        Pryamougolnik(g, stal, x - 54, Stena - 76, 108, 76);
        for (int i = 0; i < 10; i++)
        {
            Color polosa = i % 2 == 0 ? Temno(Color.Gold, svet + 0.1) : Color.FromArgb(24, 24, 28);
            Mnogougolnik(g, polosa, x - 54 + i * 11, Stena - 84, x - 43 + i * 11, Stena - 84, x - 49 + i * 11, Stena - 76, x - 60 + i * 11, Stena - 76);
        }
        Dver(g, x, 48, 70, Color.FromArgb(70, 80, 92), svet);

        Vyveska(g, gorod, mesto, x, verh + 12, 170, svet);
        SnegNaKryshe(g, x, 396, verh - 2, zima, svet);
    }
}
