using System;
using System.Collections.Generic;
using System.Drawing;

// Карта города: все места, персонажи, их реплики и действия
static class Karta
{
    static Mesto Zdanie(Gorod gorod, string nazvanie, int x, int otkrytoS, int otkrytoDo, Color cvet, string stil, string komnata)
    {
        Mesto mesto = gorod.Novoe(nazvanie, x, otkrytoS, otkrytoDo, true, cvet, "zdanie");
        mesto.stil = stil;
        mesto.komnata = komnata;
        return mesto;
    }

    // Добавляет работника и пункт "Поговорить". Реплики идут по кругу
    static void Rabotnik(Gorod gorod, Mesto mesto, string imya, Vneshnost vid, params string[] repliki)
    {
        mesto.rabotnik = imya;
        mesto.vneshnost = vid;
        mesto.Dobavit("Поговорить: " + imya, 0.25, () => true, () =>
        {
            gorod.Razgovor(imya, repliki[mesto.nomerRepliki % repliki.Length]);
            mesto.nomerRepliki++;
        });
    }

    public static void Sozdat(Gorod gorod)
    {
        Igrok ig = gorod.ig;
        Func<bool> vsegda = () => true;

        Mesto podval = Zdanie(gorod, "Подвал", 220, 0, 24, Color.FromArgb(120, 118, 112), "panelka", "podval");
        podval.vTeple = false;
        podval.rabotnik = "Матрас";
        podval.Dobavit("Посмотреть на фотографию Кати", 0.25, vsegda, () =>
        {
            ig.nadezhda += 6;
            gorod.Razgovor("Виктор", "Катя. На этом снимке тебе пятнадцать. Сейчас уже двадцать два. Прости меня.");
        }, true).zvuk = "bumaga";
        podval.Dobavit("Лечь спать (после 18:00)", 0, () => ig.chas >= 18, () =>
        {
            gorod.Usnut(55, ig.spalnik ? -5 : -20, -5);
            gorod.Skazat("Ночь прошла. В подвале сыро, но это лучше, чем на улице.");
        }).zvuk = "matras";

        Mesto musorka1 = gorod.Novoe("Мусорка", 430, 0, 24, false, Color.SeaGreen, "musorka");
        musorka1.Dobavit("Поискать бутылки", 1, vsegda, gorod.PoiskatVMusorke, true).zvuk = "musor";

        Mesto stolovaya = Zdanie(gorod, "Столовая", 620, 12, 15, Color.IndianRed, "kirpich", "stolovaya");
        Rabotnik(gorod, stolovaya, "Галина", Vneshnost.Zhenshina(Color.WhiteSmoke, Color.FromArgb(120, 70, 50)),
            "Садись, садись. Первый раз вижу тебя с таким взглядом. Значит, что-то решил?",
            "Суп сегодня с перловкой. Не ресторан, зато горячий и бесплатный.",
            "Я здесь пятнадцать лет на раздаче. Знаешь, сколько людей отсюда выбрались? Больше, чем ты думаешь.");
        stolovaya.Dobavit("Пообедать", 1, vsegda, () =>
        {
            ig.eda += 50;
            if (ig.etap == 3)
            {
                gorod.Etap(4);
                gorod.Razgovor("Галина", "Ешь и слушай. Сходи к Михалычу, он сидит на скамейке возле рынка. Он этот город знает лучше всех.");
            }
            else
            {
                gorod.Skazat("Горячий суп и хлеб. Здесь никто не задаёт лишних вопросов.");
            }
        }, true).zvuk = "eda";

        Mesto magazin = Zdanie(gorod, "Продукты", 1000, 8, 22, Color.Goldenrod, "magazin", "magazin");
        Rabotnik(gorod, magazin, "Зина", Vneshnost.Zhenshina(Color.CadetBlue, Color.FromArgb(200, 170, 90)),
            "Только не стой на проходе, ладно? Хлеб свежий, с утра привезли.",
            "Вчера какой-то студент оставил сдачу и сказал: это вам на чай. Так что чай сегодня за счёт заведения. Шучу. Три рубля.",
            "Ты другой какой-то стал. Побрился, что ли? Ну, так держать.");
        magazin.Dobavit("Хлеб и горячий чай - 3 р.", 0.5, () => ig.dengi >= 3, () =>
        {
            ig.dengi -= 3;
            ig.eda += 30;
            if (ig.etap == 3)
            {
                gorod.Etap(4);
                gorod.Razgovor("Виктор", "Надо спросить совета у Михалыча. Он сидит на скамейке возле рынка и всё про всех знает.");
            }
            else
            {
                gorod.Skazat("Простая еда, а на душе стало легче.");
            }
        }).zvuk = "pakety";

        Mesto ostanovka1 = gorod.Novoe("Окраина", 1210, 6, 23, false, Color.SteelBlue, "ostanovka");

        Mesto rynok = Zdanie(gorod, "Рынок", 1400, 8, 18, Color.SteelBlue, "rynok", "rynok");
        Rabotnik(gorod, rynok, "Степаныч", Vneshnost.Muzhchina(Color.DarkOliveGreen, Color.Gray, Color.Gray),
            "Работа есть, если спина выдержит. Фура приходит рано, разгружать надо быстро.",
            "Я сам когда-то грузчиком начинал. Сейчас командую, а спина помнит всё.",
            "Плачу честно, сразу и без лишних разговоров. Кто работает, тот у меня и ест.");
        rynok.Dobavit("Разгрузить фуру: заработать 15 р. (нужно 40 сил)", 4, () => ig.sily >= 40, () =>
        {
            ig.dengi += 15;
            ig.sily -= 30;
            gorod.Skazat("Спина болит, зато деньги честные.");
        }, true).zvuk = "gruz";
        rynok.Dobavit("Горячий пирожок - 2 р.", 0.25, () => ig.dengi >= 2, () =>
        {
            ig.dengi -= 2;
            ig.eda += 18;
            gorod.Skazat("С капустой. Лучший пирожок за этот год.");
        }).zvuk = "eda";

        Mesto skameyka = gorod.Novoe("Скамейка", 1640, 0, 24, false, Color.Sienna, "skameyka");
        skameyka.rabotnik = "Михалыч";
        skameyka.vneshnost = Vneshnost.Muzhchina(Color.FromArgb(82, 88, 62), Color.FromArgb(82, 45, 38), Color.FromArgb(175, 175, 170));
        skameyka.vneshnost.ushanka = true;
        skameyka.Dobavit("Спросить совета", 0.5, () => !ig.mihalychSpasen, () =>
        {
            gorod.Etap(5);
            ig.nadezhda += 10;
            gorod.Razgovor(
                "Виктор", "Михалыч, как отсюда выбраться? Ну, насовсем.",
                "Михалыч", "Насовсем, говоришь? Я вот не смог. А ты можешь. У тебя руки золотые и дочка есть.",
                "Михалыч", "В центре города работает Анна, соцработница. Иди к ней, Витя, не тяни.");
        }, false, 4).zvuk = "vybor";
        skameyka.Dobavit("Поговорить с Михалычем", 1, () => !ig.mihalychSpasen, () =>
        {
            ig.nadezhda += 12;
            string[] slova =
            {
                "Ты, Витя, держись. У тебя хотя бы есть, куда идти.",
                "Я в восемьдесят шестом мосты строил. Вон тот, за вокзалом, тоже мой. Смешно, да?",
                "Знаешь, что самое страшное? Когда тебя перестают замечать. Спасибо, что садишься рядом.",
                "Завод бы твой открыли обратно, вот бы я посмотрел, как ты за станок встаёшь."
            };
            gorod.Razgovor("Михалыч", slova[skameyka.nomerRepliki % slova.Length]);
            skameyka.nomerRepliki++;
        }, true);
        skameyka.Dobavit("Выпить с Михалычем - 3 р.", 1, () => ig.dengi >= 3 && !ig.mihalychSpasen, () =>
        {
            ig.dengi -= 3;
            ig.teplo += 25;
            ig.nadezhda -= 15;
            ig.sily -= 5;
            gorod.Skazat("На час стало теплее. Потом стало только хуже.");
        }).zvuk = "glotok";
        skameyka.Dobavit("Отвести Михалыча к Анне", 1, () => !ig.mihalychSpasen, () =>
        {
            ig.mihalychSpasen = true;
            ig.nadezhda += 20;
            gorod.Razgovor(
                "Виктор", "Вставай, Михалыч. Пойдём к Анне. Я сам тебя отведу.",
                "Михалыч", "Спасибо, Витя. Я уже думал, что про меня все забыли.");
        }, false, 10);

        Mesto sekond = Zdanie(gorod, "Секонд-хенд", 1800, 9, 20, Color.Orchid, "magazin", "sekond");
        Rabotnik(gorod, sekond, "Лидия", Vneshnost.Zhenshina(Color.MediumVioletRed, Color.FromArgb(60, 40, 40)),
            "Заходите, не стесняйтесь. Вещи хорошие, только немного пожившие. Как и мы с вами.",
            "На собеседование берите светлую рубашку. Встречают по одёжке, это не я придумала.",
            "Куртки разбирают первыми, как только похолодает. Не тяните.");
        sekond.Dobavit("Тёплая куртка - 15 р.", 0.5, () => ig.dengi >= 15 && !ig.kurtka, () =>
        {
            ig.dengi -= 15;
            ig.kurtka = true;
            gorod.Skazat("Старая, но тёплая. Холод теперь вдвое слабее.");
        }).zvuk = "odezhda";
        sekond.Dobavit("Спальный мешок - 20 р.", 0.5, () => ig.dengi >= 20 && !ig.spalnik, () =>
        {
            ig.dengi -= 20;
            ig.spalnik = true;
            gorod.Skazat("Ночью в подвале теперь почти не холодно.");
        }).zvuk = "odezhda";
        sekond.Dobavit("Чистая рубашка и брюки - 20 р.", 0.5, () => ig.dengi >= 20 && !ig.odezhda, () =>
        {
            ig.dengi -= 20;
            ig.odezhda = true;
            ig.nadezhda += 5;
            gorod.Skazat("В зеркале Виктор почти узнал себя прежнего.");
        }).zvuk = "odezhda";

        Mesto priyomka = Zdanie(gorod, "Приём бутылок", 2200, 9, 19, Color.Teal, "garazh", "priyomka");
        Rabotnik(gorod, priyomka, "Семёныч", Vneshnost.Muzhchina(Color.SaddleBrown, Color.DimGray, Color.DimGray),
            "Бутылка рубль, банка рубль, железо три. Цены не я придумал, я их только терплю.",
            "Сегодня с утра уже трое заходили. На тротуарах ещё что-то лежит, если поторопишься.",
            "Металл бери, если найдёшь. За него платят лучше всего.");
        priyomka.Dobavit("Сдать всё (бутылка и банка 1 р., металл 3 р.)", 0.25, () => gorod.CenaVtorsyrya() > 0, gorod.SdatVtorsyre);

        Mesto musorka2 = gorod.Novoe("Мусорка", 2460, 0, 24, false, Color.SeaGreen, "musorka");
        musorka2.Dobavit("Поискать бутылки", 1, vsegda, gorod.PoiskatVMusorke, true).zvuk = "musor";

        Mesto centr = Zdanie(gorod, "Центр помощи", 3000, 9, 22, Color.MediumPurple, "klassika", "centr");
        Rabotnik(gorod, centr, "Анна", Vneshnost.Zhenshina(Color.Teal, Color.FromArgb(70, 45, 35)),
            "Как вы сегодня, Виктор? Ели? Не стесняйтесь, чай в термосе всегда есть.",
            "Я не волшебница. Но документы, справки и один нужный звонок иногда меняют всю жизнь.",
            "Самое трудное вы уже сделали: пришли сюда. Дальше просто шаг за шагом.");
        centr.Dobavit("Рассказать Анне о себе", 1, vsegda, () =>
        {
            gorod.Etap(6);
            ig.nadezhda += 10;
            gorod.Razgovor(
                "Анна", "Садитесь, Виктор. Вот чай. Расскажите, что случилось.",
                "Виктор", "Завод закрылся, квартиры нет, а паспорт украли. Я уже и не знаю, с чего начать.",
                "Анна", "Начнём с паспорта. Без него вам не дадут ни работу, ни жильё. Соберите сорок рублей и идите в паспортный стол.",
                "Виктор", "Спасибо. За долгое время это первый план, который у меня есть.");
        }, false, 5);
        centr.Dobavit("Показать Анне паспорт", 1, vsegda, () =>
        {
            gorod.Etap(9);
            ig.nadezhda += 10;
            gorod.Razgovor(
                "Анна", "Отлично, Виктор! А у меня тоже новость. Заводу в промзоне нужен токарь, я уже позвонила.",
                "Виктор", "Токарь? Вы серьёзно?",
                "Анна", "Купите чистую одежду, сходите в баню и приходите на собеседование. У вас всё получится.");
        }, false, 8);
        centr.Dobavit("Погреться и выпить чаю", 1, vsegda, () =>
        {
            ig.teplo += 20;
            ig.eda += 5;
            gorod.Skazat("Здесь тепло, и можно просто посидеть в тишине.");
        }, true).zvuk = "glotok";
        centr.Dobavit("Ночлег - 5 р. (после 18:00)", 0, () => ig.chas >= 18 && ig.dengi >= 5, () =>
        {
            ig.dengi -= 5;
            gorod.Usnut(85, 30, 5);
            gorod.Skazat("Настоящая кровать и чистое бельё. Утро началось хорошо.");
        }).zvuk = "matras";

        Mesto cerkov = Zdanie(gorod, "Церковь", 3420, 8, 20, Color.WhiteSmoke, "cerkov", "cerkov");
        Rabotnik(gorod, cerkov, "Отец Николай", Vneshnost.Muzhchina(Color.FromArgb(35, 35, 40), Color.DimGray, Color.Silver),
            "Проходите. Здесь никто не спросит, откуда вы и что у вас в карманах.",
            "Отчаяние говорит очень громко, но почти всегда лжёт. Посидите в тишине.",
            "Двор надо убрать, а рук не хватает. За работу накормим, у нас так заведено.");
        cerkov.Dobavit("Посидеть в тишине", 1, vsegda, () =>
        {
            ig.nadezhda += 10;
            gorod.Skazat("Тихо горят свечи. Мысли понемногу успокаиваются.");
        }, true).zvuk = "kolokol";
        cerkov.Dobavit("Помочь убрать двор за обед", 2, () => ig.sily >= 20, () =>
        {
            ig.eda += 40;
            gorod.Skazat("Двор чист. Каша с чаем показалась праздничным обедом.");
        }, true).zvuk = "metla";

        Mesto biblioteka = Zdanie(gorod, "Библиотека", 3840, 9, 19, Color.BurlyWood, "klassika", "biblioteka");
        Rabotnik(gorod, biblioteka, "Вера Павловна", Vneshnost.Zhenshina(Color.DarkSlateBlue, Color.Silver),
            "Читальный зал открыт для всех. Главное, тишина и чистые руки.",
            "Вчера вы читали справочник токаря. Я его отложила, он ваш на сколько нужно.",
            "Книги никого не спасают, но помогают дождаться спасения.");
        biblioteka.Dobavit("Почитать газеты и объявления", 1, vsegda, () =>
        {
            ig.nadezhda += 6;
            gorod.Skazat("В разделе вакансий обведено: требуются рабочие.");
        }, true).zvuk = "bumaga";
        biblioteka.Dobavit("Посидеть в читальном зале", 1, vsegda, () =>
        {
            ig.teplo += 20;
            gorod.Skazat("Батарея под столом греет лучше любого чая.");
        }, true).zvuk = "bumaga";

        Mesto ostanovka2 = gorod.Novoe("Центр", 4010, 6, 23, false, Color.SteelBlue, "ostanovka");

        Mesto banya = Zdanie(gorod, "Баня", 4240, 10, 21, Color.CadetBlue, "kirpich", "banya");
        Rabotnik(gorod, banya, "Петрович", Vneshnost.Muzhchina(Color.WhiteSmoke, Color.LightGray, Color.FromArgb(226, 190, 160)),
            "Пар сегодня лёгкий. Заходи, с тебя пять рублей и хорошее настроение.",
            "После бани человек другой. Я сорок лет за этим наблюдаю.",
            "На собеседование? Тогда мойся как следует. Мастера чистоту замечают первой.");
        banya.Dobavit("Помыться - 5 р.", 1, () => ig.dengi >= 5, () =>
        {
            ig.dengi -= 5;
            ig.denBani = ig.den;
            ig.nadezhda += 5;
            gorod.Skazat("Горячая вода смыла не только грязь, но и усталость.");
        }).zvuk = "voda";

        Mesto pochta = Zdanie(gorod, "Почта", 4640, 9, 18, Color.RoyalBlue, "klassika", "pochta");
        Rabotnik(gorod, pochta, "Нина", Vneshnost.Zhenshina(Color.RoyalBlue, Color.FromArgb(150, 80, 50)),
            "Сейчас почти никто не пишет писем. Если решитесь, конверт у меня найдётся.",
            "Ответы идут дня четыре, иногда дольше. Не теряйте надежду.",
            "Кому письмо? Дочери? Тогда пишите просто. Просто всегда лучше всего.");
        pochta.Dobavit("Написать письмо Кате - 3 р.", 1, () => ig.dengi >= 3 && ig.denPisma == 0, () =>
        {
            ig.dengi -= 3;
            ig.denPisma = ig.den;
            ig.nadezhda += 10;
            gorod.Razgovor("Виктор", "Катя, это папа. Я не знаю, простишь ли ты меня. Я просто хочу сказать, что живой и что стараюсь.");
        }).zvuk = "pero";
        pochta.Dobavit("Спросить, нет ли ответа", 0.25, () => ig.denPisma > 0 && ig.den >= ig.denPisma + 4 && !ig.otvetKati, () =>
        {
            ig.otvetKati = true;
            ig.nadezhda += 30;
            gorod.Razgovor(
                "Нина", "Виктор! Вам письмо. Вот, держите, и присядьте лучше.",
                "Катя", "Папа, я думала, что тебя больше нет. Мне всё равно, что случилось. Только не пропадай снова.",
                "Виктор", "Катюша. Я не пропаду.");
        }).zvuk = "bumaga";

        Mesto musorka3 = gorod.Novoe("Мусорка", 5240, 0, 24, false, Color.SeaGreen, "musorka");
        musorka3.Dobavit("Поискать бутылки", 1, vsegda, gorod.PoiskatVMusorke, true).zvuk = "musor";

        Mesto pasportniy = Zdanie(gorod, "Паспортный стол", 5040, 9, 17, Color.SlateGray, "klassika", "pasportniy");
        Rabotnik(gorod, pasportniy, "Ольга Сергеевна", Vneshnost.Zhenshina(Color.DimGray, Color.Black),
            "Следующий! Заявление, фото, пошлина. Всё остальное потом.",
            "Обед с часу до двух. Но вы по очереди, так что проходите.",
            "Паспорт, между прочим, нужен всем. Даже тем, кому кажется, что уже нет.");
        pasportniy.Dobavit("Подать заявление - 40 р.", 1, () => ig.dengi >= 40, () =>
        {
            ig.dengi -= 40;
            ig.denPasporta = ig.den + 3;
            gorod.Etap(7);
            gorod.Razgovor("Ольга Сергеевна", "Принято. Паспорт будет готов через три дня. Не теряйте талон.");
        }, false, 6).zvuk = "pechat";
        pasportniy.Dobavit("Забрать паспорт", 0.5, () => ig.den >= ig.denPasporta, () =>
        {
            gorod.Etap(8);
            ig.nadezhda += 15;
            gorod.Razgovor(
                "Ольга Сергеевна", "Распишитесь здесь. Держите. Берегите его.",
                "Виктор", "На фотографии усталый человек. Но это снова я.");
        }, false, 7).zvuk = "pechat";

        Mesto kafe = Zdanie(gorod, "Кафе", 5440, 10, 22, Color.Coral, "magazin", "kafe");
        Rabotnik(gorod, kafe, "Света", Vneshnost.Zhenshina(Color.Maroon, Color.FromArgb(230, 200, 120)),
            "Посудомойщица опять заболела. Поможете вечером? Заплачу и покормлю.",
            "Шеф у нас строгий, но добрый. Котлету за так не даст, а за работу две.",
            "Сегодня борщ. Хотите, налью погуще.");
        kafe.Dobavit("Горячий обед - 6 р.", 0.5, () => ig.dengi >= 6, () =>
        {
            ig.dengi -= 6;
            ig.eda += 50;
            ig.teplo += 10;
            gorod.Skazat("Борщ со сметаной. Почти как дома.");
        }).zvuk = "eda";
        kafe.Dobavit("Мыть посуду: 10 р. и ужин (нужно 25 сил)", 3, () => ig.sily >= 25, () =>
        {
            ig.dengi += 10;
            ig.eda += 30;
            ig.sily -= 15;
            gorod.Skazat("Гора тарелок побеждена. Руки пахнут лимоном.");
        }, true).zvuk = "posuda";

        Mesto apteka = Zdanie(gorod, "Аптека", 5840, 9, 21, Color.MediumSeaGreen, "magazin", "apteka");
        Rabotnik(gorod, apteka, "Марина", Vneshnost.Zhenshina(Color.WhiteSmoke, Color.FromArgb(40, 30, 30)),
            "Вы кашляете. Давно? Если станет хуже, сразу идите к врачу, ладно?",
            "Витамины не чудо, но зимой они правда помогают.",
            "Руки у вас замёрзли. Постойте у батареи, никто не выгонит.");
        apteka.Dobavit("Витамины и горячий настой - 4 р.", 0.25, () => ig.dengi >= 4, () =>
        {
            ig.dengi -= 4;
            ig.sily += 20;
            gorod.Skazat("Стало заметно бодрее.");
        }, true).zvuk = "tabletki";

        Mesto vokzal = Zdanie(gorod, "Вокзал", 6300, 0, 24, Color.Peru, "vokzal", "vokzal");
        Rabotnik(gorod, vokzal, "Игорь", Vneshnost.Muzhchina(Color.Navy, Color.Black, Color.FromArgb(226, 190, 160)),
            "Спать в зале нельзя. Но если сидеть ровно, я ничего не вижу.",
            "Сумку у тебя украли? Год назад? Мы тогда троих поймали, но вещей не нашли. Прости, брат.",
            "Поезд на Брест опаздывает. Как всегда, в общем.");
        vokzal.Dobavit("Попросить мелочь у прохожих", 1, vsegda, () =>
        {
            int summa = gorod.sluchay.Next(1, 6);
            ig.dengi += summa;
            ig.nadezhda -= 6;
            gorod.Skazat("Кто-то молча оставил " + summa + " р. Смотреть людям в глаза тяжело.");
        }, true).zvuk = "moneta";
        vokzal.Dobavit("Подремать на лавке (после 18:00)", 0, () => ig.chas >= 18, () =>
        {
            gorod.Usnut(35, 5, -5);
            gorod.Skazat("Сон урывками, но зато в тепле.");
        }).zvuk = "matras";

        Mesto ostanovka3 = gorod.Novoe("Промзона", 6520, 6, 23, false, Color.SteelBlue, "ostanovka");

        Mesto arenda = Zdanie(gorod, "Аренда жилья", 6760, 9, 19, Color.DarkSeaGreen, "panelka", "arenda");
        Rabotnik(gorod, arenda, "Дмитрий", Vneshnost.Muzhchina(Color.FromArgb(50, 55, 75), Color.FromArgb(40, 30, 25), Color.FromArgb(226, 190, 160)),
            "Комната есть. Триста рублей за первый месяц, паспорт обязательно.",
            "Хозяйка строгая, зато топят отлично. И до завода десять минут.",
            "Приходите с деньгами, сразу подпишем договор.");
        arenda.Dobavit("Снять комнату - 300 р.", 0.5, () => ig.dengi >= 300, gorod.PokazatPobedu, false, 10).zvuk = "klyuchi";

        Mesto musorka4 = gorod.Novoe("Мусорка", 7000, 0, 24, false, Color.SeaGreen, "musorka");
        musorka4.Dobavit("Поискать бутылки", 1, vsegda, gorod.PoiskatVMusorke, true).zvuk = "musor";

        Mesto garazh = Zdanie(gorod, "Автомастерская", 7160, 8, 19, Color.DarkOrange, "garazh", "garazh");
        Rabotnik(gorod, garazh, "Толик", Vneshnost.Muzhchina(Color.FromArgb(60, 75, 100), Color.Black, Color.FromArgb(226, 190, 160)),
            "Мотор перебрать поможешь? Руки у тебя, смотрю, из нужного места.",
            "Ты токарем был? Да ладно! Мне бы такого на постоянку.",
            "Деньги сразу, чай бесплатно, радио только плохое.");
        garazh.Dobavit("Помочь в гараже: 20 р. (нужно 40 сил)", 5, () => ig.sily >= 40, () =>
        {
            ig.dengi += 20;
            ig.sily -= 25;
            gorod.Skazat("Мотор завёлся с первого раза. Толик доволен.");
        }, true).zvuk = "instrument";

        Mesto zavod = Zdanie(gorod, "Завод", 7620, 8, 18, Color.Firebrick, "zavod", "zavod");
        Rabotnik(gorod, zavod, "Борис Андреевич", Vneshnost.Muzhchina(Color.FromArgb(55, 70, 85), Color.Gray, Color.FromArgb(226, 190, 160)),
            "Станки старые, зато надёжные. Как и те, кто на них работает.",
            "Смена с восьми. Опоздания не прощаю, хорошую работу не забываю.",
            "За двадцать лет ты ничего не забыл. Руки помнят.");
        zavod.Dobavit("Пройти собеседование", 1, () => ig.odezhda && ig.den - ig.denBani <= 1 && ig.nadezhda >= 30, () =>
        {
            gorod.Etap(10);
            ig.nadezhda += 20;
            gorod.Razgovor(
                "Борис Андреевич", "Вот чертёж, вот заготовка. Покажи, что умеешь.",
                "Рассказчик", "Станок загудел. Руки сами вспомнили работу, а стружка пошла ровной лентой.",
                "Борис Андреевич", "Двадцать лет опыта не пропьёшь. Завтра к восьми. Смена оплачивается каждый день.",
                "Виктор", "Спасибо. Я не подведу.");
        }, false, 9).zvuk = "stanok";
        zavod.Dobavit("Отработать смену: 40 р. и обед (нужно 30 сил)", 8, () => ig.sily >= 30, () =>
        {
            ig.dengi += 40;
            ig.sily -= 15;
            ig.eda += 30;
            gorod.Skazat("Гудок. Смена окончена. Устал, но это хорошая усталость.");
        }, true, 10).zvuk = "stanok";

        // Каждая остановка возит на две другие
        Mesto[] ostanovki = { ostanovka1, ostanovka2, ostanovka3 };
        foreach (Mesto otkuda in ostanovki)
        {
            foreach (Mesto kuda in ostanovki)
            {
                if (otkuda == kuda) continue;
                otkuda.Dobavit("Доехать: " + kuda.nazvanie + " - 1 р.", 0.25, () => ig.dengi >= 1, () => gorod.Proehat(kuda)).zvuk = "avtobus";
            }
        }
    }
}
