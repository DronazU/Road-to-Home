using System;
using System.IO;

// Записывает ошибки в файл, чтобы игра не падала, а причину можно было найти
static class Zhurnal
{
    static string poslednyaya = "";

    public static void Zapisat(Exception oshibka)
    {
        string tekst = oshibka.ToString();
        if (tekst == poslednyaya) return;
        poslednyaya = tekst;

        try
        {
            File.AppendAllText(Akkaunty.Put("oshibki.txt"), DateTime.Now + "\n" + tekst + "\n\n");
        }
        catch (Exception)
        {
            // Если файл записать нельзя, игра всё равно продолжается
        }
    }
}
