using BToolkitForWPF.Controls;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BToolkitForWPF.Controls
{
    /// <summary>
    /// 功能：无边框+四边拖拉缩放
    /// 配合NoBorderWindow.cs类使用（主窗口继承自该类）
    /// <Window x:Class=
    /// 换成
    /// <btoolkit:NoBorderWindow x:Class=
    /// </summary>
    public class NoBorderWindow : Window
    {
        /// <summary>
        /// 是否允许拖拉窗口大小
        /// </summary>
        public bool canDragResize = true;
        public const int WM_SYSCOMMAND = 0x112;
        public HwndSource HwndSource;
        public Dictionary<ResizeDirection, Cursor> cursors = new Dictionary<ResizeDirection, Cursor>
        {
            {ResizeDirection.Top, Cursors.SizeNS},
            {ResizeDirection.Bottom, Cursors.SizeNS},
            {ResizeDirection.Left, Cursors.SizeWE},
            {ResizeDirection.Right, Cursors.SizeWE},
            {ResizeDirection.TopLeft, Cursors.SizeNWSE},
            {ResizeDirection.BottomRight, Cursors.SizeNWSE},
            {ResizeDirection.TopRight, Cursors.SizeNESW},
            {ResizeDirection.BottomLeft, Cursors.SizeNESW}
        };

        public enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public NoBorderWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.SourceInitialized += delegate (object sender, EventArgs e)
            {
                this.HwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            };
            this.Loaded += VcreditWindowBehindCode_Loaded;
            this.MouseMove += VcreditWindowBehindCode_MouseMove;
        }

        /// <summary>
        /// 设置拖动时光标状态
        /// </summary>
        void VcreditWindowBehindCode_MouseMove(object sender, MouseEventArgs e)
        {
            if (canDragResize)
            {
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                {
                    FrameworkElement element = e.OriginalSource as FrameworkElement;
                    if (element != null && !element.Name.Contains("Resize"))
                    {
                        this.Cursor = Cursors.Arrow;
                    }
                }
            }
        }

        /// <summary>
        /// 绑定拖动事件
        /// </summary>
        void VcreditWindowBehindCode_Loaded(object sender, RoutedEventArgs e)
        {
            ControlTemplate customWindowTemplate = Application.Current.Resources["NoBorderWindowTemplete"] as ControlTemplate;
            if (customWindowTemplate != null)
            {
                var TopLeft = customWindowTemplate.FindName("ResizeTopLeft", this) as Rectangle;
                TopLeft.MouseMove += ResizePressed;
                TopLeft.MouseDown += ResizePressed;
                var Top = customWindowTemplate.FindName("ResizeTop", this) as Rectangle;
                Top.MouseMove += ResizePressed;
                Top.MouseDown += ResizePressed;
                var TopRight = customWindowTemplate.FindName("ResizeTopRight", this) as Rectangle;
                TopRight.MouseMove += ResizePressed;
                TopRight.MouseDown += ResizePressed;
                var Left = customWindowTemplate.FindName("ResizeLeft", this) as Rectangle;
                Left.MouseMove += ResizePressed;
                Left.MouseDown += ResizePressed;
                var Right = customWindowTemplate.FindName("ResizeRight", this) as Rectangle;
                Right.MouseMove += ResizePressed;
                Right.MouseDown += ResizePressed;
                var BottomLeft = customWindowTemplate.FindName("ResizeBottomLeft", this) as Rectangle;
                BottomLeft.MouseMove += ResizePressed;
                BottomLeft.MouseDown += ResizePressed;
                var Bottom = customWindowTemplate.FindName("ResizeBottom", this) as Rectangle;
                Bottom.MouseMove += ResizePressed;
                Bottom.MouseDown += ResizePressed;
                var BottomRight = customWindowTemplate.FindName("ResizeBottomRight", this) as Rectangle;
                BottomRight.MouseMove += ResizePressed;
                BottomRight.MouseDown += ResizePressed;
            }
        }

        /// <summary>
        /// 拖动事件
        /// </summary>
        public void ResizePressed(object sender, MouseEventArgs e)
        {
            if (canDragResize)
            {
                FrameworkElement element = sender as FrameworkElement;
                ResizeDirection direction = (ResizeDirection)Enum.Parse(typeof(ResizeDirection), element.Name.Replace("Resize", ""));
                this.Cursor = cursors[direction];
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    SendMessage(HwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private double normaltop;
        private double normalleft;
        private double normalwidth;
        private double normalheight;
        /// <summary>
        /// 最大化（全屏/还原）
        /// </summary>
        public void Maximized()
        {
            //wpf最大化 全屏显示任务栏处理
            if (this.WindowState == WindowState.Normal)
            {
                normaltop = this.Top;
                normalleft = this.Left;
                normalwidth = this.Width;
                normalheight = this.Height;

                double shadowSize = 20;

                double top = SystemParameters.WorkArea.Top - shadowSize;
                double left = SystemParameters.WorkArea.Left - shadowSize;
                double right = SystemParameters.PrimaryScreenWidth - SystemParameters.WorkArea.Right - shadowSize;
                double bottom = SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Bottom - shadowSize;

                Border outBorder = GetTemplateChild("outBorder") as Border;
                outBorder.Margin = new Thickness(left, top, right, bottom);

                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;

                //必须先设置为0,在重新设值,若前后值一样,会失效 --拖动任务栏后,还原-始终显示在屏幕最左上方
                this.Top = 0;
                this.Left = 0;
                this.Width = 0;
                this.Height = 0;
                this.Top = normaltop;
                this.Left = normalleft;
                this.Width = normalwidth;
                this.Height = normalheight;

                Border outBorder = GetTemplateChild("outBorder") as Border;
                outBorder.Margin = new Thickness(0);
            }
        }

        /// <summary>
        /// 最小化（缩到任务栏）
        /// </summary>
        public void Minimiz()
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
