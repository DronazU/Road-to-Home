using System;
using System.Drawing;
using System.Windows.Forms;
using static Kisti;

// Экраны до начала игры: вход, регистрация и выбор игры
static class Ekrany
{
    public static string ekran = "vhod";
    public static string komanda = "";
    public static string polzovatel = "";
    public static double vremya = 0;

    static string pochta = "", parol = "", povtor = "", oshibka = "";
    static int pole = 0;
    static bool podtverzhdenie = false;
    static Igrok sohranenie = null;

    public static void Sbrosit()
    {
        ekran = "vhod";
        polzovatel = "";
        pochta = "";
        parol = "";
        povtor = "";
        oshibka = "";
        pole = 0;
    }

    public static void OtkrytMenu()
    {
        ekran = "menu";
        podtverzhdenie = false;
        sohranenie = Akkaunty.Zagruzit(polzovatel);
    }

    public static Igrok Sohranenie()
    {
        return sohranenie;
    }

    static int ChisloPoley()
    {
        return ekran == "registraciya" ? 3 : 2;
    }

    // Печать символов в выбранном поле
    public static void Bukva(char bukva)
    {
        if (ekran == "menu") return;

        string tekst = pole == 0 ? pochta : pole == 1 ? parol : povtor;
        if (bukva == '\b')
        {
            if (tekst.Length > 0) tekst = tekst.Substring(0, tekst.Length - 1);
        }
        else if (!char.IsControl(bukva) && tekst.Length < 40)
        {
            tekst += bukva;
            Zvuk.Igrat("klavisha");
        }

        if (pole == 0) pochta = tekst;
        else if (pole == 1) parol = tekst;
        else povtor = tekst;

        oshibka = "";
    }

    public static void Klavisha(Keys klavisha)
    {
        if (ekran == "menu")
        {
            if (klavisha == Keys.D1) Vybor(1);
            if (klavisha == Keys.D2) Vybor(2);
            if (klavisha == Keys.D3) Vybor(3);
            return;
        }

        if (klavisha == Keys.Tab || klavisha == Keys.Down) pole = (pole + 1) % ChisloPoley();
        if (klavisha == Keys.Up) pole = (pole + ChisloPoley() - 1) % ChisloPoley();

        if (klavisha == Keys.Enter)
        {
            if (pole < ChisloPoley() - 1) pole++;
            else Podtverdit();
        }
    }

    static void SmenitRezhim()
    {
        ekran = ekran == "vhod" ? "registraciya" : "vhod";
        parol = "";
        povtor = "";
        oshibka = "";
        pole = 0;
        Zvuk.Igrat("vybor");
    }

    static void Podtverdit()
    {
        oshibka = Proverki.Pochta(pochta);
        if (oshibka == "" && ekran == "registraciya") oshibka = Proverki.Parol(parol);
        if (oshibka == "" && ekran == "registraciya" && parol != povtor) oshibka = "Пароли не совпадают";

        if (oshibka == "" && ekran == "registraciya") oshibka = Akkaunty.Zaregistrirovat(pochta, parol);
        if (oshibka == "" && ekran == "vhod" && !Akkaunty.Voyti(pochta, parol)) oshibka = "Неверная почта или пароль";

        if (oshibka != "")
        {
            Zvuk.Igrat("nelzya");
            return;
        }

        Zvuk.Igrat("uspeh");
        polzovatel = pochta.ToLowerInvariant();
        OtkrytMenu();
    }

    static void Vybor(int nomer)
    {
        int punkt = sohranenie == null ? nomer + 1 : nomer;

        if (punkt == 1)
        {
            komanda = "prodolzhit";
        }
        else if (punkt == 2)
        {
            if (sohranenie != null && !podtverzhdenie)
            {
                podtverzhdenie = true;
                Zvuk.Igrat("nelzya");
                return;
            }
            komanda = "novaya";
        }
        else if (punkt == 3)
        {
            Zvuk.Igrat("zakryt");
            Sbrosit();
            return;
        }

        Zvuk.Igrat("uspeh");
    }

    static bool Popal(double mx, double my, double x, double y, double shirina, double vysota)
    {
        return mx >= x && mx <= x + shirina && my >= y && my <= y + vysota;
    }

    public static void Klik(double mx, double my)
    {
        if (ekran == "menu")
        {
            int chislo = sohranenie == null ? 2 : 3;
            for (int i = 0; i < chislo; i++)
            {
                if (Popal(mx, my, 310, 300 + i * 62, 380, 50)) Vybor(i + 1);
            }
            return;
        }

        bool registraciya = ekran == "registraciya";
        double verh = registraciya ? 246 : 262;

        for (int i = 0; i < ChisloPoley(); i++)
        {
            if (Popal(mx, my, 320, verh + i * 62 + 20, 360, 34)) pole = i;
        }

        double knopkaY = verh + ChisloPoley() * 62 + (registraciya ? 30 : 6);
        if (Popal(mx, my, 320, knopkaY, 360, 40)) Podtverdit();
        if (Popal(mx, my, 320, knopkaY + 46, 360, 26)) SmenitRezhim();
    }

    public static void Risovat(Graphics g)
    {
        Kartinki.Titul(g, vremya, false);
        if (ekran == "menu") Menu(g);
        else Forma(g);
    }

    static void Pole(Graphics g, int nomer, string podpis, string tekst, bool skryt, double y)
    {
        bool aktivno = pole == nomer;
        string vidno = skryt ? new string('*', tekst.Length) : tekst;
        bool mercanie = aktivno && (int)(vremya * 2) % 2 == 0;

        Tekst(g, podpis, Interfeys.melkiy, Color.Silver, 322, y, 360, false);
        Skruglennyy(g, aktivno ? Risovanie.yantar : Color.FromArgb(80, 84, 96), 320, y + 20, 360, 34, 7);
        Skruglennyy(g, Color.FromArgb(30, 33, 44), 322, y + 22, 356, 30, 6);
        Tekst(g, vidno + (mercanie ? "|" : ""), Interfeys.melkiy, Color.White, 332, y + 28, 340, false);
    }

    static void Knopka(Graphics g, string tekst, double x, double y, double shirina, double vysota, bool glavnaya)
    {
        Skruglennyy(g, glavnaya ? Risovanie.yantar : Color.FromArgb(58, 62, 76), x, y, shirina, vysota, 9);
        Tekst(g, tekst, Interfeys.sredniy, glavnaya ? Color.FromArgb(18, 21, 29) : Color.White, x, y + vysota / 2 - 12, shirina, true);
    }

    static void Forma(Graphics g)
    {
        bool registraciya = ekran == "registraciya";
        double verh = registraciya ? 246 : 262;
        double knopkaY = verh + ChisloPoley() * 62 + (registraciya ? 30 : 6);

        Skruglennyy(g, Color.FromArgb(238, 18, 21, 29), 290, verh - 46, 420, knopkaY + 84 - verh + 46, 16);
        Tekst(g, registraciya ? "Регистрация" : "Вход", Interfeys.sredniy, Color.White, 290, verh - 34, 420, true);

        Pole(g, 0, "Почта", pochta, false, verh);
        Pole(g, 1, "Пароль", parol, true, verh + 62);

        if (registraciya)
        {
            Pole(g, 2, "Повтори пароль", povtor, true, verh + 124);
            Trebovanie(g, "8 символов", Proverki.DlinaParolya(parol), 322);
            Trebovanie(g, "1 большая буква", Proverki.EstBolshaya(parol), 470);
        }

        Knopka(g, registraciya ? "Создать аккаунт" : "Войти", 320, knopkaY, 360, 40, true);
        string ssylka = registraciya ? "Уже есть аккаунт? Войти" : "Нет аккаунта? Регистрация";
        Tekst(g, ssylka, Interfeys.melkiy, Risovanie.yantar, 320, knopkaY + 50, 360, true);

        if (oshibka != "")
        {
            Skruglennyy(g, Color.FromArgb(240, 120, 30, 34), 210, 158, 580, 34, 8);
            Tekst(g, oshibka, Interfeys.melkiy, Color.White, 210, 166, 580, true);
        }
    }

    static void Trebovanie(Graphics g, string tekst, bool vypolneno, double x)
    {
        Krug(g, vypolneno ? Color.FromArgb(90, 200, 110) : Color.FromArgb(120, 124, 134), x + 6, verhTrebovaniy + 10, 6);
        Tekst(g, tekst, Interfeys.melkiy, vypolneno ? Color.White : Color.Silver, x + 18, verhTrebovaniy, 150, false);
    }

    const double verhTrebovaniy = 434;

    static void Menu(Graphics g)
    {
        Skruglennyy(g, Color.FromArgb(238, 18, 21, 29), 280, 232, 440, sohranenie == null ? 200 : 262, 16);
        Tekst(g, polzovatel, Interfeys.melkiy, Color.Silver, 280, 248, 440, true);
        Tekst(g, "Что будем делать?", Interfeys.sredniy, Color.White, 280, 268, 440, true);

        double y = 300;
        if (sohranenie != null)
        {
            Knopka(g, "1. Продолжить: день " + sohranenie.den, 310, y, 380, 50, true);
            y += 62;
            Knopka(g, podtverzhdenie ? "2. Точно заново? Прогресс удалится" : "2. Новая игра", 310, y, 380, 50, false);
            y += 62;
            Knopka(g, "3. Выйти из аккаунта", 310, y, 380, 50, false);
        }
        else
        {
            Knopka(g, "1. Новая игра", 310, y, 380, 50, true);
            y += 62;
            Knopka(g, "2. Выйти из аккаунта", 310, y, 380, 50, false);
        }
    }
}
