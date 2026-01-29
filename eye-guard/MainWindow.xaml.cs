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
        private DispatcherTimer _uiUpdateTimer;
        
        public MainWindow()
        {
            InitializeComponent();
            InitializeComponents();
            InitializeEventHandlers();
            InitializeUpdateTimer();
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
            StartButton.Click += (sender, e) => _timerManager.Start();
            PauseButton.Click += (sender, e) => _timerManager.Pause();
            
            // 定时器事件
            _timerManager.TimerElapsed += (sender, e) => _screenController.StartBlackout();
            
            // 黑屏结束事件
            _screenController.BlackoutEnded += (sender, e) => _timerManager.Start();
            
            // 托盘图标事件
            _trayIcon.ShowMainWindow += (sender, e) => ShowWindow();
            _trayIcon.ExitApplication += (sender, e) => System.Windows.Application.Current.Shutdown();
            _trayIcon.StartTimer += (sender, e) => _timerManager.Start();
            _trayIcon.PauseTimer += (sender, e) => _timerManager.Pause();
        }
        
        private void InitializeUpdateTimer()
        {
            _uiUpdateTimer = new DispatcherTimer();
            _uiUpdateTimer.Interval = TimeSpan.FromSeconds(1);
            _uiUpdateTimer.Tick += (sender, e) => UpdateUI();
            _uiUpdateTimer.Start();
        }
        
        private void UpdateUI()
        {
            // 更新剩余时间显示
            TimeRemainingText.Text = $"{_timerManager.RemainingMinutes}:00";
            
            // 更新托盘图标状态
            string status = _timerManager.IsRunning ? "运行中" : "已暂停";
            _trayIcon.UpdateStatus(status);
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
            _uiUpdateTimer.Stop();
            
            base.OnClosed(e);
        }
    }
}