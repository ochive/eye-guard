using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

namespace eye_guard.Core
{
    public class ScreenController
    {
        private List<Window> _blackoutWindows;
        private System.Windows.Forms.Timer _blackoutTimer;
        private const int BLACKOUT_DURATION_SECONDS = 60;
        private int _remainingSeconds;
        
        public event EventHandler BlackoutEnded;
        
        public bool IsBlackoutActive { get; private set; }
        public int RemainingSeconds => _remainingSeconds;
        
        public ScreenController()
        {
            _blackoutWindows = new List<Window>();
            _blackoutTimer = new System.Windows.Forms.Timer();
            _blackoutTimer.Interval = 1000; // 1秒间隔
            _blackoutTimer.Tick += OnBlackoutTimerTick;
        }
        
        public void StartBlackout()
        {
            if (IsBlackoutActive)
                return;
            
            IsBlackoutActive = true;
            _remainingSeconds = BLACKOUT_DURATION_SECONDS;
            
            // 为每个显示器创建黑屏窗口
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                var window = CreateBlackoutWindow(screen);
                _blackoutWindows.Add(window);
                window.Show();
            }
            
            _blackoutTimer.Start();
        }
        
        public void EndBlackout()
        {
            if (!IsBlackoutActive)
                return;
            
            _blackoutTimer.Stop();
            
            // 关闭所有黑屏窗口
            foreach (var window in _blackoutWindows)
            {
                window.Close();
            }
            _blackoutWindows.Clear();
            
            IsBlackoutActive = false;
            BlackoutEnded?.Invoke(this, EventArgs.Empty);
        }
        
        private Window CreateBlackoutWindow(System.Windows.Forms.Screen screen)
        {
            var window = new Window
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = System.Windows.Media.Brushes.Black,
                Topmost = true,
                Left = screen.Bounds.Left,
                Top = screen.Bounds.Top,
                Width = screen.Bounds.Width,
                Height = screen.Bounds.Height
            };
            
            return window;
        }
        
        private void OnBlackoutTimerTick(object sender, EventArgs e)
        {
            _remainingSeconds--;
            
            if (_remainingSeconds <= 0)
            {
                EndBlackout();
            }
        }
        
        public void Dispose()
        {
            _blackoutTimer?.Dispose();
            EndBlackout();
        }
    }
}