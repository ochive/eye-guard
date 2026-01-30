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
            // For now, we'll just hide the dropdown if it's open
            if (DelayComboBox.Visibility == Visibility.Visible)
            {
                DelayComboBox.Visibility = Visibility.Collapsed;
            }
        }
        
        private void DelayButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle the visibility of the delay combo box
            DelayComboBox.Visibility = DelayComboBox.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }
        
        private void DelayComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DelayComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
            {
                if (int.TryParse(selectedItem.Tag.ToString(), out int delayMinutes))
                {
                    // Trigger delay rest event
                    DelayRestRequested?.Invoke(this, delayMinutes);
                    Close();
                }
            }
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