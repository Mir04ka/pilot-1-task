// Pilot-1 by Miraslau

using Pilot.Game;
using Pilot.Storage;
using Pilot.Tools;
using Pilot.UI;

namespace Pilot
{
    class Program
    {   
        private const int MinCharCount = 8;                // Min characters count
        private const int MaxCharCount = 30;               // Max characters count
        private const int IntervalMs = 30000;              // Time to input(ms)
        private const string SaveFilePath = "scores.json"; // Saves file
        
        static void Main()
        {
            var ui = new ConsoleUI();

            var loc = new LocalizationService(new()
                {
                    ["ru"] = new Dictionary<string, string>
                    {
                        ["enter_init_word"] = "Введите начальное слово: ",
                        ["init_word_is_too_short"] = $"Начальное слово слишком короткое(минимум {MinCharCount} символов)",
                        ["init_word_is_too_long"] = $"Начальное слово слишком длинное(максимум {MaxCharCount} символов)",
                        ["player"] = "Игрок ",
                        ["enter_new_word"] = ", введите новое слово: ",
                        ["new_word_characters_are_incorrect"] = "\nСлово не состоит из тех же букв!",
                        ["new_word_repetition"] = "\nЭто слово уже было ранее!",
                        ["time_elapsed"] = "\nВремя вышло!",
                        ["won"] = " победил!",
                        ["enter_name"] = "Введите имя игрока ",
                        ["wins_count"] = " побед(а)",
                        ["games_count"] = " игр(а) всего",
                        ["all_words"] = "Все введенные слова текущей игры: ",
                        ["these_players_score"] = "Общий счет по играм текущих игроков: ",
                        ["all_players_score"] = "Счет всех игроков: ",
                    },
                    ["en"] = new Dictionary<string, string>
                    {
                        ["enter_init_word"] = "Enter initial word: ",
                        ["init_word_is_too_short"] = $"Initial word is too short(at least {MinCharCount} characters)",
                        ["init_word_is_too_long"] = $"Initial word is too long(maximum {MaxCharCount} characters)",
                        ["player"] = "Player ",
                        ["enter_new_word"] = ", enter new word: ",
                        ["new_word_characters_are_incorrect"] = "\nThe word doesn't consist of the same characters!",
                        ["new_word_repetition"] = "\nThis word repeats!",
                        ["time_elapsed"] = "\nTime elapsed!",
                        ["won"] = " won!",
                        ["enter_name"] = "Enter name of player ",
                        ["wins_count"] = " win(s)",
                        ["games_count"] = " game(s) total",
                        ["all_words"] = "All entered words of current game: ",
                        ["these_players_score"] = "Current players score: ",
                        ["all_players_score"] = "All players scores: ",
                    }
                }
            );

            var storage = new StorageService<Dictionary<string, PlayerStats>>(SaveFilePath);
            
            var engine = new Engine(ui, loc, storage);
            
            var timer = new GameTimer(IntervalMs);

            // On time elapsed
            timer.Timeout += () =>
            {
                ui.Print(loc["time_elapsed"]);
                engine.FinishGame();
            };
            
            // On in-game exit
            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                engine.FinishGame();
            };

            // On game finished
            engine.GameFinished += (winner, loser) =>
            {
                ui.Print(winner + loc["won"]);
                Environment.Exit(0);
            };
            
            // Language choosing loop
            string lang;
            do
            {
                ui.Print("Choose language(en or ru): ");
                lang = ui.Read();
            } while (!engine.SetLanguage(lang));

            // Player 1 name input loop
            string playerName;
            do
            {
                ui.Print(loc["enter_name"] + "1:");
                playerName = ui.Read().Trim();
            } while (string.IsNullOrEmpty(playerName));
            
            engine.RegisterPlayer(1, playerName);

            // Player 2 name input loop
            do
            {
                ui.Print(loc["enter_name"] + "2:");
                playerName = ui.Read().Trim();
            } while (string.IsNullOrEmpty(playerName));
            
            engine.RegisterPlayer(2, playerName);
            
            // Initial word input loop
            string initialWord;
            do 
            {
                ui.Print(loc["enter_init_word"]);
                initialWord = ui.Read().Trim().ToLowerInvariant();
                
                if (initialWord.Length < MinCharCount) ui.Print(loc["init_word_is_too_short"]);
                if (initialWord.Length > MaxCharCount) ui.Print(loc["init_word_is_too_long"]);
            } while (initialWord.Length < MinCharCount ||  initialWord.Length > MaxCharCount);
            
            engine.SetInitialWord(initialWord);

            while (!engine.Finished)
            {
                timer.Start();

                ui.Print(loc["player"] +
                         (engine.State.CurrentPlayer == 1 ? engine.State.Player1.Name : engine.State.Player2.Name) +
                         loc["enter_new_word"]);
                string newWord = ui.Read();
                
                timer.Stop();

                if (!engine.TryProcessWord(newWord)) break;
            }
            
            engine.FinishGame();
        }    
    }
}