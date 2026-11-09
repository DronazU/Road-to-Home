using System;
using System.Collections.Generic;

// Всё, что известно о герое: шкалы, деньги, вещи и время
class Igrok
{
    // Шкалы от 0 до 100
    public double eda = 60, teplo = 60, sily = 70, nadezhda = 35;

    public int dengi = 0, butylki = 0, banki = 0, metall = 0;
    public int den = 1;
    public double chas = 8;

    // Положение на улице и внутри комнаты
    public double x = 220, y = Gorod.Verh;
    public double xKomnaty = 130, yKomnaty = 480;
    public bool smotritVpravo = true;

    // Этап истории: 0 - начало, 5 - есть работа
    public int etap = 0;
    public int denPasporta = 0, denPisma = 0, denBani = -10;
    public bool kurtka, spalnik, odezhda, otvetKati, mihalychSpasen;

    // Номера действий "раз в день", уже сделанных сегодня. Нужны для сохранения
    public List<int> sdelanoSegodnya = new List<int>();

    // С каждым днём на улице холоднее
    public int Temperatura()
    {
        return 6 - den;
    }

    public void ProshloVremya(double chasy, bool vTeple)
    {
        chas += chasy;
        eda -= 2.5 * chasy;
        sily -= 1 * chasy;

        double holod = 1.5 + den * 0.1;
        if (kurtka) holod = holod / 2;

        if (vTeple) teplo += 6 * chasy;
        else teplo -= holod * chasy;

        // Голод и холод отнимают силы
        if (eda <= 0 || teplo <= 0) sily -= 7 * chasy;
        Ogranichit();
    }

    public void Spat(double dobavitSil, double dobavitTepla, double dobavitNadezhdy)
    {
        den++;
        chas = 7;
        eda -= 15;
        sily += dobavitSil;
        teplo += dobavitTepla;
        nadezhda += dobavitNadezhdy;

        if (eda <= 0 || teplo <= 0) sily -= 20;
        Ogranichit();
    }

    // Шкалы не должны выходить за пределы 0 и 100
    public void Ogranichit()
    {
        eda = Math.Clamp(eda, 0, 100);
        teplo = Math.Clamp(teplo, 0, 100);
        sily = Math.Clamp(sily, 0, 100);
        nadezhda = Math.Clamp(nadezhda, 0, 100);
    }
}
