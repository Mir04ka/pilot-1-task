namespace Pilot.Tools
{
    public class LocalizationService
    {
        private string _language = "en"; // Current language

        private readonly Dictionary<string, Dictionary<string, string>> _localization;

        public LocalizationService(Dictionary<string, Dictionary<string, string>> localization)
        {
            _localization = localization;
        }
        
        // Returns true if language has set successfully
        public bool SetLanguage(string language)
        {
            if (_localization.ContainsKey(language))
            {
                _language = language;
                return true;
            }

            return false;
        }
        
        public string this[string key] => _localization[_language][key]; // Get localized text by key
    }
}