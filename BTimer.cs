using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace BToolkitForWPF
{
    class BTimer
    {
        private static Dictionary<string, DispatcherTimer> timers = new Dictionary<string, DispatcherTimer>();

        public static void Invoke(float delaySeconds, Action Function, string timerId = null)
        {
            if (delaySeconds <= 0)
            {
                Function();
                return;
            }
            DispatcherTimer timer = null;
            if (timerId != null)
            {
                if (timers.ContainsKey(timerId))
                {
                    timer = timers[timerId];
                }
                else
                {
                    timer = new DispatcherTimer();
                    timers.Add(timerId, timer);
                }
            }
            else
            {
                timer = new DispatcherTimer();
            }
            timer.Tick += new EventHandler((object sender, EventArgs e) =>
            {
                Function();
                timer.Stop();
            });
            timer.Interval = TimeSpan.FromSeconds(delaySeconds);
            timer.Start();
        }

        public static void InvokeCancel(string timerId)
        {
            if (timers.ContainsKey(timerId))
            {
                DispatcherTimer timer = timers[timerId];
                timer.Stop();
                timers.Remove(timerId);
                timer = null;
            }
        }
    }
}
