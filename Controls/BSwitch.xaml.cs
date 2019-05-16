using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BToolkitForWPF.Controls
{
    public class BSwitchBase : UserControl
    {
        //附加CornerRadius属性
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(BSwitchBase));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        //附加BarSize属性
        public static readonly DependencyProperty BarSizeProperty = DependencyProperty.Register("BarSize", typeof(double), typeof(BSwitchBase));
        public double BarSize
        {
            get { return (double)GetValue(BarSizeProperty); }
            set { SetValue(BarSizeProperty, value); }
        }
        //附加BarMargin属性
        public static readonly DependencyProperty BarMarginProperty = DependencyProperty.Register("BarMargin", typeof(Thickness), typeof(BSwitchBase));
        public Thickness BarMargin
        {
            get { return (Thickness)GetValue(BarMarginProperty); }
            set { SetValue(BarMarginProperty, value); }
        }
        //附加TextColor属性
        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register("TextColor", typeof(Brush), typeof(BSwitchBase));
        public Brush TextColor
        {
            get { return (Brush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }
        //附加IsOn属性
        public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register("IsOn", typeof(bool), typeof(BSwitchBase));
        public bool IsOn
        {
            get { return (bool)GetValue(IsOnProperty); }
            set
            {
                SetValue(IsOnProperty, value);
                this.OnStyleVisibility = value ? Visibility.Visible : Visibility.Hidden;
                this.OffStyleVisibility = value ? Visibility.Hidden : Visibility.Visible;
                //触发事件
                RoutedEventArgs routedEvent = new RoutedEventArgs(ValueChangedEvent, this);
                this.RaiseEvent(routedEvent);
            }
        }
        //附加OnStyleVisibility属性
        public static readonly DependencyProperty OnStyleVisibilityProperty = DependencyProperty.Register("OnStyleVisibility", typeof(Visibility), typeof(BSwitchBase));
        public Visibility OnStyleVisibility
        {
            get { return (Visibility)GetValue(OnStyleVisibilityProperty); }
            set { SetValue(OnStyleVisibilityProperty, value); }
        }
        //附加OffStyleVisible属性
        public static readonly DependencyProperty OffStyleVisibilityProperty = DependencyProperty.Register("OffStyleVisibility", typeof(Visibility), typeof(BSwitchBase));
        public Visibility OffStyleVisibility
        {
            get { return (Visibility)GetValue(OffStyleVisibilityProperty); }
            set { SetValue(OffStyleVisibilityProperty, value); }
        }
        //添加事件
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BSwitchBase));
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public BSwitchBase()
        {
            this.SetBinding(TextColorProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("TextColor")
            });
            this.SetBinding(IsOnProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("IsOn")
            });
            this.SetBinding(OnStyleVisibilityProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("OnStyleVisibility")
            });
            this.SetBinding(OffStyleVisibilityProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath("OffStyleVisibility")
            });
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            //圆角半径永远等于高度的一半
            CornerRadius = new CornerRadius(this.Height * 0.5);
            BarSize = this.Height - this.BorderThickness.Left * 4;
            BarMargin = this.BorderThickness;
        }
    }

    /// <summary>
    /// BSwitch.xaml 的交互逻辑
    /// </summary>
    public partial class BSwitch : BSwitchBase
    {
        private const float ClickSleep = 3;
        private bool canClick = true;

        public BSwitch()
        {
            InitializeComponent();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (!canClick)
            {
                return;
            }
            canClick = false;
            BTimer.Invoke(ClickSleep, () => { canClick = true; });
            base.OnMouseUp(e);
            IsOn = !IsOn;
        }

    }
}
