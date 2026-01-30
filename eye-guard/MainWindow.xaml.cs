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
            
            // 默认隐藏主窗口，仅在托盘运行
            this.Visibility = Visibility.Hidden;
            this.ShowInTaskbar = false;
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
            _timerManager.TimeWarning += (sender, e) =>
            {
                // 在UI线程中显示警告窗口
                Dispatcher.Invoke(() => ShowWarningWindow());
            };
            
            // 黑屏结束事件
            _screenController.BlackoutEnded += (sender, e) =>
            {
                // 在UI线程中执行定时器启动操作
                Dispatcher.Invoke(() => _timerManager.Start());
            };
            
            // 托盘图标事件
            _trayIcon.ShowMainWindow += (sender, e) => ShowWindow();
            _trayIcon.ExitApplication += (sender, e) => ShowExitConfirmationWindow();
            _trayIcon.PauseTimer += (sender, e) =>
            {
                _timerManager.AddTime(1); // 推迟1分钟
                ShowMessage("已经将休息时间推迟1分钟");
            };
            _trayIcon.ImmediateRest += (sender, e) =>
            {
                // 立即触发黑屏
                _screenController.StartBlackout();
                // 重置计时器
                _timerManager.Reset();
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
        
        private void ShowWarningWindow()
        {
            var warningWindow = new WarningWindow();
            
            // 处理立即休息事件
            warningWindow.ImmediateRestRequested += (sender, e) =>
            {
                _screenController.StartBlackout();
                _timerManager.Reset();
            };
            
            // 处理推迟休息事件
            warningWindow.DelayRestRequested += (sender, delayMinutes) =>
            {
                _timerManager.AddTime(delayMinutes);
                ShowMessage($"已经将休息时间推迟{delayMinutes}分钟");
            };
            
            // 开始倒计时
            int remainingWarningSeconds = _timerManager.RemainingWarningSeconds;
            warningWindow.StartCountdown(remainingWarningSeconds);
            
            // 显示窗口
            warningWindow.Show();
        }
        
        private void ShowWindow()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }
        
        private void ShowExitConfirmationWindow()
        {
            var exitWindow = new ExitConfirmationWindow();
            exitWindow.ExitConfirmed += (sender, option) => HandleExitOption(option);
            exitWindow.ShowDialog();
        }
        
        private void HandleExitOption(ExitConfirmationWindow.ExitOption option)
        {
            switch (option)
            {
                case ExitConfirmationWindow.ExitOption.ExitCompletely:
                    // 完全关闭退出
                    System.Windows.Application.Current.Shutdown();
                    break;
                case ExitConfirmationWindow.ExitOption.TemporaryExit10Minutes:
                    // 暂时关闭10分钟
                    _timerManager.Pause();
                    ShowMessage("眼睛护士已暂时关闭，10分钟后将自动恢复");
                    // 10分钟后自动恢复
                    System.Threading.Tasks.Task.Delay(TimeSpan.FromMinutes(10)).ContinueWith(t =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            _timerManager.Start();
                            ShowMessage("眼睛护士已自动恢复运行");
                        });
                    });
                    break;
                case ExitConfirmationWindow.ExitOption.TemporaryExit15Minutes:
                    // 暂时关闭15分钟
                    _timerManager.Pause();
                    ShowMessage("眼睛护士已暂时关闭，15分钟后将自动恢复");
                    // 15分钟后自动恢复
                    System.Threading.Tasks.Task.Delay(TimeSpan.FromMinutes(15)).ContinueWith(t =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            _timerManager.Start();
                            ShowMessage("眼睛护士已自动恢复运行");
                        });
                    });
                    break;
                case ExitConfirmationWindow.ExitOption.TemporaryExit20Minutes:
                    // 暂时关闭20分钟
                    _timerManager.Pause();
                    ShowMessage("眼睛护士已暂时关闭，20分钟后将自动恢复");
                    // 20分钟后自动恢复
                    System.Threading.Tasks.Task.Delay(TimeSpan.FromMinutes(20)).ContinueWith(t =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            _timerManager.Start();
                            ShowMessage("眼睛护士已自动恢复运行");
                        });
                    });
                    break;
                case ExitConfirmationWindow.ExitOption.TemporaryExit1Hour:
                    // 暂时关闭1小时
                    _timerManager.Pause();
                    ShowMessage("眼睛护士已暂时关闭，1小时后将自动恢复");
                    // 1小时后自动恢复
                    System.Threading.Tasks.Task.Delay(TimeSpan.FromHours(1)).ContinueWith(t =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            _timerManager.Start();
                            ShowMessage("眼睛护士已自动恢复运行");
                        });
                    });
                    break;
            }
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