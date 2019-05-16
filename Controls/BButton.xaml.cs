using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace BToolkitForWPF.Controls
{
    public class BButtonBase : Button
    {
        //附加Image属性
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(BButtonBase));
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        //附加CornerRadius属性
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(BButtonBase));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        //附加TextAlignment属性
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(BButtonBase));
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public BButtonBase()
        {
            SetBinding(BButtonBase.ImageProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("Image")
            });
            SetBinding(BButtonBase.CornerRadiusProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("CornerRadius")
            });
            SetBinding(BButtonBase.TextAlignmentProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("TextAlignment")
            });
            //默认居中，XAML里设置将会覆盖
            TextAlignment = TextAlignment.Center;
        }
    }

    /// <summary>
    /// BButton.xaml 的交互逻辑
    /// </summary>
    public partial class BButton : BButtonBase
    {
        public BButton()
        {
            InitializeComponent();
        }
    }
}
