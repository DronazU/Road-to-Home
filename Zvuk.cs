using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

// Один играющий звук. Громкость меняется плавно, чтобы не было щелчков
class Dorozhka : ISampleProvider
{
    float[] dannye;
    int poziciya = 0;
    bool povtor;
    public float gromkost, cel;

    public Dorozhka(float[] dannye, float gromkost, bool povtor)
    {
        this.dannye = dannye;
        this.gromkost = gromkost;
        this.cel = gromkost;
        this.povtor = povtor;
    }

    public WaveFormat WaveFormat
    {
        get { return Zvuk.format; }
    }

    public int Read(float[] bufer, int sdvig, int kolichestvo)
    {
        int zapisano = 0;

        while (zapisano < kolichestvo)
        {
            if (poziciya >= dannye.Length)
            {
                if (!povtor) break;
                poziciya = 0;
            }

            gromkost += (cel - gromkost) * 0.00006f;
            bufer[sdvig + zapisano] = dannye[poziciya] * gromkost;
            poziciya++;
            zapisano++;
        }

        return zapisano;
    }
}

// Музыка, погода и звуки действий. Файлы лежат в папке Zvuki
static class Zvuk
{
    public static WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat(22050, 2);
    public static bool vyklyucheno = false;

    static Dictionary<string, float[]> zvuki = new Dictionary<string, float[]>();
    static Dictionary<string, Dorozhka> fon = new Dictionary<string, Dorozhka>();
    static MixingSampleProvider mikser = null;
    static WaveOutEvent vyhod = null;
    static Random sluchay = new Random();

    // Звук загружается отдельно, поэтому не задерживает открытие игры
    public static void Zagruzit()
    {
        Task.Run(() =>
        {
            try
            {
                Podgotovit();
            }
            catch (Exception oshibka)
            {
                Zhurnal.Zapisat(oshibka);
            }
        });
    }

    static void Podgotovit()
    {
        string papka = Path.Combine(AppContext.BaseDirectory, "Zvuki");
        Dictionary<string, float[]> zagruzheno = new Dictionary<string, float[]>();

        foreach (string fayl in Directory.GetFiles(papka, "*.wav"))
        {
            zagruzheno[Path.GetFileNameWithoutExtension(fayl)] = ProchitatWav(fayl);
        }

        MixingSampleProvider novyyMikser = new MixingSampleProvider(format);
        novyyMikser.ReadFully = true;

        foreach (string imya in zagruzheno.Keys)
        {
            if (!imya.StartsWith("fon_")) continue;
            Dorozhka dorozhka = new Dorozhka(zagruzheno[imya], 0, true);
            fon[imya] = dorozhka;
            novyyMikser.AddMixerInput(dorozhka);
        }

        WaveOutEvent novyyVyhod = new WaveOutEvent();
        novyyVyhod.DesiredLatency = 150;
        novyyVyhod.Init(novyyMikser, true);
        novyyVyhod.Play();

        zvuki = zagruzheno;
        mikser = novyyMikser;
        vyhod = novyyVyhod;
    }

    // Файлы созданы в одном формате: 22050 Гц, стерео, 16 бит, заголовок 44 байта
    static float[] ProchitatWav(string fayl)
    {
        byte[] bayty = File.ReadAllBytes(fayl);
        float[] dannye = new float[(bayty.Length - 44) / 2];

        for (int i = 0; i < dannye.Length; i++)
        {
            dannye[i] = BitConverter.ToInt16(bayty, 44 + i * 2) / 32768f;
        }
        return dannye;
    }

    public static void Igrat(string imya)
    {
        Sygrat(imya, 1f);
    }

    // У шагов несколько вариантов, чтобы они не звучали одинаково
    public static void Shag(string imya)
    {
        Sygrat(imya + sluchay.Next(1, 4), 0.55f);
    }

    static void Sygrat(string imya, float gromkost)
    {
        if (mikser == null || !zvuki.ContainsKey(imya)) return;
        mikser.AddMixerInput(new Dorozhka(zvuki[imya], gromkost, false));
    }

    public static void Pereklyuchit()
    {
        vyklyucheno = !vyklyucheno;
        if (vyhod != null) vyhod.Volume = vyklyucheno ? 0f : 1f;
    }

    static void Gromkost(string imya, double znachenie)
    {
        if (fon.ContainsKey(imya)) fon[imya].cel = (float)znachenie;
    }

    // Каждый кадр подстраивает музыку и погоду под место и время
    public static void Obnovit(Gorod gorod, Mesto otkrytoe)
    {
        if (mikser == null) return;

        if (gorod == null)
        {
            Gromkost("fon_muzyka_noch", 0.55);
            Gromkost("fon_muzyka_den", 0);
            Gromkost("fon_veter", 0.2);
            Gromkost("fon_dozhd", 0);
            Gromkost("fon_ptitsy", 0);
            Gromkost("fon_gorod", 0);
            Gromkost("fon_mashina", 0);
            Gromkost("fon_komnata", 0);
            Gromkost("fon_cex", 0);
            return;
        }

        Igrok ig = gorod.ig;
        double svet = Risovanie.Svet(ig.chas);
        bool vnutri = gorod.vnutri != null;
        double ulica = vnutri ? 0.12 : 1;
        bool noch = svet < 0.4;

        double blizhayshaya = 100000;
        for (int i = 0; i < Gorod.KolichestvoMashin; i++)
        {
            blizhayshaya = Math.Min(blizhayshaya, Math.Abs(gorod.MashinaX(i) - ig.x));
        }

        double kZavodu = vnutri ? (gorod.vnutri.komnata == "zavod" ? 0 : 100000) : Math.Abs(7620 - ig.x);

        Gromkost("fon_muzyka_den", noch ? 0 : 0.42);
        Gromkost("fon_muzyka_noch", noch ? 0.42 : 0);
        Gromkost("fon_dozhd", gorod.Dozhd() ? 0.75 * ulica : 0);
        Gromkost("fon_veter", (0.12 + gorod.Zima() * 0.6) * ulica);
        Gromkost("fon_ptitsy", !noch && !gorod.Dozhd() && gorod.Zima() == 0 ? 0.4 * ulica : 0);
        Gromkost("fon_gorod", (noch ? 0.14 : 0.32) * ulica);
        Gromkost("fon_mashina", Math.Max(0, 1 - blizhayshaya / 420) * 0.8 * ulica);
        Gromkost("fon_komnata", vnutri ? 0.35 : 0);
        Gromkost("fon_cex", Math.Max(0, 1 - kZavodu / 500) * 0.65);

        // Изредка гремит гром
        if (gorod.Dozhd() && !vnutri && sluchay.Next(1500) == 0) Igrat("grom");
    }
}
