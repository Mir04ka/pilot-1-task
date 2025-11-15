using System.Text.Json;

namespace Pilot.Storage
{
    public class StorageService<T> where T : new()
    {
        private readonly string _filePath;     // File for storing game data
        private readonly object _lock = new(); // Locker for JSON read/write operations
        
        public StorageService(string filePath)
        {
            _filePath = filePath;
        }

        // Load data from file
        public T Load()
        {
            lock (_lock)
            {
                try
                {
                    if (!File.Exists(_filePath)) return new T();
                    
                    string json = File.ReadAllText(_filePath);
                    return JsonSerializer.Deserialize<T>(json) ?? new T();
                }
                catch
                {
                    return new T(); // On error returns empty leaderboard
                }
            }
        }
        
        // Save data to file
        public void Save(T data)
        {
            lock (_lock)
            {
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }); // Multiline JSON
                File.WriteAllText(_filePath, json);
            }
        }
    }
}