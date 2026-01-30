using System;
using System.Timers;

namespace eye_guard.Core
{
    public class TimerManager
    {
        private System.Timers.Timer _timer;
        private int _remainingMinutes;
        private int _remainingSeconds;
        private const int INTERVAL_MINUTES = 60;
        private const int INTERVAL_SECONDS = INTERVAL_MINUTES * 60;
        private const int WARNING_MINUTES = 1;
        private const int WARNING_SECONDS = WARNING_MINUTES * 60;
        
        public event EventHandler TimerElapsed;
        public event EventHandler TimeUpdated;
        public event EventHandler TimeWarning;
        
        public bool IsRunning { get; private set; }
        public bool IsInWarningPeriod { get; private set; }
        public int RemainingMinutes => _remainingMinutes;
        public int RemainingSeconds => _remainingSeconds;
        public int RemainingWarningSeconds => (_remainingMinutes * 60) + _remainingSeconds;
        
        public TimerManager()
        {
            _timer = new System.Timers.Timer(1000); // 1秒间隔
            _timer.Elapsed += OnTimerElapsed;
            Reset();
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
            _remainingSeconds = 0;
            IsInWarningPeriod = false;
        }
        
        public void AddTime(int minutes)
        {
            _remainingMinutes += minutes;
            IsInWarningPeriod = false; // Reset warning state when adding time
            TimeUpdated?.Invoke(this, EventArgs.Empty);
        }
        
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_remainingSeconds > 0)
            {
                _remainingSeconds--;
            }
            else if (_remainingMinutes > 0)
            {
                _remainingMinutes--;
                _remainingSeconds = 59;
            }
            
            // Calculate total remaining seconds
            int totalRemainingSeconds = (_remainingMinutes * 60) + _remainingSeconds;
            
            // Check if we've entered the warning period
            if (totalRemainingSeconds <= WARNING_SECONDS && !IsInWarningPeriod)
            {
                IsInWarningPeriod = true;
                TimeWarning?.Invoke(this, EventArgs.Empty);
            }
            
            // 触发时间更新事件
            TimeUpdated?.Invoke(this, EventArgs.Empty);
            
            // 当时间结束时触发计时结束事件
            if (_remainingMinutes <= 0 && _remainingSeconds <= 0)
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