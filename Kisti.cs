using System;
using System.Drawing;
using System.Drawing.Drawing2D;

// Простые помощники для рисования. Все размеры в них обычные числа
static class Kisti
{
    public static void Pryamougolnik(Graphics g, Color cvet, double x, double y, double shirina, double vysota)
    {
        using (SolidBrush kist = new SolidBrush(cvet)) g.FillRectangle(kist, (float)x, (float)y, (float)shirina, (float)vysota);
    }

    public static void Skruglennyy(Graphics g, Color cvet, double x, double y, double shirina, double vysota, double radius)
    {
        float d = (float)(radius * 2);
        float levo = (float)x, verh = (float)y, pravo = (float)(x + shirina) - d, niz = (float)(y + vysota) - d;
        if (shirina < d || vysota < d) return;

        using (GraphicsPath kontur = new GraphicsPath())
        using (SolidBrush kist = new SolidBrush(cvet))
        {
            kontur.AddArc(levo, verh, d, d, 180, 90);
            kontur.AddArc(pravo, verh, d, d, 270, 90);
            kontur.AddArc(pravo, niz, d, d, 0, 90);
            kontur.AddArc(levo, niz, d, d, 90, 90);
            kontur.CloseFigure();
            g.FillPath(kist, kontur);
        }
    }

    public static void Krug(Graphics g, Color cvet, double x, double y, double radius)
    {
        Oval(g, cvet, x - radius, y - radius, radius * 2, radius * 2);
    }

    public static void Oval(Graphics g, Color cvet, double x, double y, double shirina, double vysota)
    {
        using (SolidBrush kist = new SolidBrush(cvet)) g.FillEllipse(kist, (float)x, (float)y, (float)shirina, (float)vysota);
    }

    public static void Liniya(Graphics g, Color cvet, double tolshina, double x1, double y1, double x2, double y2)
    {
        using (Pen pero = new Pen(cvet, (float)tolshina))
        {
            pero.StartCap = LineCap.Round;
            pero.EndCap = LineCap.Round;
            g.DrawLine(pero, (float)x1, (float)y1, (float)x2, (float)y2);
        }
    }

    // Многоугольник: координаты идут парами x, y
    public static void Mnogougolnik(Graphics g, Color cvet, params double[] chisla)
    {
        PointF[] tochki = new PointF[chisla.Length / 2];
        for (int i = 0; i < tochki.Length; i++) tochki[i] = new PointF((float)chisla[i * 2], (float)chisla[i * 2 + 1]);
        using (SolidBrush kist = new SolidBrush(cvet)) g.FillPolygon(kist, tochki);
    }

    // Плавный переход цвета сверху вниз
    public static void Gradient(Graphics g, Color verh, Color niz, double x, double y, double shirina, double vysota)
    {
        RectangleF oblast = new RectangleF((float)x, (float)y, (float)shirina, (float)vysota);
        RectangleF kistObl = new RectangleF((float)x, (float)y - 1, (float)shirina, (float)vysota + 2);
        using (LinearGradientBrush kist = new LinearGradientBrush(kistObl, verh, niz, 90f)) g.FillRectangle(kist, oblast);
    }

    // Мягкое круглое свечение: яркое в центре, прозрачное по краю
    public static void Svechenie(Graphics g, Color cvet, double x, double y, double radius)
    {
        using (GraphicsPath kontur = new GraphicsPath())
        {
            kontur.AddEllipse((float)(x - radius), (float)(y - radius), (float)(radius * 2), (float)(radius * 2));
            using (PathGradientBrush kist = new PathGradientBrush(kontur))
            {
                kist.CenterColor = cvet;
                kist.SurroundColors = new Color[] { Color.FromArgb(0, cvet) };
                g.FillPath(kist, kontur);
            }
        }
    }

    public static void Tekst(Graphics g, string stroka, Font shrift, Color cvet, double x, double y, double shirina, bool poCentru)
    {
        RectangleF oblast = new RectangleF((float)x, (float)y, (float)shirina, Risovanie.Vysota);
        using (StringFormat format = new StringFormat())
        using (SolidBrush kist = new SolidBrush(cvet))
        {
            if (poCentru) format.Alignment = StringAlignment.Center;
            g.DrawString(stroka, shrift, kist, oblast, format);
        }
    }

    public static Color Alfa(Color cvet, double alfa)
    {
        return Color.FromArgb((int)(Math.Clamp(alfa, 0, 255)), cvet);
    }

    public static Color Smes(Color a, Color b, double dolya)
    {
        // Доля строго от 0 до 1, иначе цвет выйдет за пределы и игра упадёт
        dolya = Math.Clamp(dolya, 0, 1);

        return Color.FromArgb(
            (int)(a.R + (b.R - a.R) * dolya),
            (int)(a.G + (b.G - a.G) * dolya),
            (int)(a.B + (b.B - a.B) * dolya));
    }

    // Цвет объекта при текущем освещении
    public static Color Temno(Color cvet, double svet)
    {
        return Smes(Color.FromArgb(12, 14, 34), cvet, 0.28 + svet * 0.72);
    }
}
