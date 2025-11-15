namespace Pilot.Game
{
    public class WordValidator
    {
        public enum WordValidationError
        {
            None,
            WrongCharacters,
            Repeated
        }
        
        // Checks, if new word consists of the same letters as initial word
        // true - the word is correct
        private static bool CheckWordCharacters(string initWord, string newWord) 
        {
            if (newWord.Length != initWord.Length) return false;

            List<char> charList = initWord.ToList();
            
            foreach (var c in newWord)
            {
                if (!charList.Remove(c)) return false;
            }
            
            return true;
        }

        public WordValidationError IsValid(string initWord, string newWord, List<string> history)
        {
            if (history.Contains(newWord)) return WordValidationError.Repeated; // Word was entered before

            if (!CheckWordCharacters(initWord, newWord)) return WordValidationError.WrongCharacters; // Letters are not the same as in initial word
            
            return WordValidationError.None; // Word is valid
        }
    }
}