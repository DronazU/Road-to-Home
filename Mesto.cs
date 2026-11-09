using System;
using System.Collections.Generic;
using System.Drawing;

// Одно действие в меню, например "Купить хлеб"
class Deystvie
{
    public string tekst;
    public double chasy;
    public Func<bool> mozhno;
    public Action sdelat;
    public bool razVDen;
    public int etap;
    public int denVypolneniya = 0;
    public string zvuk = "vybor";
}

// Одна реплика разговора или один кадр рассказа
class Replika
{
    public string kto, tekst, kartinka;
}

// Как выглядит человек
class Vneshnost
{
    public Color odezhda, volosy, boroda;
    public bool zhenshina, ryukzak, ushanka;

    public static Vneshnost Muzhchina(Color odezhda, Color volosy, Color boroda)
    {
        Vneshnost vid = new Vneshnost();
        vid.odezhda = odezhda;
        vid.volosy = volosy;
        vid.boroda = boroda;
        return vid;
    }

    public static Vneshnost Zhenshina(Color odezhda, Color volosy)
    {
        Vneshnost vid = new Vneshnost();
        vid.odezhda = odezhda;
        vid.volosy = volosy;
        vid.zhenshina = true;
        return vid;
    }
}

// Вещь, которая лежит на улице
class Nahodka
{
    public double x, y;
    public string vid;
}

// Место в городе: здание, мусорка, скамейка или остановка
class Mesto
{
    public string nazvanie;
    public int x, y, otkrytoS, otkrytoDo;
    public bool vTeple;
    public Color cvet;
    public string vid;

    // Как здание выглядит снаружи и внутри, и кто в нём работает
    public string stil = "";
    public string komnata = "";
    public string rabotnik = "";
    public Vneshnost vneshnost = null;
    public int nomerRepliki = 0;

    public List<Deystvie> deystviya = new List<Deystvie>();

    public bool Otkryto(double chas)
    {
        return chas >= otkrytoS && chas < otkrytoDo;
    }

    // etap = -1 значит, что действие видно всегда
    public Deystvie Dobavit(string tekst, double chasy, Func<bool> mozhno, Action sdelat, bool razVDen = false, int etap = -1)
    {
        Deystvie deystvie = new Deystvie();
        deystvie.tekst = tekst;
        deystvie.chasy = chasy;
        deystvie.mozhno = mozhno;
        deystvie.sdelat = sdelat;
        deystvie.razVDen = razVDen;
        deystvie.etap = etap;
        deystviya.Add(deystvie);
        return deystvie;
    }
}
