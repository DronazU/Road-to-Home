using System;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.ThreadException += (otpravitel, sobytie) => Zhurnal.Zapisat(sobytie.Exception);
        Application.Run(new Okno());
    }
}
