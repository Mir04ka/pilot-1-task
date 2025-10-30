// Pilot-1 by Miraslau

using System.Timers;

class Program
{
    private const int MinCharCount = 8;   // Min characters count
    private const int MaxCharCount = 30;  // Max characters count
    private const int IntervalMs = 10000; // Time to input(ms)
    
    private static string _initWord = null!;                 // Initial word
    private static string _language = null!;                 // Language
    private static int _currentPlayer = 1;                   // Current player number
    private static List<string> _words = new List<string>(); // Entered words archive

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
    
    private static string? Localize(string key) // Returns localized text by key
    {
        return Localisation[_language].ContainsKey(key) ? Localisation[_language][key] : null;
    }

    private static void Print(string? text) // Output 
    {
        Console.WriteLine(text);
    }

    private static string Read() // Input
    {
        return Console.ReadLine()!;
    }

    // Checks, if word consists of the same letters as _initWord
    // true - the word is correct
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

    // Checks, if word was entered before
    // false - the word is correct(it doesn't repeats)
    private static bool IsRepeated(string word)
    {
        return _words.Contains(word);
    }

    private static void OnTimerElapsed(object sender, ElapsedEventArgs e) // Method emits when time is over
    {
        Print(Localize("time_elapsed"));
        Environment.Exit(0);
    }
    
    static void Main()
    {
        // Language choosing loop
        do
        {
            Print(Localisation["en"]["choose_language"]);
            _language = Read();
        } while (!Localisation.ContainsKey(_language));
        
        // Initial word input loop
        do 
        {
            Print(Localize("enter_init_word"));
            _initWord = Read();
            
            if (_initWord.Length < MinCharCount) Print(Localize("init_word_is_too_short"));
            if (_initWord.Length > MaxCharCount) Print(Localize("init_word_is_too_long"));
        } while (_initWord.Length < MinCharCount ||  _initWord.Length > MaxCharCount);
        
        _initWord = _initWord.Trim().ToLowerInvariant();
        _words.Add(_initWord);
        
        string newWord; 
        
        // Timer creation 
        System.Timers.Timer inputTimer = new  System.Timers.Timer(IntervalMs);
        inputTimer.Elapsed += OnTimerElapsed!;
        inputTimer.AutoReset = true;
        
        // Game main loop
        do
        {
            inputTimer.Start();
            
            Print(Localize("player") + _currentPlayer + Localize("enter_new_word"));
            newWord = Read();
            newWord = newWord.Trim().ToLowerInvariant();
            
            inputTimer.Stop();
            
            // End the game if the word repeats
            if (IsRepeated(newWord))
            {
                Print(Localize("new_word_repetition"));
                Print(Localize("player") + (_currentPlayer == 1 ? 2 : 1) + Localize("won"));
                Environment.Exit(0);
            } 
            
            _words.Add(newWord);
            _currentPlayer++;
            if (_currentPlayer > 2) _currentPlayer = 1;
        } while (CheckWordCharacters(newWord));
        
        // End the game if the word doesn't consists of initial word
        Print(Localize("new_word_characters_are_incorrect"));
        Print(Localize("player") + _currentPlayer + Localize("won"));
    }    
}