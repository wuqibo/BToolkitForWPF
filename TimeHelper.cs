using System;

namespace BToolkitForWPF
{
    public class TimeHelper
    {
        public const string TimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 获取当前时间戳（秒）
        /// </summary>
        public static long GetCurrTimeStamp()
        {
            //柏林时间DateTime.UtcNow
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// 获取当前时间（格式：2000-01-01 01:01:01）
        /// </summary>
        public static string GetCurrTimeString()
        {
            DateTime dt = DateTime.Now;
            return dt.ToString(TimeFormat);
        }

    }
}
