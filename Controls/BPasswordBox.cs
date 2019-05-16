using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BToolkitForWPF.Controls
{
    class BPasswordBox : BTextBox
    {
        private Brush defaultForeground;

        public BPasswordBox()
        {
            CreateStarPoints();
        }

        /// <summary>
        /// 创建小圆点
        /// </summary>
        private void CreateStarPoints()
        {
            Brush pointColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666"));
            this.TextDecorations = new TextDecorationCollection(new TextDecoration[] {
                new TextDecoration() {
                    Location= TextDecorationLocation.Strikethrough,
                    Pen= new Pen(pointColor, 10f) {
                        DashCap =  PenLineCap.Round,
                        StartLineCap= PenLineCap.Round,
                        EndLineCap= PenLineCap.Round,
                        DashStyle= new DashStyle(new double[] {0.0,1.2 }, 0.6f)
                    }
                }

            });
        }

        /// <summary>
        /// 该方法用于屏蔽复制剪切粘贴功能
        /// </summary>
        protected override void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }

        /// <summary>
        /// 根据内容变换透明度，以确保为空时只看得见小圆点
        /// </summary>
        /// <returns></returns>
        protected override Brush GetForeground()
        {
            if (defaultForeground == null)
            {
                defaultForeground = this.Foreground;
            }
            return string.IsNullOrEmpty(this.Text) ? defaultForeground : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
        }
    }
}
