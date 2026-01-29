using System;
using System.Windows.Forms;

namespace eye_guard.UI
{
    public class TrayIcon : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;
        
        public event EventHandler ShowMainWindow;
        public event EventHandler ExitApplication;
        public event EventHandler PauseTimer;
        public event EventHandler ImmediateRest;
        
        public TrayIcon()
        {
            InitializeTrayIcon();
            InitializeContextMenu();
        }
        
        private void InitializeTrayIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Text = "护眼软件",
                Visible = true
            };
            
            // 设置默认图标
            _notifyIcon.Icon = System.Drawing.SystemIcons.Application;
            
            // 双击托盘图标显示主窗口
            _notifyIcon.DoubleClick += (sender, e) => ShowMainWindow?.Invoke(this, EventArgs.Empty);
        }
        
        private void InitializeContextMenu()
        {
            _contextMenu = new ContextMenuStrip();
            
            // 添加菜单项
            var showMainWindowItem = _contextMenu.Items.Add("显示主窗口");
            showMainWindowItem.Click += (sender, e) => ShowMainWindow?.Invoke(this, EventArgs.Empty);
            
            _contextMenu.Items.Add(new ToolStripSeparator());
            
            var pauseItem = _contextMenu.Items.Add("推迟休息");
            pauseItem.Click += (sender, e) => PauseTimer?.Invoke(this, EventArgs.Empty);
            
            var immediateRestItem = _contextMenu.Items.Add("立即休息");
            immediateRestItem.Click += (sender, e) => ImmediateRest?.Invoke(this, EventArgs.Empty);
            
            _contextMenu.Items.Add(new ToolStripSeparator());
            
            var exitItem = _contextMenu.Items.Add("退出");
            exitItem.Click += (sender, e) => ExitApplication?.Invoke(this, EventArgs.Empty);
            
            _notifyIcon.ContextMenuStrip = _contextMenu;
        }
        
        public void UpdateStatus(string status)
        {
            _notifyIcon.Text = $"护眼软件 - {status}";
        }
        
        public void Dispose()
        {
            _notifyIcon?.Dispose();
            _contextMenu?.Dispose();
        }
    }
}