using System;
using System.Timers;

namespace eye_guard.Core
{
    public class TimerManager
    {
        private System.Timers.Timer _timer;
        private int _remainingMinutes;
        private const int INTERVAL_MINUTES = 30;
        
        public event EventHandler TimerElapsed;
        
        public bool IsRunning { get; private set; }
        public int RemainingMinutes => _remainingMinutes;
        
        public TimerManager()
        {
            _timer = new System.Timers.Timer(60000); // 1分钟间隔
            _timer.Elapsed += OnTimerElapsed;
            _remainingMinutes = INTERVAL_MINUTES;
        }
        
        public void Start()
        {
            if (!IsRunning)
            {
                _timer.Start();
                IsRunning = true;
            }
        }
        
        public void Pause()
        {
            if (IsRunning)
            {
                _timer.Stop();
                IsRunning = false;
            }
        }
        
        public void Reset()
        {
            Pause();
            _remainingMinutes = INTERVAL_MINUTES;
        }
        
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _remainingMinutes--;
            
            if (_remainingMinutes <= 0)
            {
                Pause();
                TimerElapsed?.Invoke(this, EventArgs.Empty);
                Reset();
            }
        }
        
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}