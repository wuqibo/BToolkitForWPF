using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace BToolkitForWPF.Controls
{
    public class BTextBoxBase : TextBox
    {
        //附加属性CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(BTextBoxBase));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        //附加属性Placeholder
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(BTextBoxBase));
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }
        //附加属性PlaceholderVisible
        public static readonly DependencyProperty PlaceholderVisibleProperty = DependencyProperty.Register("PlaceholderVisible", typeof(Visibility), typeof(BTextBoxBase));
        public Visibility PlaceholderVisible
        {
            get { return (Visibility)GetValue(PlaceholderVisibleProperty); }
            set { SetValue(PlaceholderVisibleProperty, value); }
        }

        public BTextBoxBase()
        {
            this.SetBinding(CornerRadiusProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("CornerRadius")
            });
            this.SetBinding(PlaceholderVisibleProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("PlaceholderVisible")
            });
            //设置默认值
            this.VerticalContentAlignment = VerticalAlignment.Center;
        }
    }

    /// <summary>
    /// BTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class BTextBox : BTextBoxBase
    {
        public BTextBox()
        {
            InitializeComponent();

            this.TextChanged += OnTextChanged;
            
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            this.PlaceholderVisible = string.IsNullOrEmpty(this.Text) ? Visibility.Visible : Visibility.Hidden;
            this.Foreground = GetForeground();
        }

        /// <summary>
        /// 该方法用于允许复制粘贴剪切功能(取反则不允许)
        /// </summary>
        protected virtual void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = false;
        }

        /// <summary>
        /// 虚拟函数，以便子类做逻辑
        /// </summary>
        protected virtual Brush GetForeground()
        {
            return this.Foreground;
        }
    }
}
