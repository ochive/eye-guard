using System;
using System.Windows;
using System.Windows.Threading;

namespace eye_guard
{
    public partial class WarningWindow : Window
    {
        private DispatcherTimer _countdownTimer;
        private int _remainingSeconds;
        
        public event EventHandler ImmediateRestRequested;
        public event EventHandler<int> DelayRestRequested;
        
        public WarningWindow()
        {
            InitializeComponent();
            InitializeTimer();
        }
        
        public void StartCountdown(int seconds)
        {
            _remainingSeconds = seconds;
            UpdateCountdownText();
            _countdownTimer.Start();
        }
        
        private void InitializeTimer()
        {
            _countdownTimer = new DispatcherTimer();
            _countdownTimer.Interval = TimeSpan.FromSeconds(1);
            _countdownTimer.Tick += OnCountdownTick;
        }
        
        private void OnCountdownTick(object sender, EventArgs e)
        {
            if (_remainingSeconds > 0)
            {
                _remainingSeconds--;
                UpdateCountdownText();
            }
            else
            {
                _countdownTimer.Stop();
                // Time's up, trigger immediate rest
                ImmediateRestRequested?.Invoke(this, EventArgs.Empty);
                Close();
            }
        }
        
        private void UpdateCountdownText()
        {
            CountdownText.Text = $"离休息还剩余时间: {_remainingSeconds}秒";
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close button should not close the window, just hide it
            // But according to the requirement, we should respect user's choice
            // For now, we'll just hide any open context menus
            if (ActionButton.ContextMenu != null && ActionButton.ContextMenu.IsOpen)
            {
                ActionButton.ContextMenu.IsOpen = false;
            }
        }
        
        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the context menu when action button is clicked
            ActionButton.ContextMenu.IsOpen = true;
        }
        
        private void ImmediateRestMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Trigger immediate rest event
            ImmediateRestRequested?.Invoke(this, EventArgs.Empty);
            Close();
        }
        
        private void Delay3MinutesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Trigger delay rest event with 3 minutes
            DelayRestRequested?.Invoke(this, 3);
            Close();
        }
        
        private void Delay5MinutesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Trigger delay rest event with 5 minutes
            DelayRestRequested?.Invoke(this, 5);
            Close();
        }
        
        private void Delay8MinutesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Trigger delay rest event with 8 minutes
            DelayRestRequested?.Invoke(this, 8);
            Close();
        }
        
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Allow dragging the window
            DragMove();
        }
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _countdownTimer?.Stop();
            base.OnClosing(e);
        }
    }
}