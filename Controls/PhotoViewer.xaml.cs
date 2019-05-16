using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BToolkitForWPF.Controls
{
    /// <summary>
    /// PhotoViewer.xaml 的交互逻辑
    /// </summary>
    public partial class PhotoViewer : NoBorderWindow
    {
        public PhotoViewer(ImageSource imgSource)
        {
            InitializeComponent();
            this.img.Source = imgSource;
            this.Topmost = true;
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// 最大化
        /// </summary>
        private void Btn_Maximized_Click(object sender, RoutedEventArgs e)
        {
            this.Maximized();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 滚轮
        /// </summary>
        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            
        }
    }
}
