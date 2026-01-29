using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using eye_guard.Core;
using eye_guard.UI;

namespace eye_guard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TimerManager _timerManager;
        private ScreenController _screenController;
        private TrayIcon _trayIcon;
        
        public MainWindow()
        {
            InitializeComponent();
            InitializeComponents();
            InitializeEventHandlers();
            AutoStartTimer();
        }
        
        private void InitializeComponents()
        {
            _timerManager = new TimerManager();
            _screenController = new ScreenController();
            _trayIcon = new TrayIcon();
        }
        
        private void InitializeEventHandlers()
        {
            // 按钮点击事件
            PauseButton.Click += (sender, e) =>
            {
                if (_timerManager.IsRunning)
                {
                    _timerManager.Pause();
                    ShowMessage("已暂停计时");
                }
                else
                {
                    _timerManager.Start();
                    ShowMessage("已继续计时");
                }
            };
            
            // 定时器事件
            _timerManager.TimerElapsed += (sender, e) =>
            {
                // 在UI线程中执行黑屏操作
                Dispatcher.Invoke(() => _screenController.StartBlackout());
            };
            _timerManager.TimeUpdated += (sender, e) =>
            {
                // 在UI线程中更新UI
                Dispatcher.Invoke(() => UpdateUI());
            };
            
            // 黑屏结束事件
            _screenController.BlackoutEnded += (sender, e) =>
            {
                // 在UI线程中执行定时器启动操作
                Dispatcher.Invoke(() => _timerManager.Start());
            };
            
            // 托盘图标事件
            _trayIcon.ShowMainWindow += (sender, e) => ShowWindow();
            _trayIcon.ExitApplication += (sender, e) => System.Windows.Application.Current.Shutdown();
            _trayIcon.StartTimer += (sender, e) =>
            {
                _timerManager.Start();
                ShowMessage("已开始计时");
            };
            _trayIcon.PauseTimer += (sender, e) =>
            {
                _timerManager.Pause();
                ShowMessage("已暂停计时");
            };
        }
        
        private void AutoStartTimer()
        {
            _timerManager.Start();
            ShowMessage("护眼软件已启动，开始计时");
        }
        
        private void UpdateUI()
        {
            // 更新剩余时间显示
            TimeRemainingText.Text = $"{_timerManager.RemainingMinutes:00}:{_timerManager.RemainingSeconds:00}";
            
            // 更新暂停按钮文本
            PauseButton.Content = _timerManager.IsRunning ? "暂停" : "继续";
            
            // 更新托盘图标状态
            string status = _timerManager.IsRunning ? "运行中" : "已暂停";
            _trayIcon.UpdateStatus(status);
        }
        
        private void ShowMessage(string message)
        {
            System.Windows.MessageBox.Show(message, "护眼软件", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void ShowWindow()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }
        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // 最小化到托盘，不真正关闭
            e.Cancel = true;
            this.Hide();
        }
        
        protected override void OnClosed(EventArgs e)
        {
            // 清理资源
            _timerManager.Dispose();
            _screenController.Dispose();
            _trayIcon.Dispose();
            
            base.OnClosed(e);
        }
    }
}