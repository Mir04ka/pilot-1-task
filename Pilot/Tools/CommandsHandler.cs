using Pilot.Game;
using Pilot.UI;

namespace Pilot.Tools
{
    public class CommandsHandler
    {
        private readonly ConsoleUI _ui;
        private readonly LocalizationService _loc;

        public CommandsHandler(ConsoleUI ui, LocalizationService loc)
        {
            _ui = ui;
            _loc = loc;
        }
        
        // Returns false if the command is unknown
        public bool TryHandle(string command, Dictionary<string, PlayerStats> stats, GameState gameState)
        {
            switch (command)
            {
                // Prints history of entered words
                case "/show-words":
                    _ui.Print(_loc["all_words"]);
                    foreach (var word in gameState.EnteredWords)
                    {
                        _ui.Print(word);
                    }
                    break;
                
                // Prints current players scores
                case "/score":
                    _ui.Print(_loc["these_players_score"]);
                    
                    _ui.Print($"{gameState.Player1.Name}:");
                    _ui.Print($"{stats[gameState.Player1.Name].Wins} {_loc["wins_count"]}");
                    _ui.Print($"{stats[gameState.Player1.Name].GamesPlayed} {_loc["games_count"]}");
                    
                    _ui.Print($"{gameState.Player2.Name}:");
                    _ui.Print($"{stats[gameState.Player2.Name].Wins} {_loc["wins_count"]}");
                    _ui.Print($"{stats[gameState.Player2.Name].GamesPlayed} {_loc["games_count"]}");
                    break;
                
                // Prints all players scores
                case "/total-score":
                    _ui.Print(_loc["all_players_score"]);
                    foreach (var player in stats.Keys)
                    {
                        _ui.Print($"\n{player}:");
                        _ui.Print($"{stats[player].Wins} {_loc["wins_count"]}");
                        _ui.Print($"{stats[player].GamesPlayed} {_loc["games_count"]}");
                    }
                    break;
                
                // The word is not a command
                default:
                    return false;
            }
            
            return true;
        }
    }
}