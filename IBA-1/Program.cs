// Pilot-1 by Miraslau

using System.Timers;

class Program
{
    private const int MinCharCount = 8;   // Минимальное количество символов
    private const int MaxCharCount = 30;  // Максимальное количество символов
    private const int IntervalMs = 10000; // Время на ввод слова(в мс)
    
    private static string _initWord = null!;                 // Начальное слово
    private static string _language = null!;                 // Язык
    private static int _currentPlayer = 1;                   // Номер игрока, который должен ввести слово
    private static List<string> _words = new List<string>(); // Все введенные слова

    private static readonly Dictionary<string, Dictionary<string, string>> Localisation = new()
    {
        ["ru"] = new Dictionary<string, string>
        {
            ["choose_language"] = "Выберите язык(en или ru): ",
            ["enter_init_word"] = "Введите начальное слово: ",
            ["init_word_is_too_short"] = $"Начальное слово слишком короткое(минимум {MinCharCount} символов)",
            ["init_word_is_too_long"] = $"Начальное слово слишком длинное(максимум {MaxCharCount} символов)",
            ["player"] = "Игрок ",
            ["enter_new_word"] = ", введите новое слово: ",
            ["new_word_characters_are_incorrect"] = "\nСлово не состоит из тех же букв!",
            ["new_word_repetition"] = "\nЭто слово уже было ранее!",
            ["time_elapsed"] = "\nВремя вышло!",
            ["won"] = " победил!",
        },
        ["en"] = new Dictionary<string, string>
        {
            ["choose_language"] = "Choose language(en or ru): ",
            ["enter_init_word"] = "Enter initial word: ",
            ["init_word_is_too_short"] = $"Initial word is too short(at least {MinCharCount} characters)",
            ["init_word_is_too_long"] = $"Initial word is too long(maximum {MaxCharCount} characters)",
            ["player"] = "Player ",
            ["enter_new_word"] = ", enter new word: ",
            ["new_word_characters_are_incorrect"] = "\nThe word doesn't consist of the same characters!",
            ["new_word_repetition"] = "\nThis word repeats!",
            ["time_elapsed"] = "\nTime elapsed!",
            ["won"] = " won!",
        }
    };
    
    private static string? Localize(string key) // Возвращает локализированный текст по ключу
    {
        return Localisation[_language].ContainsKey(key) ? Localisation[_language][key] : null;
    }

    // Проверка, что слово состоит из тех же букв, что и _initWord
    // true - слово корректно
    private static bool CheckWordCharacters(string word) 
    {
        if (word.Length != _initWord.Length) return false;

        List<char> charList = _initWord.ToList();
        
        foreach (var c in word)
        {
            if (!charList.Remove(c)) return false;
        }
        
        return true;
    }

    // Проверка, что слово не повторяется
    // false - слово корректно(не повторяется)
    private static bool IsRepeats(string word)
    {
        return _words.Contains(word);
    }

    private static void OnTimerElapsed(object sender, ElapsedEventArgs e) // Метод вызывается при истечении времени
    {
        Console.WriteLine(Localize("time_elapsed"));
        Environment.Exit(0);
    }
    
    static void Main()
    {
        // Цикл выбора языка
        do
        {
            Console.WriteLine(Localisation["en"]["choose_language"]);
            _language = Console.ReadLine()!;
        } while (!Localisation.ContainsKey(_language));
        
        // Цикл ввода начального слова
        do 
        {
            Console.WriteLine(Localize("enter_init_word"));
            _initWord = Console.ReadLine()!;
            
            if (_initWord.Length < MinCharCount) Console.WriteLine(Localize("init_word_is_too_short"));
            if (_initWord.Length > MaxCharCount) Console.WriteLine(Localize("init_word_is_too_long"));
        } while (_initWord.Length < MinCharCount ||  _initWord.Length > MaxCharCount);
        
        _initWord = _initWord.Trim().ToLowerInvariant();
        _words.Add(_initWord);
        
        string newWord; // Вводимое игроками слово
        
        // Создание таймера
        System.Timers.Timer inputTimer = new  System.Timers.Timer(IntervalMs);
        inputTimer.Elapsed += OnTimerElapsed!;
        inputTimer.AutoReset = true;
        
        // Основной цикл игры
        do
        {
            inputTimer.Start();
            
            Console.WriteLine(Localize("player") + _currentPlayer + Localize("enter_new_word"));
            newWord = Console.ReadLine()!;
            newWord = newWord.Trim().ToLowerInvariant();
            
            inputTimer.Stop();
            
            // Если слово повторяется, завершаем игру
            if (IsRepeats(newWord))
            {
                Console.WriteLine(Localize("new_word_repetition"));
                Console.WriteLine(Localize("player") + (_currentPlayer == 1 ? 2 : 1) + Localize("won"));
                Environment.Exit(0);
            } 
            
            _words.Add(newWord);
            _currentPlayer++;
            if (_currentPlayer > 2) _currentPlayer = 1;
        } while (CheckWordCharacters(newWord));
        
        // Если слово не состоит из символов начального слова, завершаем игру
        Console.WriteLine(Localize("new_word_characters_are_incorrect"));
        Console.WriteLine(Localize("player") + _currentPlayer + Localize("won"));
    }    
}