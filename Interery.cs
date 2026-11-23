using System;
using System.Drawing;
using static Kisti;

// Комнаты внутри зданий. У каждого здания свой интерьер
static class Interery
{
    const int Pol = 410;

    public static void Risovat(Graphics g, Gorod gorod, double shag)
    {
        Mesto mesto = gorod.vnutri;
        Igrok ig = gorod.ig;

        switch (mesto.komnata)
        {
            case "podval": Podval(g, ig.chas); break;
            case "stolovaya": Stolovaya(g, ig.chas); break;
            case "magazin": Magazin(g, ig.chas); break;
            case "rynok": Rynok(g, ig.chas); break;
            case "sekond": Sekond(g, ig.chas); break;
            case "priyomka": Priyomka(g, ig.chas); break;
            case "centr": Centr(g, ig.chas); break;
            case "cerkov": Cerkov(g, ig.chas, gorod.vremya); break;
            case "biblioteka": Biblioteka(g, ig.chas); break;
            case "banya": Banya(g, ig.chas, gorod.vremya); break;
            case "pochta": Pochta(g, ig.chas); break;
            case "pasportniy": Pasportniy(g, ig.chas); break;
            case "kafe": Kafe(g, ig.chas); break;
            case "apteka": Apteka(g, ig.chas); break;
            case "vokzal": Vokzal(g, ig.chas); break;
            case "arenda": Arenda(g, ig.chas); break;
            case "garazh": Garazh(g, ig.chas); break;
            default: Zavod(g, ig.chas, gorod.vremya); break;
        }

        if (mesto.vneshnost != null)
        {
            double dyhanie = Math.Sin(gorod.vremya * 1.6) * 1.5;
            Risovanie.Chelovek(g, 770, 434 + dyhanie, 1.34, false, 0, false, mesto.vneshnost);
            Prilavok(g, mesto.cvet);
        }

        double masshtab = 1.22 + (ig.yKomnaty - 445) / 85 * 0.26;
        Risovanie.viktor.ryukzak = true;
        Risovanie.Chelovek(g, ig.xKomnaty, ig.yKomnaty, masshtab, ig.smotritVpravo, shag, false, Risovanie.viktor);

        // Мягкая тень по углам комнаты
        Gradient(g, Color.FromArgb(120, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), 0, 0, Risovanie.Shirina, 120);
        Gradient(g, Color.FromArgb(0, 0, 0, 0), Color.FromArgb(110, 0, 0, 0), 0, 470, Risovanie.Shirina, 90);
    }

    // Общая основа комнаты: стены, пол и дверь наружу
    static void Komnata(Graphics g, Color stena, Color polCvet, bool plitka)
    {
        Gradient(g, Smes(stena, Color.Black, 0.28), stena, 0, 0, Risovanie.Shirina, Pol);
        Pryamougolnik(g, Smes(stena, Color.Black, 0.4), 0, 0, Risovanie.Shirina, 8);
        Pryamougolnik(g, Smes(stena, Color.Black, 0.45), 0, Pol - 14, Risovanie.Shirina, 14);

        Gradient(g, Smes(polCvet, Color.Black, 0.25), polCvet, 0, Pol, Risovanie.Shirina, 150);

        Color liniya = Smes(polCvet, Color.Black, 0.2);
        for (int i = 0; i < 6; i++) Pryamougolnik(g, liniya, 0, Pol + i * i * 4.2 + 8, Risovanie.Shirina, 1.6);
        for (int i = -10; i < 22; i++)
        {
            double verh = 500 + (i - 6) * 42;
            double niz = 500 + (i - 6) * 112;
            if (plitka || i % 2 == 0) Mnogougolnik(g, liniya, verh, Pol, verh + 1.5, Pol, niz + 2, 560, niz, 560);
        }

        // Дверь с надписью над ней
        Pryamougolnik(g, Color.FromArgb(50, 40, 34), 62, 218, 104, 192);
        Gradient(g, Color.FromArgb(120, 84, 58), Color.FromArgb(84, 56, 40), 70, 226, 88, 184);
        Pryamougolnik(g, Color.FromArgb(60, 42, 30), 82, 244, 64, 62);
        Pryamougolnik(g, Color.FromArgb(60, 42, 30), 82, 322, 64, 72);
        Krug(g, Risovanie.yantar, 148, 318, 4);
        Skruglennyy(g, Color.FromArgb(40, 150, 90), 72, 186, 84, 24, 5);
        Tekst(g, "ВЫХОД", Interfeys.melkiy, Color.White, 72, 189, 84, true);
    }

    static void Lampa(Graphics g, double x, Color cvet)
    {
        Pryamougolnik(g, Color.FromArgb(40, 40, 44), x - 1, 0, 2, 66);
        Mnogougolnik(g, Color.FromArgb(44, 46, 54), x - 12, 64, x + 12, 64, x + 34, 88, x - 34, 88);
        Oval(g, cvet, x - 24, 84, 48, 10);
        Svechenie(g, Alfa(cvet, 170), x, 100, 190);
        Mnogougolnik(g, Alfa(cvet, 22), x - 30, 90, x + 30, 90, x + 170, Pol + 80, x - 170, Pol + 80);
    }

    // Окно показывает настоящее время суток
    static void Okno(Graphics g, double x, double y, double shirina, double vysota, double chas)
    {
        double svet = Risovanie.Svet(chas);
        Color rama = Color.FromArgb(230, 226, 216);
        Pryamougolnik(g, rama, x - 7, y - 7, shirina + 14, vysota + 14);
        Gradient(g, Smes(Color.FromArgb(10, 14, 36), Color.FromArgb(120, 170, 215), svet), Smes(Color.FromArgb(36, 36, 74), Color.FromArgb(220, 232, 236), svet), x, y, shirina, vysota);

        Color doma = Smes(Color.FromArgb(14, 16, 32), Color.FromArgb(120, 134, 150), svet);
        for (int i = 0; i < 4; i++)
        {
            double vysotaDoma = vysota * 0.3 + i * 37 % 34;
            Pryamougolnik(g, doma, x + i * shirina / 4, y + vysota - vysotaDoma, shirina / 4 - 2, vysotaDoma);
        }

        Pryamougolnik(g, rama, x + shirina / 2 - 3, y, 6, vysota);
        Pryamougolnik(g, rama, x, y + vysota * 0.4, shirina, 6);
        Pryamougolnik(g, Color.FromArgb(205, 200, 190), x - 12, y + vysota + 6, shirina + 24, 8);
    }

    static void Polki(Graphics g, double x, double y, double shirina, double vysota, Color derevo, int ryadov, Color[] tovary)
    {
        Pryamougolnik(g, Smes(derevo, Color.Black, 0.45), x, y, shirina, vysota);
        Pryamougolnik(g, derevo, x - 6, y - 6, shirina + 12, 8);

        double shag = vysota / ryadov;
        for (int ryad = 0; ryad < ryadov; ryad++)
        {
            double niz = y + (ryad + 1) * shag;
            Pryamougolnik(g, derevo, x - 6, niz - 6, shirina + 12, 7);

            for (double p = x + 6; p < x + shirina - 16; p += 19)
            {
                int nomer = (int)(p + ryad * 11);
                Color cvet = tovary[nomer % tovary.Length];
                double h = shag - 18 - nomer % 3 * 5;
                Skruglennyy(g, cvet, p, niz - 6 - h, 15, h, 2.5);
                Pryamougolnik(g, Color.FromArgb(70, 255, 255, 255), p + 3, niz - 3 - h, 3, h - 6);
            }
        }
    }

    static void Plakat(Graphics g, double x, double y, double shirina, double vysota, string tekst, Color cvet)
    {
        Pryamougolnik(g, Color.FromArgb(50, 0, 0, 0), x + 3, y + 3, shirina, vysota);
        Pryamougolnik(g, Color.FromArgb(245, 240, 228), x, y, shirina, vysota);
        Pryamougolnik(g, cvet, x, y, shirina, 10);
        Tekst(g, tekst, Interfeys.melkiy, Color.FromArgb(50, 50, 56), x + 6, y + 16, shirina - 12, true);
    }

    static void Stol(Graphics g, double x, double y, Color cvet, bool s_edoy)
    {
        Oval(g, Color.FromArgb(70, 0, 0, 0), x - 76, y - 4, 152, 14);
        Pryamougolnik(g, Smes(cvet, Color.Black, 0.35), x - 60, y - 48, 7, 50);
        Pryamougolnik(g, Smes(cvet, Color.Black, 0.35), x + 53, y - 48, 7, 50);
        Skruglennyy(g, cvet, x - 74, y - 58, 148, 12, 3);
        if (!s_edoy) return;
        Oval(g, Color.WhiteSmoke, x - 24, y - 64, 48, 9);
        Oval(g, Color.Peru, x - 15, y - 65, 30, 6);
        Pryamougolnik(g, Color.SteelBlue, x + 36, y - 74, 12, 16);
    }

    static void Taburet(Graphics g, double x, double y, Color cvet)
    {
        Pryamougolnik(g, Smes(cvet, Color.Black, 0.3), x - 11, y - 30, 4, 30);
        Pryamougolnik(g, Smes(cvet, Color.Black, 0.3), x + 7, y - 30, 4, 30);
        Skruglennyy(g, cvet, x - 16, y - 36, 32, 8, 3);
    }

    static void Batareya(Graphics g, double x, double y)
    {
        for (int i = 0; i < 8; i++) Skruglennyy(g, Color.FromArgb(218, 216, 208), x + i * 13, y, 11, 48, 4);
        Pryamougolnik(g, Color.FromArgb(180, 178, 170), x, y + 8, 104, 3);
        Pryamougolnik(g, Color.FromArgb(180, 178, 170), x, y + 38, 104, 3);
    }

    static void Cvetok(Graphics g, double x, double y)
    {
        Mnogougolnik(g, Color.FromArgb(170, 96, 60), x - 18, y - 30, x + 18, y - 30, x + 12, y, x - 12, y);
        for (int i = -3; i <= 3; i++) Liniya(g, Color.FromArgb(50, 120, 70), 6, x, y - 30, x + i * 11, y - 60 - (9 - i * i) * 5);
    }

    static void Chasy(Graphics g, double x, double y, double chas)
    {
        Krug(g, Color.FromArgb(50, 50, 56), x, y, 27);
        Krug(g, Color.FromArgb(246, 244, 234), x, y, 23);
        double ugolChasa = chas / 12 * Math.PI * 2 - Math.PI / 2;
        double ugolMinuty = chas % 1 * Math.PI * 2 - Math.PI / 2;
        Liniya(g, Color.Black, 3, x, y, x + Math.Cos(ugolChasa) * 12, y + Math.Sin(ugolChasa) * 12);
        Liniya(g, Color.Black, 2, x, y, x + Math.Cos(ugolMinuty) * 19, y + Math.Sin(ugolMinuty) * 19);
    }

    // Стойка, за которой стоит работник
    static void Prilavok(Graphics g, Color cvet)
    {
        Color derevo = Smes(cvet, Color.FromArgb(120, 84, 58), 0.65);
        Oval(g, Color.FromArgb(80, 0, 0, 0), 620, 428, 320, 22);
        Gradient(g, derevo, Smes(derevo, Color.Black, 0.35), 640, 362, 280, 76);
        for (int i = 0; i < 5; i++) Pryamougolnik(g, Smes(derevo, Color.Black, 0.22), 650 + i * 58, 374, 44, 54);
        Skruglennyy(g, Smes(derevo, Color.White, 0.3), 630, 354, 300, 14, 4);
    }

    static Color[] yarkie = { Color.Tomato, Color.Goldenrod, Color.SteelBlue, Color.SeaGreen, Color.Orchid, Color.Coral };

    static void Podval(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(78, 76, 74), Color.FromArgb(58, 54, 52), false);

        // Трубы, потёки и маленькое окошко под потолком
        Skruglennyy(g, Color.FromArgb(90, 70, 58), 0, 54, 1000, 16, 7);
        Skruglennyy(g, Color.FromArgb(70, 76, 82), 0, 86, 1000, 10, 5);
        for (int i = 0; i < 6; i++) Pryamougolnik(g, Color.FromArgb(60, 40, 30, 24), 220 + i * 133, 70, 18 + i * 3, 120 + i * 37 % 90);
        Okno(g, 430, 130, 120, 64, chas);
        Svechenie(g, Color.FromArgb(100, 255, 190, 120), 790, 210, 200);
        Liniya(g, Color.FromArgb(30, 30, 30), 2, 790, 0, 790, 150);
        Krug(g, Color.FromArgb(255, 226, 150), 790, 156, 9);

        // Ящики, матрас и фотография у стены
        Pryamougolnik(g, Color.FromArgb(120, 92, 62), 320, 318, 84, 84);
        Pryamougolnik(g, Color.FromArgb(94, 70, 46), 320, 356, 84, 3);
        Pryamougolnik(g, Color.FromArgb(120, 92, 62), 338, 262, 64, 56);
        Skruglennyy(g, Color.FromArgb(120, 112, 100), 640, 410, 280, 34, 10);
        Skruglennyy(g, Color.FromArgb(90, 80, 74), 640, 402, 96, 22, 8);
        Skruglennyy(g, Color.FromArgb(150, 60, 50), 730, 396, 188, 32, 10);
        Pryamougolnik(g, Color.WhiteSmoke, 860, 258, 40, 50);
        Pryamougolnik(g, Color.FromArgb(120, 150, 170), 864, 262, 32, 34);
        Krug(g, Color.FromArgb(226, 190, 160), 880, 278, 8);
    }

    static void Stolovaya(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(214, 200, 168), Color.FromArgb(150, 120, 92), false);
        Okno(g, 220, 110, 130, 140, chas);
        Okno(g, 420, 110, 130, 140, chas);
        Lampa(g, 300, Color.FromArgb(255, 226, 170));
        Lampa(g, 560, Color.FromArgb(255, 226, 170));
        Plakat(g, 650, 120, 120, 120, "МЕНЮ\nсуп\nкаша\nчай", Color.IndianRed);
        Batareya(g, 240, 330);
        Stol(g, 310, 470, Color.FromArgb(190, 150, 110), true);
        Taburet(g, 216, 470, Color.FromArgb(150, 110, 80));
        Taburet(g, 404, 470, Color.FromArgb(150, 110, 80));

        // Котёл с паром на раздаче
        Skruglennyy(g, Color.Silver, 830, 280, 90, 76, 12);
        Pryamougolnik(g, Color.Gray, 824, 276, 102, 8);
    }

    static void Magazin(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(224, 214, 182), Color.FromArgb(180, 170, 150), true);
        Lampa(g, 340, Color.FromArgb(250, 245, 215));
        Lampa(g, 640, Color.FromArgb(250, 245, 215));
        Polki(g, 210, 130, 320, 262, Color.FromArgb(150, 110, 70), 4, yarkie);
        Polki(g, 640, 120, 300, 200, Color.FromArgb(150, 110, 70), 3, yarkie);
        Skruglennyy(g, Color.FromArgb(60, 64, 72), 540, 350, 70, 60, 5);
        Pryamougolnik(g, Color.FromArgb(120, 220, 170), 550, 358, 50, 18);
    }

    static void Rynok(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(130, 150, 170), Color.FromArgb(120, 116, 110), true);
        for (int i = 0; i < 12; i++)
        {
            Color polosa = i % 2 == 0 ? Color.SteelBlue : Color.WhiteSmoke;
            Mnogougolnik(g, polosa, 500, -60, 500, -60, i * 100 + 100, 120, i * 100, 120);
        }
        Lampa(g, 620, Color.FromArgb(255, 226, 170));

        // Ящики с овощами и мешки
        for (int i = 0; i < 4; i++)
        {
            double x = 220 + i * 92;
            Pryamougolnik(g, Color.FromArgb(150, 110, 70), x, 330, 84, 64);
            for (int t = 0; t < 7; t++) Krug(g, yarkie[(i + t) % 3], x + 10 + t * 11, 326 + t % 2 * 6, 9);
        }
        for (int i = 0; i < 3; i++) Skruglennyy(g, Color.FromArgb(190, 170, 130), 240 + i * 90, 400, 76, 72, 20);
    }

    static void Sekond(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(216, 186, 206), Color.FromArgb(150, 122, 106), false);
        Lampa(g, 400, Color.FromArgb(255, 230, 190));
        Lampa(g, 660, Color.FromArgb(255, 230, 190));

        // Вешалка с одеждой
        Pryamougolnik(g, Color.Gray, 210, 180, 400, 6);
        Pryamougolnik(g, Color.Gray, 214, 180, 5, 228);
        Pryamougolnik(g, Color.Gray, 600, 180, 5, 228);
        for (int i = 0; i < 9; i++)
        {
            double x = 232 + i * 42;
            Liniya(g, Color.Gray, 2, x + 14, 182, x + 14, 192);
            Mnogougolnik(g, yarkie[i % 6], x, 196, x + 28, 196, x + 36, 310, x - 8, 310);
        }

        // Большое зеркало
        Skruglennyy(g, Color.FromArgb(120, 84, 58), 660, 120, 110, 230, 10);
        Gradient(g, Color.FromArgb(210, 230, 240), Color.FromArgb(150, 180, 200), 670, 130, 90, 210);
        Mnogougolnik(g, Color.FromArgb(90, 255, 255, 255), 680, 340, 720, 130, 744, 130, 704, 340);
    }

    static void Priyomka(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(120, 146, 140), Color.FromArgb(96, 96, 96), true);
        Lampa(g, 500, Color.FromArgb(235, 245, 225));
        Plakat(g, 650, 120, 150, 110, "ПРИЁМ\nбутылка 1 р.\nбанка 1 р.\nметалл 3 р.", Color.Teal);

        // Ящики с бутылками друг на друге
        for (int ryad = 0; ryad < 3; ryad++)
        {
            for (int i = 0; i < 4 - ryad; i++)
            {
                double x = 230 + i * 86 + ryad * 42;
                double y = 346 - ryad * 66;
                Pryamougolnik(g, Color.FromArgb(190, 150, 40), x, y, 80, 62);
                for (int b = 0; b < 6; b++) Skruglennyy(g, Color.FromArgb(70, 140, 100), x + 5 + b * 12, y - 18, 8, 26, 3);
                Pryamougolnik(g, Color.FromArgb(150, 116, 30), x, y + 26, 80, 5);
            }
        }
    }

    static void Centr(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(206, 196, 224), Color.FromArgb(160, 140, 120), false);
        Okno(g, 250, 106, 170, 150, chas);
        Lampa(g, 520, Color.FromArgb(255, 232, 190));
        Plakat(g, 500, 130, 130, 120, "Вы не одни\n\nПомощь\nбесплатна", Color.MediumPurple);
        Plakat(g, 660, 110, 110, 86, "Юрист\nпо средам", Color.SteelBlue);
        Batareya(g, 270, 330);
        Cvetok(g, 470, 410);

        // Диван для посетителей и термос на стойке
        Skruglennyy(g, Color.FromArgb(120, 100, 160), 230, 410, 210, 62, 14);
        Skruglennyy(g, Color.FromArgb(140, 118, 180), 230, 376, 210, 50, 14);
        Skruglennyy(g, Color.Silver, 860, 300, 28, 54, 6);
    }

    static void Cerkov(Graphics g, double chas, double vremya)
    {
        Komnata(g, Color.FromArgb(150, 120, 90), Color.FromArgb(120, 96, 76), true);

        Color[] steklo = { Color.Crimson, Color.DodgerBlue, Color.Gold, Color.MediumSeaGreen };
        foreach (double x in new double[] { 240, 480 })
        {
            Oval(g, Color.FromArgb(60, 46, 34), x - 6, 76, 132, 110);
            Pryamougolnik(g, Color.FromArgb(60, 46, 34), x - 6, 130, 132, 150);
            for (int i = 0; i < 24; i++) Pryamougolnik(g, steklo[(i * 5 + (int)(i / 4)) % 4], x + (i % 4) * 30, 124 + (int)(i / 4) * 25, 29, 24);
            Svechenie(g, Color.FromArgb(70, 255, 210, 150), x + 60, 200, 150);
        }

        // Подсвечник с живыми огоньками
        Pryamougolnik(g, Color.FromArgb(190, 150, 70), 640, 300, 110, 8);
        Pryamougolnik(g, Color.FromArgb(190, 150, 70), 690, 306, 8, 104);
        for (int i = 0; i < 6; i++)
        {
            double x = 648 + i * 19;
            Pryamougolnik(g, Color.FromArgb(240, 228, 190), x - 3, 270, 6, 30);
            Svechenie(g, Color.FromArgb(160, 255, 190, 100), x, 262, 24);
            Oval(g, Color.FromArgb(255, 226, 130), x - 3, 257 + Math.Sin(vremya * 7 + i), 6, 13);
        }

        // Скамьи
        for (int i = 0; i < 2; i++)
        {
            double y = 448 + i * 54;
            Pryamougolnik(g, Color.FromArgb(90, 60, 40), 240, y - 40, 300, 12);
            Pryamougolnik(g, Color.FromArgb(110, 74, 48), 240, y - 16, 300, 10);
            Pryamougolnik(g, Color.FromArgb(70, 46, 30), 250, y - 40, 9, 40);
            Pryamougolnik(g, Color.FromArgb(70, 46, 30), 520, y - 40, 9, 40);
        }
    }

    static void Biblioteka(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(150, 118, 84), Color.FromArgb(124, 88, 62), false);
        Color[] knigi = { Color.Maroon, Color.DarkSlateBlue, Color.DarkOliveGreen, Color.Peru, Color.Teal, Color.SaddleBrown };
        Polki(g, 210, 60, 280, 336, Color.FromArgb(92, 60, 40), 6, knigi);
        Polki(g, 520, 60, 200, 336, Color.FromArgb(92, 60, 40), 6, knigi);
        Polki(g, 750, 60, 210, 230, Color.FromArgb(92, 60, 40), 4, knigi);
        Lampa(g, 620, Color.FromArgb(255, 226, 170));

        // Стол с зелёной лампой
        Stol(g, 340, 470, Color.FromArgb(120, 80, 54), false);
        Pryamougolnik(g, Color.FromArgb(190, 150, 70), 338, 386, 4, 30);
        Skruglennyy(g, Color.FromArgb(40, 110, 80), 312, 374, 56, 16, 7);
        Svechenie(g, Color.FromArgb(110, 255, 226, 150), 340, 400, 90);
        Pryamougolnik(g, Color.WhiteSmoke, 270, 408, 36, 5);
    }

    static void Banya(Graphics g, double chas, double vremya)
    {
        Komnata(g, Color.FromArgb(166, 124, 82), Color.FromArgb(120, 130, 136), true);

        // Деревянные стены из брёвен
        for (int i = 0; i < 9; i++) Skruglennyy(g, Color.FromArgb(70, 60, 30, 10), 180, 40 + i * 42, 820, 6, 3);

        Lampa(g, 500, Color.FromArgb(255, 214, 150));
        Pryamougolnik(g, Color.FromArgb(120, 84, 54), 230, 354, 360, 12);
        Pryamougolnik(g, Color.FromArgb(90, 62, 40), 250, 366, 12, 44);
        Pryamougolnik(g, Color.FromArgb(90, 62, 40), 560, 366, 12, 44);

        // Кадка, веник и пар
        Skruglennyy(g, Color.FromArgb(120, 84, 54), 610, 340, 64, 70, 8);
        Pryamougolnik(g, Color.Gray, 610, 352, 64, 5);
        Pryamougolnik(g, Color.Gray, 610, 388, 64, 5);
        Liniya(g, Color.FromArgb(90, 62, 40), 4, 300, 180, 300, 240);
        for (int i = -3; i <= 3; i++) Liniya(g, Color.FromArgb(70, 120, 70), 5, 300, 236, 300 + i * 9, 300);

        for (int i = 0; i < 12; i++)
        {
            double faza = (vremya * 0.1 + i / 12.0) % 1;
            Krug(g, Alfa(Color.White, (1 - faza) * 62), 200 + i * 62 + Math.Sin(i) * 30, 410 - faza * 330, 34 + faza * 44);
        }
    }

    static void Pochta(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(180, 200, 228), Color.FromArgb(130, 136, 150), true);
        Okno(g, 240, 110, 150, 150, chas);
        Lampa(g, 520, Color.FromArgb(250, 240, 210));
        Plakat(g, 650, 110, 140, 96, "Письма\nи посылки", Color.RoyalBlue);

        // Почтовые ячейки и ящик
        for (int ryad = 0; ryad < 5; ryad++)
        {
            for (int i = 0; i < 4; i++)
            {
                Pryamougolnik(g, Color.FromArgb(120, 84, 58), 430 + i * 44, 180 + ryad * 42, 40, 38);
                Pryamougolnik(g, Color.FromArgb(60, 42, 30), 434 + i * 44, 184 + ryad * 42, 32, 30);
                if ((i + ryad) % 2 == 0) Pryamougolnik(g, Color.WhiteSmoke, 438 + i * 44, 200 + ryad * 42, 24, 12);
            }
        }
        Skruglennyy(g, Color.RoyalBlue, 250, 330, 90, 90, 12);
        Pryamougolnik(g, Color.FromArgb(20, 30, 70), 266, 356, 58, 7);
    }

    static void Pasportniy(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(196, 202, 206), Color.FromArgb(132, 118, 104), true);
        Okno(g, 230, 110, 150, 130, chas);
        Lampa(g, 500, Color.FromArgb(240, 245, 245));
        Chasy(g, 450, 150, chas);
        Plakat(g, 530, 120, 120, 110, "Очередь\nпо талонам", Color.SlateGray);
        Plakat(g, 680, 110, 110, 96, "Окно\nприёма", Color.SlateGray);

        // Ряд стульев очереди и стенд с образцами
        for (int i = 0; i < 4; i++)
        {
            double x = 240 + i * 76;
            Skruglennyy(g, Color.FromArgb(70, 90, 110), x, 340, 56, 44, 8);
            Skruglennyy(g, Color.FromArgb(90, 110, 130), x - 4, 382, 64, 12, 5);
            Pryamougolnik(g, Color.FromArgb(50, 50, 56), x + 6, 394, 4, 22);
            Pryamougolnik(g, Color.FromArgb(50, 50, 56), x + 46, 394, 4, 22);
        }
    }

    static void Kafe(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(128, 72, 60), Color.FromArgb(110, 74, 54), false);
        Okno(g, 230, 110, 170, 150, chas);
        Lampa(g, 320, Color.FromArgb(255, 200, 130));
        Lampa(g, 520, Color.FromArgb(255, 200, 130));
        Lampa(g, 760, Color.FromArgb(255, 200, 130));
        Plakat(g, 440, 120, 120, 120, "Сегодня\nборщ\nкотлеты\nкомпот", Color.Coral);

        // Кофемашина и полка за стойкой
        Polki(g, 680, 120, 240, 110, Color.FromArgb(92, 60, 40), 2, yarkie);
        Skruglennyy(g, Color.Silver, 840, 282, 70, 74, 8);
        Stol(g, 320, 470, Color.FromArgb(170, 120, 84), true);
        Stol(g, 510, 520, Color.FromArgb(170, 120, 84), true);
        Taburet(g, 226, 470, Color.Maroon);
        Taburet(g, 414, 470, Color.Maroon);
    }

    static void Apteka(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(220, 236, 228), Color.FromArgb(180, 198, 192), true);
        Lampa(g, 400, Color.FromArgb(235, 250, 245));
        Lampa(g, 700, Color.FromArgb(235, 250, 245));
        Color[] lekarstva = { Color.WhiteSmoke, Color.LightBlue, Color.MediumSeaGreen, Color.Khaki };
        Polki(g, 220, 120, 340, 276, Color.WhiteSmoke, 4, lekarstva);
        Polki(g, 640, 110, 300, 210, Color.WhiteSmoke, 3, lekarstva);

        // Зелёный крест
        Pryamougolnik(g, Color.MediumSeaGreen, 582, 118, 42, 14);
        Pryamougolnik(g, Color.MediumSeaGreen, 596, 104, 14, 42);
    }

    static void Vokzal(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(200, 176, 140), Color.FromArgb(150, 136, 120), true);

        foreach (double x in new double[] { 250, 410 })
        {
            Oval(g, Color.FromArgb(230, 222, 200), x - 8, 60, 136, 110);
            Pryamougolnik(g, Color.FromArgb(230, 222, 200), x - 8, 116, 136, 190);
            Okno(g, x, 120, 120, 170, chas);
        }

        // Табло с рейсами
        Pryamougolnik(g, Color.FromArgb(20, 24, 30), 570, 90, 360, 130);
        Tekst(g, "Брест    12:40    опаздывает", Interfeys.melkiy, Color.Gold, 584, 104, 340, false);
        Tekst(g, "Гомель   14:15    по расписанию", Interfeys.melkiy, Color.Gold, 584, 132, 340, false);
        Tekst(g, "Витебск  18:05    по расписанию", Interfeys.melkiy, Color.Gold, 584, 160, 340, false);
        Chasy(g, 540, 250, chas);
        Lampa(g, 490, Color.FromArgb(255, 232, 190));

        // Лавка для ожидания и чемодан
        Skruglennyy(g, Color.FromArgb(120, 84, 58), 230, 400, 320, 14, 4);
        Skruglennyy(g, Color.FromArgb(120, 84, 58), 230, 356, 320, 12, 4);
        Pryamougolnik(g, Color.FromArgb(60, 42, 30), 248, 368, 9, 80);
        Pryamougolnik(g, Color.FromArgb(60, 42, 30), 524, 368, 9, 80);
        Skruglennyy(g, Color.FromArgb(110, 60, 50), 570, 420, 62, 42, 6);
    }

    static void Arenda(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(198, 214, 196), Color.FromArgb(136, 110, 88), false);
        Okno(g, 240, 110, 150, 150, chas);
        Lampa(g, 520, Color.FromArgb(255, 236, 200));
        Plakat(g, 430, 110, 96, 84, "Комната\n300 р.", Color.DarkSeaGreen);
        Plakat(g, 540, 110, 96, 84, "Студия\n520 р.", Color.DarkSeaGreen);
        Plakat(g, 650, 110, 96, 84, "Дом\n900 р.", Color.DarkSeaGreen);

        // Доска с ключами и кресло
        Pryamougolnik(g, Color.FromArgb(120, 84, 58), 430, 216, 220, 90);
        for (int i = 0; i < 10; i++)
        {
            Krug(g, Color.Gold, 448 + i * 21, 240, 6);
            Pryamougolnik(g, Color.Gold, 446 + i * 21, 246, 4, 24);
        }
        Cvetok(g, 500, 410);
        Skruglennyy(g, Color.FromArgb(70, 110, 90), 240, 370, 110, 96, 20);
    }

    static void Garazh(Graphics g, double chas)
    {
        Komnata(g, Color.FromArgb(120, 124, 132), Color.FromArgb(82, 84, 90), true);
        Lampa(g, 470, Color.FromArgb(240, 245, 250));

        // Инструменты на стене
        Pryamougolnik(g, Color.FromArgb(60, 64, 72), 620, 110, 300, 130);
        for (int i = 0; i < 9; i++) Liniya(g, Color.Silver, 5, 640 + i * 32, 124, 640 + i * 32, 186 + i % 3 * 16);

        // Машина с открытым капотом
        Oval(g, Color.FromArgb(100, 0, 0, 0), 190, 462, 400, 26);
        Skruglennyy(g, Color.FromArgb(176, 60, 50), 200, 380, 370, 76, 18);
        Mnogougolnik(g, Color.FromArgb(176, 60, 50), 240, 384, 290, 318, 440, 318, 500, 384);
        Mnogougolnik(g, Color.FromArgb(150, 190, 215), 262, 382, 300, 330, 366, 330, 366, 382);
        Mnogougolnik(g, Color.FromArgb(150, 190, 215), 376, 382, 376, 330, 432, 330, 474, 382);
        Mnogougolnik(g, Color.FromArgb(140, 50, 42), 480, 380, 570, 300, 586, 312, 520, 382);
        Krug(g, Color.FromArgb(20, 20, 24), 280, 456, 28);
        Krug(g, Color.Silver, 280, 456, 11);
        Krug(g, Color.FromArgb(20, 20, 24), 500, 456, 28);
        Krug(g, Color.Silver, 500, 456, 11);
    }

    static void Zavod(Graphics g, double chas, double vremya)
    {
        Komnata(g, Color.FromArgb(120, 128, 136), Color.FromArgb(92, 92, 96), true);
        Lampa(g, 320, Color.FromArgb(235, 240, 250));
        Lampa(g, 640, Color.FromArgb(235, 240, 250));

        // Балки цеха
        for (int i = 0; i < 6; i++) Pryamougolnik(g, Color.FromArgb(70, 76, 84), 180 + i * 150, 0, 14, 400);
        Pryamougolnik(g, Color.FromArgb(70, 76, 84), 180, 70, 820, 12);

        // Токарный станок с искрами
        Pryamougolnik(g, Color.FromArgb(62, 96, 110), 220, 300, 340, 34);
        Skruglennyy(g, Color.FromArgb(70, 110, 124), 220, 234, 98, 70, 8);
        Skruglennyy(g, Color.FromArgb(70, 110, 124), 470, 262, 74, 42, 8);
        Pryamougolnik(g, Color.Silver, 318, 262, 152, 12);
        Pryamougolnik(g, Color.FromArgb(50, 76, 88), 240, 334, 40, 76);
        Pryamougolnik(g, Color.FromArgb(50, 76, 88), 500, 334, 40, 76);

        Svechenie(g, Color.FromArgb(180, 255, 180, 80), 400, 268, 54);
        for (int i = 0; i < 14; i++)
        {
            double faza = (vremya * 1.4 + i / 14.0) % 1;
            double x = 400 + (i - 7) * 7 * faza;
            double y = 268 + faza * faza * 110 - Math.Sin(faza * 3) * 22;
            Krug(g, Alfa(Color.Gold, (1 - faza) * 255), x, y, 2);
        }

        Pryamougolnik(g, Color.FromArgb(62, 96, 110), 610, 170, 300, 26);
        Tekst(g, "БЕЗОПАСНОСТЬ ПРЕЖДЕ ВСЕГО", Interfeys.melkiy, Color.Gold, 610, 174, 300, true);
    }
}
