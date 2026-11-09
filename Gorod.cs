using System;
using System.Collections.Generic;
using System.Drawing;

// Правила игры: движение, время, находки, машины и концовки
class Gorod
{
    public const int Dlina = 8000;
    public const int Verh = 382, Niz = 535;
    public const int KolichestvoMashin = 5;

    public Igrok ig = new Igrok();
    public List<Mesto> mesta = new List<Mesto>();
    public List<Nahodka> nahodki = new List<Nahodka>();
    public Mesto vnutri = null;

    // Очередь реплик: пока она не пуста, игра стоит на паузе
    public List<Replika> ochered = new List<Replika>();
    public double vremyaRepliki = 0;

    public string soobshenie = "";
    public string vsplyvashka = "";
    public string banner = "";
    public double vremyaSoobsheniya = 0, vremyaVsplyvashki = 0, vremyaBannera = 0, vremyaZashchity = 0, vremya = 0;
    public bool konec = false, zakonchena = false;

    public Random sluchay = new Random();

    // Если передан сохранённый герой, игра продолжается с его состояния
    public Gorod(Igrok sohranennyy)
    {
        if (sohranennyy != null) ig = sohranennyy;

        Karta.Sozdat(this);
        Razbrosat();

        if (sohranennyy != null)
        {
            VosstanovitSdelannoe();
            if (ig.etap == 0) Voyti(mesta[0]);
            Skazat("С возвращением, Виктор. Игра продолжается.");
            return;
        }

        Voyti(mesta[0]);
        Rasskaz("zavod", "Виктору сорок семь. Двадцать лет он стоял у токарного станка и на слух понимал, когда деталь идёт неправильно.");
        Rasskaz("kvartira", "Потом завод закрылся. Появились долги, и квартиру пришлось продать.");
        Rasskaz("vokzal", "На вокзале украли сумку с паспортом. Без документов нет работы. Без работы нет жилья.");
        Rasskaz("podval", "Второй месяц его дом - подвал на краю города. В кармане осталась только фотография дочери Кати.");
        Rasskaz("sneg", "Скоро зима. Ещё одну зиму на улице Виктор может не пережить. Пора что-то менять.");
        Razgovor("Виктор", "Хватит лежать. Сначала выйду на улицу и поищу, что можно сдать. А там посмотрим.");
    }

    // Готовит героя к записи в файл
    public Igrok DlyaSohraneniya()
    {
        ig.sdelanoSegodnya.Clear();
        int nomer = 0;

        foreach (Mesto mesto in mesta)
        {
            foreach (Deystvie deystvie in mesto.deystviya)
            {
                if (deystvie.denVypolneniya == ig.den) ig.sdelanoSegodnya.Add(nomer);
                nomer++;
            }
        }
        return ig;
    }

    void VosstanovitSdelannoe()
    {
        int nomer = 0;

        foreach (Mesto mesto in mesta)
        {
            foreach (Deystvie deystvie in mesto.deystviya)
            {
                if (ig.sdelanoSegodnya.Contains(nomer)) deystvie.denVypolneniya = ig.den;
                nomer++;
            }
        }
    }

    public bool Dozhd()
    {
        return ig.den < 6 && ig.den % 3 == 2;
    }

    public double Zima()
    {
        return Math.Clamp((ig.den - 5) / 8.0, 0, 1);
    }

    // Звук шага зависит от того, где идёт герой
    public string ZvukShaga()
    {
        if (vnutri != null) return "shag_pol";
        if (Zima() > 0.25) return "shag_sneg";
        if (Dozhd()) return "shag_luzha";
        return "shag_ulica";
    }

    public Mesto Novoe(string nazvanie, int x, int otkrytoS, int otkrytoDo, bool vTeple, Color cvet, string vid)
    {
        Mesto mesto = new Mesto();
        mesto.nazvanie = nazvanie;
        mesto.x = x;
        mesto.y = vid == "zdanie" ? Verh : 528;
        mesto.otkrytoS = otkrytoS;
        mesto.otkrytoDo = otkrytoDo;
        mesto.vTeple = vTeple;
        mesto.cvet = cvet;
        mesto.vid = vid;
        mesta.Add(mesto);
        return mesto;
    }

    // Каждое утро на улицах появляются новые находки
    public void Razbrosat()
    {
        nahodki.Clear();

        for (int i = 0; i < 12; i++)
        {
            Nahodka nahodka = new Nahodka();
            int kubik = sluchay.Next(100);

            nahodka.x = sluchay.Next(60, Dlina - 60);
            nahodka.y = sluchay.Next(2) == 0 ? sluchay.Next(390, 420) : sluchay.Next(510, 532);

            if (kubik < 55) nahodka.vid = "butylka";
            else if (kubik < 75) nahodka.vid = "banka";
            else if (kubik < 85) nahodka.vid = "moneta";
            else if (kubik < 95) nahodka.vid = "metall";
            else nahodka.vid = "yabloko";

            nahodki.Add(nahodka);
        }
    }

    void Sobrat()
    {
        for (int i = nahodki.Count - 1; i >= 0; i--)
        {
            Nahodka nahodka = nahodki[i];
            if (Math.Abs(nahodka.x - ig.x) > 34 || Math.Abs(nahodka.y - ig.y) > 26) continue;

            nahodki.RemoveAt(i);
            Zvuk.Igrat(nahodka.vid);

            if (nahodka.vid == "butylka")
            {
                ig.butylki++;
                Vsplyt("+1 бутылка");
            }
            else if (nahodka.vid == "banka")
            {
                ig.banki++;
                Vsplyt("+1 банка");
            }
            else if (nahodka.vid == "metall")
            {
                ig.metall++;
                Vsplyt("+1 металл");
            }
            else if (nahodka.vid == "moneta")
            {
                int summa = sluchay.Next(1, 4);
                ig.dengi += summa;
                Vsplyt("+" + summa + " р.");
            }
            else
            {
                ig.eda += 12;
                ig.Ogranichit();
                Vsplyt("+ яблоко");
            }

            ProveritZadanie();
        }
    }

    public int CenaVtorsyrya()
    {
        return ig.butylki + ig.banki + ig.metall * 3;
    }

    public void SdatVtorsyre()
    {
        int summa = CenaVtorsyrya();
        ig.dengi += summa;
        ig.butylki = 0;
        ig.banki = 0;
        ig.metall = 0;
        Zvuk.Igrat("dengi");
        Skazat("Получено рублей: " + summa);
        if (ig.etap == 2) Etap(3);
    }

    public void PoiskatVMusorke()
    {
        int naydeno = sluchay.Next(1, 5);
        ig.butylki += naydeno;

        if (!ig.kurtka && sluchay.Next(100) < 8)
        {
            ig.kurtka = true;
            Skazat("Кто-то выбросил тёплую куртку! А ещё бутылок: " + naydeno);
        }
        else
        {
            Skazat("Найдено бутылок: " + naydeno);
        }

        ProveritZadanie();
    }

    public void Usnut(double dobavitSil, double dobavitTepla, double dobavitNadezhdy)
    {
        ig.Spat(dobavitSil, dobavitTepla, dobavitNadezhdy);
        Razbrosat();
        Zvuk.Igrat("son");
    }

    public void Proehat(Mesto ostanovka)
    {
        ig.dengi -= 1;
        ig.x = ostanovka.x;
        ig.y = 505;
        Skazat("Автобус довёз до остановки \"" + ostanovka.nazvanie + "\".");
    }

    public void Rasskaz(string kartinka, string tekst)
    {
        Replika replika = new Replika();
        replika.kto = "Рассказчик";
        replika.tekst = tekst;
        replika.kartinka = kartinka;
        Dobavit(replika);
    }

    // Принимает пары: кто говорит и что говорит
    public void Razgovor(params string[] chasti)
    {
        for (int i = 0; i + 1 < chasti.Length; i += 2)
        {
            Replika replika = new Replika();
            replika.kto = chasti[i];
            replika.tekst = chasti[i + 1];
            replika.kartinka = "";
            Dobavit(replika);
        }
    }

    void Dobavit(Replika replika)
    {
        ochered.Add(replika);
        if (ochered.Count == 1) Nachat();
    }

    void Nachat()
    {
        vremyaRepliki = 0;
        if (ochered[0].tekst != "") Zvuk.Igrat("replika");
    }

    public void Dalshe()
    {
        if (ochered.Count == 0) return;

        // Первое нажатие сразу показывает весь текст, второе идёт дальше
        int vidnoBukv = (int)(vremyaRepliki * 36);
        if (vidnoBukv < ochered[0].tekst.Length)
        {
            vremyaRepliki = 1000;
            return;
        }

        ochered.RemoveAt(0);
        if (ochered.Count > 0) Nachat();

        if (ochered.Count == 0 && konec) zakonchena = true;
    }

    public void Etap(int nomer)
    {
        ig.etap = nomer;
        Zvuk.Igrat("zadanie");
        banner = Tsel();
        vremyaBannera = 5;
    }

    void ProveritZadanie()
    {
        if (ig.etap == 1 && ig.butylki + ig.banki + ig.metall >= 3)
        {
            Etap(2);
            Razgovor("Виктор", "Для начала хватит. Сдам всё в приёмке, она в конце нашего района.");
        }
    }

    public void PokazatPobedu()
    {
        string tekst = "Ключ повернулся в замке. Маленькая комната, старый диван, горячая вода из крана. ";

        if (ig.otvetKati) tekst += "В воскресенье приехала Катя. Они пили чай и почти не говорили. Этого было достаточно. ";
        else tekst += "На столе лежал чистый лист бумаги. Завтра Виктор напишет Кате. ";

        if (ig.mihalychSpasen) tekst += "Михалыч теперь живёт при центре и помогает Анне на кухне. ";
        else tekst += "Вечером Виктор вспомнил Михалыча и решил завтра сходить к скамейке. ";

        Zakonchit("dom", tekst + "Виктор вернулся домой. Дней в пути: " + ig.den + ".");
    }

    void Zakonchit(string kartinka, string tekst)
    {
        konec = true;
        ochered.Clear();
        Zvuk.Igrat(kartinka == "dom" ? "pobeda" : "porazhenie");
        Rasskaz(kartinka, tekst);
    }

    public void Skazat(string tekst)
    {
        soobshenie = tekst;
        vremyaSoobsheniya = 6;
    }

    void Vsplyt(string tekst)
    {
        vsplyvashka = tekst;
        vremyaVsplyvashki = 1.5;
    }

    public string Tsel()
    {
        if (ig.etap == 0) return "Выйти из подвала на улицу";
        if (ig.etap == 1) return "Собрать 3 бутылки или банки: на тротуарах и в мусорках";
        if (ig.etap == 2) return "Сдать всё в пункте приёма";
        if (ig.etap == 3) return "Поесть: столовая с 12:00 до 15:00 или магазин";
        if (ig.etap == 4) return "Спросить совета у Михалыча на скамейке";
        if (ig.etap == 5) return "Сходить в Центр помощи к Анне";
        if (ig.etap == 6) return "Собрать 40 р. и подать заявление в паспортном столе";
        if (ig.etap == 7) return "Забрать паспорт в день " + ig.denPasporta;
        if (ig.etap == 8) return "Показать паспорт Анне";
        if (ig.etap == 9) return "Завод: нужны одежда, баня (сегодня или вчера) и надежда от 30%";
        return "Накопить 300 р. и снять комнату";
    }

    // Название места, куда указывает маркер на мини-карте
    public string NazvanieTseli()
    {
        if (ig.etap == 1) return "Мусорка";
        if (ig.etap == 2) return "Приём бутылок";
        if (ig.etap == 3) return "Столовая";
        if (ig.etap == 4) return "Скамейка";
        if (ig.etap == 5 || ig.etap == 8) return "Центр помощи";
        if (ig.etap == 6 || ig.etap == 7) return "Паспортный стол";
        if (ig.etap == 9) return "Завод";
        if (ig.etap == 10) return "Аренда жилья";
        return "";
    }

    public string Rayon()
    {
        if (ig.x < 2800) return "Окраина";
        if (ig.x < 6100) return "Центр";
        return "Промзона";
    }

    public Mesto MestoRyadom()
    {
        foreach (Mesto mesto in mesta)
        {
            if (Math.Abs(mesto.x - ig.x) < 110 && Math.Abs(mesto.y - ig.y) < 45) return mesto;
        }
        return null;
    }

    public bool RyadomSVyhodom()
    {
        return ig.xKomnaty < 210;
    }

    public bool RyadomSRabotnikom()
    {
        return ig.xKomnaty > 600;
    }

    public void Voyti(Mesto mesto)
    {
        vnutri = mesto;
        Zvuk.Igrat("dver");
        ig.xKomnaty = 130;
        ig.yKomnaty = 480;
        ig.smotritVpravo = true;
    }

    public void Vyyti()
    {
        ig.y = Verh;
        vnutri = null;
        Zvuk.Igrat("dver");
        if (ig.etap == 0) Etap(1);
    }

    public List<Deystvie> VidimyeDeystviya(Mesto mesto)
    {
        List<Deystvie> spisok = new List<Deystvie>();
        foreach (Deystvie deystvie in mesto.deystviya)
        {
            if (deystvie.etap == -1 || deystvie.etap == ig.etap) spisok.Add(deystvie);
        }
        return spisok;
    }

    public bool Dostupno(Deystvie deystvie)
    {
        if (deystvie.razVDen && deystvie.denVypolneniya == ig.den) return false;
        return deystvie.mozhno();
    }

    public void Vypolnit(Mesto mesto, Deystvie deystvie)
    {
        if (!Dostupno(deystvie))
        {
            Zvuk.Igrat("nelzya");
            Skazat("Сейчас это не получится.");
            return;
        }

        Zvuk.Igrat(deystvie.zvuk);
        deystvie.denVypolneniya = ig.den;
        ig.ProshloVremya(deystvie.chasy, mesto.vTeple);
        deystvie.sdelat();
        ig.Ogranichit();
        Proverit();
    }

    public void Idti(int poX, int poY, double sekundy)
    {
        if (poX != 0) ig.smotritVpravo = poX > 0;

        if (vnutri != null)
        {
            ig.xKomnaty = Math.Clamp(ig.xKomnaty + poX * sekundy * 300, 90, 900);
            ig.yKomnaty = Math.Clamp(ig.yKomnaty + poY * sekundy * 170, 445, 530);
            return;
        }

        ig.x = Math.Clamp(ig.x + poX * sekundy * 330, 20, Dlina - 20);
        ig.y = Math.Clamp(ig.y + poY * sekundy * 170, Verh, Niz);
        ig.ProshloVremya(sekundy * 0.07, false);
        Sobrat();
        Proverit();
    }

    // Вызывается в каждом кадре, пока игра не на паузе
    public void Kadr(double sekundy, bool pauza)
    {
        vremya += sekundy;
        vremyaRepliki += sekundy;
        vremyaSoobsheniya -= sekundy;
        vremyaVsplyvashki -= sekundy;
        vremyaBannera -= sekundy;
        vremyaZashchity -= sekundy;
        if (pauza) return;

        for (int i = 0; i < KolichestvoMashin; i++)
        {
            bool zadel = Math.Abs(MashinaX(i) - ig.x) < 62 && Math.Abs(MashinaY(i) - ig.y) < 15;
            if (!zadel || vnutri != null || vremyaZashchity > 0) continue;

            ig.y = ig.y < 465 ? 420 : 508;
            ig.sily = Math.Max(1, ig.sily - 4);
            vremyaZashchity = 4;
            Zvuk.Igrat("gudok");
            Skazat("Гудок! Машина едва не сбила Виктора. Дорогу лучше переходить внимательно.");
        }
    }

    public double MashinaX(int nomer)
    {
        double skorost = 190 + nomer * 35;
        double put = (vremya * skorost + nomer * 1900) % (Dlina + 400);
        return nomer % 2 == 0 ? put - 200 : Dlina + 200 - put;
    }

    public double MashinaY(int nomer)
    {
        return nomer % 2 == 0 ? 484 : 450;
    }

    void Proverit()
    {
        if (ig.chas >= 23)
        {
            Usnut(20, -30, -10);
            Skazat("Виктор не успел дойти до ночлега и уснул на улице.");
        }

        if (ig.sily <= 0 && !konec)
        {
            Zakonchit("bolnica", "Прохожий заметил на улице человека без сознания и вызвал скорую. " +
                      "Виктор выжил, но зима оказалась сильнее его плана. Всё придётся начинать сначала.");
        }
    }
}
