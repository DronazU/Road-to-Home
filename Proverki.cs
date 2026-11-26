// Проверки почты и пароля. Пустая строка значит, что ошибок нет
static class Proverki
{
    public static string Pochta(string pochta)
    {
        if (pochta == "") return "Введи почту";

        foreach (char bukva in pochta)
        {
            if (bukva >= 'Ѐ' && bukva <= 'ӿ') return "В почте нельзя использовать русские буквы";
            if (bukva == ' ') return "В почте не должно быть пробелов";
        }

        int sobaka = pochta.IndexOf('@');
        if (sobaka < 0) return "В почте должен быть знак @";
        if (sobaka == 0) return "Перед знаком @ должно быть имя";
        if (pochta.LastIndexOf('@') != sobaka) return "В почте может быть только один знак @";

        int tochka = pochta.LastIndexOf('.');
        if (tochka < sobaka) return "После @ должна быть точка, например mail.ru";
        if (tochka == sobaka + 1 || tochka == pochta.Length - 1) return "Проверь адрес после @, например mail.ru";
        return "";
    }

    public static bool DlinaParolya(string parol)
    {
        return parol.Length >= 8;
    }

    public static bool EstBolshaya(string parol)
    {
        foreach (char bukva in parol)
        {
            if (char.IsUpper(bukva)) return true;
        }
        return false;
    }

    public static string Parol(string parol)
    {
        if (!DlinaParolya(parol)) return "В пароле должно быть минимум 8 символов";
        if (!EstBolshaya(parol)) return "В пароле должна быть минимум 1 большая буква";
        return "";
    }
}
