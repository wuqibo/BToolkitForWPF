using System.Linq;
using System.Net.NetworkInformation;

namespace BToolkitForWPF
{
    public class SystemInfo
    {
        /// <summary>
        /// 获取设备唯一ID(网卡ID)
        /// </summary>
        /// <returns></returns>
        public static string DeviceID
        {
            get
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                return interfaces.Select(ni => ni.GetPhysicalAddress().ToString()).FirstOrDefault();
            }
        }
    }
}
