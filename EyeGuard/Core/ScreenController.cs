using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;

namespace EyeGuard.Core
{
    public class ScreenController
    {
        private List<Window> _blackoutWindows;
        private List<TextBlock> _countdownTexts;
        private DispatcherTimer _blackoutTimer;
        private int _remainingSeconds;
        
        public event EventHandler BlackoutEnded;
        
        public bool IsBlackoutActive { get; private set; }
        public int RemainingSeconds => _remainingSeconds;
        
        public ScreenController()
        {
            _blackoutWindows = new List<Window>();
            _countdownTexts = new List<TextBlock>();
            _blackoutTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _blackoutTimer.Tick += OnBlackoutTimerTick;
        }
        
        public void StartBlackout()
        {
            if (IsBlackoutActive)
                return;
            
            IsBlackoutActive = true;
            _remainingSeconds = AppSettings.BlackoutDurationSeconds;
            
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
            _countdownTexts.Clear();
            
            IsBlackoutActive = false;
            BlackoutEnded?.Invoke(this, EventArgs.Empty);
        }
        
        private Window CreateBlackoutWindow(System.Windows.Forms.Screen screen)
        {
            var textBlock = new TextBlock
            {
                Text = $"{_remainingSeconds}",
                FontSize = 100,
                Foreground = System.Windows.Media.Brushes.White,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };
            
            var window = new Window
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = System.Windows.Media.Brushes.Black,
                Topmost = true,
                Left = screen.Bounds.Left,
                Top = screen.Bounds.Top,
                Width = screen.Bounds.Width,
                Height = screen.Bounds.Height,
                Content = textBlock
            };
            
            _countdownTexts.Add(textBlock);
            return window;
        }
        
        private void OnBlackoutTimerTick(object sender, EventArgs e)
        {
            _remainingSeconds--;
            
            // 更新所有窗口的倒计时文本
            foreach (var textBlock in _countdownTexts)
            {
                textBlock.Text = $"{_remainingSeconds}";
            }
            
            if (_remainingSeconds <= 0)
            {
                EndBlackout();
            }
        }
        
        public void Dispose()
        {
            _blackoutTimer?.Stop();
            EndBlackout();
        }
    }
}