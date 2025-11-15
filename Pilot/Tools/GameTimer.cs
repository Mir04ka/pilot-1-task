using System.Timers;

namespace Pilot.Tools
{
    public class GameTimer
    {
        private System.Timers.Timer _timer;

        public event Action? Timeout; 

        public GameTimer(double intervalMs)
        {
            _timer = new System.Timers.Timer(intervalMs);
            _timer.Elapsed += (_, _) => Timeout?.Invoke();
            _timer.AutoReset = true;
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
    }
}