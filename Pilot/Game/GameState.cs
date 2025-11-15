namespace Pilot.Game
{
    public class GameState
    {
        public string InitialWord { get; set; } = string.Empty; // First game word
        public int CurrentPlayer { get; set; } = 1;             // Whose turn
        public List<string> EnteredWords { get; set; } = new(); // History of entered words
        public Player Player1 { get; set; } = new();            // Player 1 
        public Player Player2 { get; set; } = new();            // Player 2
    }
}