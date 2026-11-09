using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

// Окно игры: принимает нажатия клавиш и обновляет картинку
class Okno : Form
{
    Gorod gorod = null;
    Mesto otkrytoe = null;
    bool vlevo, vpravo, vverh, vniz, pauza;
    double shag = 0, vremyaDoSohraneniya = 15;
    int nomerShaga = 0;
    DateTime proshlyyKadr = DateTime.Now;
    System.Windows.Forms.Timer taymer = new System.Windows.Forms.Timer();

    public Okno()
    {
        Text = "Дорога домой";
        ClientSize = new Size((int)(Risovanie.Shirina * Risovanie.Masshtab), (int)(Risovanie.Vysota * Risovanie.Masshtab));
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        DoubleBuffered = true;
        KeyPreview = true;

        Zvuk.Zagruzit();
        taymer.Interval = 16;
        taymer.Tick += (otpravitel, sobytie) => Bezopasno(Kadr);
        taymer.Start();
    }

    // Любая ошибка попадает в журнал, а игра продолжает работать
    void Bezopasno(Action deystvie)
    {
        try
        {
            deystvie();
        }
        catch (Exception oshibka)
        {
            Zhurnal.Zapisat(oshibka);
        }
    }

    void Kadr()
    {
        double sekundy = (DateTime.Now - proshlyyKadr).TotalSeconds;
        proshlyyKadr = DateTime.Now;

        if (gorod == null)
        {
            Ekrany.vremya += sekundy;
            Zvuk.Obnovit(null, otkrytoe);
            ProveritKomandu();
            Invalidate();
            return;
        }

        bool stoim = gorod.ochered.Count > 0 || otkrytoe != null || pauza;
        gorod.Kadr(sekundy, stoim);
        Zvuk.Obnovit(gorod, otkrytoe);

        int poX = (vpravo ? 1 : 0) - (vlevo ? 1 : 0);
        int poY = (vniz ? 1 : 0) - (vverh ? 1 : 0);

        if (!stoim && (poX != 0 || poY != 0))
        {
            gorod.Idti(poX, poY, sekundy);
            shag += sekundy * 11;

            // Каждые пол-оборота ноги это один шаг
            int novyyShag = (int)(shag / Math.PI);
            if (novyyShag != nomerShaga)
            {
                nomerShaga = novyyShag;
                Zvuk.Shag(gorod.ZvukShaga());
            }
        }

        vremyaDoSohraneniya -= sekundy;
        if (vremyaDoSohraneniya <= 0) Sohranit();

        Invalidate();
    }

    void ProveritKomandu()
    {
        if (Ekrany.komanda == "novaya")
        {
            Akkaunty.Udalit(Ekrany.polzovatel);
            NachatIgru(null);
        }

        if (Ekrany.komanda == "prodolzhit") NachatIgru(Ekrany.Sohranenie());
        Ekrany.komanda = "";
    }

    void NachatIgru(Igrok sohranennyy)
    {
        gorod = new Gorod(sohranennyy);
        otkrytoe = null;
        pauza = false;
    }

    // Сохранение идёт, только пока история не закончилась
    void Sohranit()
    {
        vremyaDoSohraneniya = 15;
        if (gorod == null || gorod.konec || Ekrany.polzovatel == "") return;
        Akkaunty.Sohranit(Ekrany.polzovatel, gorod.DlyaSohraneniya());
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Bezopasno(() => Risovanie.Vse(e.Graphics, gorod, otkrytoe, shag, pauza));
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        if (gorod == null) Bezopasno(() => Ekrany.Bukva(e.KeyChar));
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (gorod == null) Bezopasno(() => Ekrany.Klik(e.X / Risovanie.Masshtab, e.Y / Risovanie.Masshtab));
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        Bezopasno(() => NazhataKlavisha(e.KeyCode));
    }

    void NazhataKlavisha(Keys klavisha)
    {
        if (klavisha == Keys.Left || klavisha == Keys.A) vlevo = true;
        if (klavisha == Keys.Right || klavisha == Keys.D) vpravo = true;
        if (klavisha == Keys.Up || klavisha == Keys.W) vverh = true;
        if (klavisha == Keys.Down || klavisha == Keys.S) vniz = true;

        if (gorod == null) Ekrany.Klavisha(klavisha);
        else if (klavisha == Keys.M) Zvuk.Pereklyuchit();
        else if (pauza) KlavishaNaPauze(klavisha);
        else if (gorod.ochered.Count > 0) KlavishaVRazgovore(klavisha);
        else if (otkrytoe != null) KlavishaVMenu(klavisha);
        else if (klavisha == Keys.E) Deystvovat();
        else if (klavisha == Keys.Escape) NazhatEscape();
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A) vlevo = false;
        if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D) vpravo = false;
        if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W) vverh = false;
        if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S) vniz = false;
    }

    // Если окно потеряло фокус, отпускаем клавиши, иначе герой пойдёт сам
    protected override void OnDeactivate(EventArgs e)
    {
        base.OnDeactivate(e);
        vlevo = false;
        vpravo = false;
        vverh = false;
        vniz = false;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        Bezopasno(Sohranit);
    }

    void NazhatEscape()
    {
        if (gorod.vnutri != null)
        {
            gorod.Vyyti();
            return;
        }

        Sohranit();
        pauza = true;
        Zvuk.Igrat("menu");
    }

    void KlavishaNaPauze(Keys klavisha)
    {
        if (klavisha == Keys.Escape)
        {
            pauza = false;
            Zvuk.Igrat("zakryt");
        }

        if (klavisha == Keys.Q)
        {
            Sohranit();
            gorod = null;
            pauza = false;
            Ekrany.OtkrytMenu();
        }
    }

    void KlavishaVRazgovore(Keys klavisha)
    {
        if (klavisha != Keys.Enter && klavisha != Keys.Space && klavisha != Keys.E) return;

        gorod.Dalshe();
        if (gorod.zakonchena)
        {
            Akkaunty.Udalit(Ekrany.polzovatel);
            gorod = null;
            otkrytoe = null;
            Ekrany.OtkrytMenu();
        }
    }

    void KlavishaVMenu(Keys klavisha)
    {
        if (klavisha == Keys.Escape)
        {
            otkrytoe = null;
            Zvuk.Igrat("zakryt");
            return;
        }

        int nomer = klavisha - Keys.D1;
        List<Deystvie> spisok = gorod.VidimyeDeystviya(otkrytoe);

        if (nomer >= 0 && nomer < spisok.Count)
        {
            Mesto mesto = otkrytoe;
            gorod.Vypolnit(mesto, spisok[nomer]);
            Sohranit();

            // Закрытие здания выводит на улицу, а автобус увозит от старой остановки
            bool zakrylos = !mesto.Otkryto(gorod.ig.chas);
            if (zakrylos && gorod.vnutri != null) gorod.Vyyti();
            if (zakrylos || gorod.ochered.Count > 0 || mesto.vid == "ostanovka") otkrytoe = null;
        }
    }

    // Что делает клавиша E зависит от того, где стоит герой
    void Deystvovat()
    {
        if (gorod.vnutri != null)
        {
            if (gorod.RyadomSVyhodom()) gorod.Vyyti();
            else if (gorod.RyadomSRabotnikom()) OtkrytMenu(gorod.vnutri);
            return;
        }

        Mesto mesto = gorod.MestoRyadom();
        if (mesto == null) return;

        if (!mesto.Otkryto(gorod.ig.chas))
        {
            Zvuk.Igrat("nelzya");
            gorod.Skazat(mesto.nazvanie + ": открывается в " + mesto.otkrytoS + ":00.");
        }
        else if (mesto.vid == "zdanie") gorod.Voyti(mesto);
        else OtkrytMenu(mesto);
    }

    void OtkrytMenu(Mesto mesto)
    {
        otkrytoe = mesto;
        Zvuk.Igrat("menu");
    }
}
