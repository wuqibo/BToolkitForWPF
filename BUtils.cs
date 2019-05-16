using System;

namespace BToolkitForWPF
{
    public class BUtils
    {
        /// <summary>
        /// 返回包含该应用程序名的所在目录完整路径
        /// </summary>
        public static string AppInstalledPath
        {
            get => AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            //Environment.CurrentDirectory
        }

        /// <summary>
        /// 按小数点后几位向下取
        /// </summary>
        public static string FloorToString(double value, int decimals = 2)
        {
            double pointOffset = System.Math.Pow(10, decimals);
            double a = value * pointOffset;
            int b = (int)a;
            double c = b / pointOffset;
            string format = "0.00";
            switch (decimals)
            {
                case 0:
                    format = "0";
                    break;
                case 1:
                    format = "0.0";
                    break;
                case 2:
                    format = "0.00";
                    break;
                case 3:
                    format = "0.000";
                    break;
                case 4:
                    format = "0.0000";
                    break;
            }
            return c.ToString(format);
        }

        /// <summary>
        /// 打印字节数组
        /// </summary>
        public static void ConsoleBytes(string tag, byte[] bytes)
        {
            string str = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    str += bytes[i] + " ";
                }
            }
            Console.WriteLine(tag + " : " + str);
        }
    }
}
