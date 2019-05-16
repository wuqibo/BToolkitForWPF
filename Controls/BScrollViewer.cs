using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace BToolkitForWPF.Controls
{
    /// <summary>
    /// 支持呈现惯性滚动
    /// </summary>
    class BScrollViewer : ScrollViewer
    {
        /// <summary>
        /// 是否允许呈现惯性滚动(手动指定)
        /// </summary>
        public bool CanInertia { get { return true; } }

        private DispatcherTimer timer = new DispatcherTimer();
        private double delta;
        private bool canMove = false;

        public BScrollViewer()
        {
            if (CanInertia)
            {
                timer.Tick += new EventHandler(OnUpdate);
                timer.Interval = TimeSpan.FromSeconds(0.01);
                timer.Start();
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!CanInertia)
            {
                base.OnMouseWheel(e);
                return;
            }
            e.Handled = true;

            canMove = true;
            delta = e.Delta;
        }

        private void OnUpdate(object sender, EventArgs e)
        {
            if (canMove)
            {
                this.ScrollToVerticalOffset(this.VerticalOffset - delta * 0.2);
                delta += (0 - delta) * 0.1f;
                if (Math.Abs(delta) < 0.1)
                {
                    canMove = false;
                }
            }
        }
    }
}
