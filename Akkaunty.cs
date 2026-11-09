using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

// Запись об игроке. Сам пароль не хранится, только его защищённый отпечаток
class Polzovatel
{
    public string sol { get; set; }
    public string hesh { get; set; }
}

// Аккаунты и сохранения лежат в папке AppData\Roaming\DorogaDomoy
static class Akkaunty
{
    static string papka = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DorogaDomoy");
    static JsonSerializerOptions nastroyki = new JsonSerializerOptions { IncludeFields = true, WriteIndented = true };

    public static string Put(string imyaFayla)
    {
        Directory.CreateDirectory(papka);
        return Path.Combine(papka, imyaFayla);
    }

    static Dictionary<string, Polzovatel> Prochitat()
    {
        string fayl = Put("polzovateli.json");
        if (!File.Exists(fayl)) return new Dictionary<string, Polzovatel>();
        return JsonSerializer.Deserialize<Dictionary<string, Polzovatel>>(File.ReadAllText(fayl));
    }

    // Запись идёт через временный файл, чтобы сбой не испортил старые данные
    static void ZapisatFayl(string fayl, string tekst)
    {
        File.WriteAllText(fayl + ".tmp", tekst);
        File.Move(fayl + ".tmp", fayl, true);
    }

    static string Hesh(string parol, byte[] sol)
    {
        byte[] otpechatok = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(parol), sol, 100000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(otpechatok);
    }

    // Возвращает текст ошибки или пустую строку
    public static string Zaregistrirovat(string pochta, string parol)
    {
        try
        {
            Dictionary<string, Polzovatel> polzovateli = Prochitat();
            string klyuch = pochta.ToLowerInvariant();
            if (polzovateli.ContainsKey(klyuch)) return "Такая почта уже зарегистрирована";

            byte[] sol = RandomNumberGenerator.GetBytes(16);
            Polzovatel novyy = new Polzovatel();
            novyy.sol = Convert.ToBase64String(sol);
            novyy.hesh = Hesh(parol, sol);
            polzovateli[klyuch] = novyy;

            ZapisatFayl(Put("polzovateli.json"), JsonSerializer.Serialize(polzovateli, nastroyki));
            return "";
        }
        catch (Exception oshibka)
        {
            Zhurnal.Zapisat(oshibka);
            return "Не удалось сохранить аккаунт";
        }
    }

    public static bool Voyti(string pochta, string parol)
    {
        try
        {
            Dictionary<string, Polzovatel> polzovateli = Prochitat();
            string klyuch = pochta.ToLowerInvariant();
            if (!polzovateli.ContainsKey(klyuch)) return false;

            Polzovatel polzovatel = polzovateli[klyuch];
            return Hesh(parol, Convert.FromBase64String(polzovatel.sol)) == polzovatel.hesh;
        }
        catch (Exception oshibka)
        {
            Zhurnal.Zapisat(oshibka);
            return false;
        }
    }

    static string FaylSohraneniya(string pochta)
    {
        byte[] hesh = SHA256.HashData(Encoding.UTF8.GetBytes(pochta.ToLowerInvariant()));
        return Put("igra_" + Convert.ToHexString(hesh) + ".json");
    }

    public static void Sohranit(string pochta, Igrok igrok)
    {
        try
        {
            ZapisatFayl(FaylSohraneniya(pochta), JsonSerializer.Serialize(igrok, nastroyki));
        }
        catch (Exception oshibka)
        {
            Zhurnal.Zapisat(oshibka);
        }
    }

    // Возвращает сохранённого героя или null, если сохранения нет
    public static Igrok Zagruzit(string pochta)
    {
        try
        {
            string fayl = FaylSohraneniya(pochta);
            if (!File.Exists(fayl)) return null;
            return JsonSerializer.Deserialize<Igrok>(File.ReadAllText(fayl), nastroyki);
        }
        catch (Exception oshibka)
        {
            Zhurnal.Zapisat(oshibka);
            return null;
        }
    }

    public static void Udalit(string pochta)
    {
        try
        {
            File.Delete(FaylSohraneniya(pochta));
        }
        catch (Exception oshibka)
        {
            Zhurnal.Zapisat(oshibka);
        }
    }
}
