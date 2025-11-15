using Pilot.UI;
using Pilot.Tools;
using Pilot.Storage;

namespace Pilot.Game
{
    public class Engine
    {
        private readonly ConsoleUI _ui;
        private readonly LocalizationService _loc;
        private readonly StorageService<Dictionary<string, PlayerStats>> _storage;
        private readonly WordValidator _wordValidator;
        private readonly CommandsHandler _commandsHandler;
        private readonly object _locker = new();

        private readonly Dictionary<string, PlayerStats> _stats;
        private readonly GameState _gameState;
        private bool _finished;

        public event Action<string, string>? GameFinished; // <winner, loser>

        public Engine(
            ConsoleUI ui,
            LocalizationService loc,
            StorageService<Dictionary<string, PlayerStats>> storage)
        {
            _ui = ui;
            _loc = loc;
            _storage = storage;
            
            _wordValidator = new WordValidator();
            _commandsHandler =  new CommandsHandler(_ui, _loc);
            _stats = storage.Load();
            _gameState = new GameState();
        }
        
        public GameState State => _gameState;
        public Dictionary<string, PlayerStats> Stats => _stats;
        public bool Finished => _finished;

        public bool SetLanguage(string language)
        {
            return _loc.SetLanguage(language);
        }

        // Add player to game
        public void RegisterPlayer(int index, string name)
        {
            if (index == 1) _gameState.Player1.Name = name;
            else _gameState.Player2.Name = name;

            if (_stats.ContainsKey(name))
            {
                _ui.Print($"{name}:");
                _ui.Print($"{_stats[name].Wins} {_loc["wins_count"]}");
                _ui.Print($"{_stats[name].GamesPlayed} {_loc["games_count"]}");
            }
            else _stats[name] = new PlayerStats();
        }
        
        // Set first word
        public void SetInitialWord(string word)
        {
            _gameState.InitialWord = word.Trim().ToLowerInvariant();
            _gameState.EnteredWords.Add(_gameState.InitialWord);
        }

        // New word entered
        public bool TryProcessWord(string newWord)
        {
            newWord = newWord.Trim().ToLowerInvariant();
            
            if (_commandsHandler.TryHandle(newWord, _stats,  _gameState)) return true;
            
            var error = _wordValidator.IsValid(_gameState.InitialWord, newWord, _gameState.EnteredWords);

            switch (error)
            {
                case WordValidator.WordValidationError.None:
                    break;
                
                case WordValidator.WordValidationError.Repeated:
                    _ui.Print(_loc["new_word_repetition"]);
                    return false;
                
                case WordValidator.WordValidationError.WrongCharacters:
                    _ui.Print(_loc["new_word_characters_are_incorrect"]);
                    return false;
                
                default:
                    _ui.Print(_loc["unknown_error"]);
                    return false;
            }
            
            _gameState.EnteredWords.Add(newWord);
            
            _gameState.CurrentPlayer++;
            if (_gameState.CurrentPlayer > 2)
                _gameState.CurrentPlayer = 1;
            
            return true;
        }

        public void FinishGame()
        {
            lock (_locker)
            {
                if (_finished) return;

                _finished = true;

                string winner = _gameState.CurrentPlayer == 1 ? _gameState.Player2.Name : _gameState.Player1.Name;
                string loser = _gameState.CurrentPlayer == 1 ? _gameState.Player1.Name : _gameState.Player2.Name;

                _stats[winner].Wins++;
                _stats[winner].GamesPlayed++;
                _stats[loser].GamesPlayed++;

                _storage.Save(_stats);

                GameFinished?.Invoke(winner, loser);
            }
        }
    }
}
