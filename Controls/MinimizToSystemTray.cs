using System;
using System.Windows;
using System.Windows.Forms;

namespace BToolkitForWPF.Controls
{
    /// <summary>
    /// 最小化到系统托盘
    /// </summary>
    class MinimizToSystemTray
    {
        private static NotifyIcon notifyIcon;
        private static Window currHideWindow;

        ///以下实现最小化到系统托盘
        public static void Init(Window window)
        {
            if (notifyIcon == null)
            {
                currHideWindow = window;

                notifyIcon = new NotifyIcon();
                notifyIcon.BalloonTipText = "系统监控中... ...";
                notifyIcon.ShowBalloonTip(2000);
                notifyIcon.Text = "系统监控中... ...";
                //this.notifyIcon.Icon = new System.Drawing.Icon(@"AppIcon.ico");
                notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
                notifyIcon.Visible = true;
                //打开菜单项
                MenuItem open = new MenuItem("打开");
                open.Click += new EventHandler(Show);
                //退出菜单项
                MenuItem exit = new MenuItem("退出");
                exit.Click += new EventHandler(Close);
                //关联托盘控件
                MenuItem[] childen = new MenuItem[] { open, exit };
                notifyIcon.ContextMenu = new ContextMenu(childen);

                notifyIcon.MouseClick += new MouseEventHandler((o, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        Show(o, e);
                    }
                });

                //监听程序退出
                AppDomain.CurrentDomain.ProcessExit += (object sender, EventArgs e) =>
                {
                    if (notifyIcon != null)
                    {
                        notifyIcon.Dispose();
                        notifyIcon = null;
                    }
                };
            }
        }

        /// <summary>
        /// 外部调用
        /// </summary>
        public static void Hide()
        {
            if (currHideWindow == null)
            {
                Console.WriteLine("为调动Init()进行初始化");
                return;
            }
            currHideWindow.ShowInTaskbar = false;
            currHideWindow.Hide();
        }

        private static void Show(object sender, EventArgs e)
        {
            if (currHideWindow == null)
            {
                Console.WriteLine("为调动Init()进行初始化");
                return;
            }
            currHideWindow.ShowInTaskbar = true;
            currHideWindow.Show();
            currHideWindow.Activate();
        }

        private static void Close(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
                notifyIcon = null;
            }
        }
    }
}
