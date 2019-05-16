using System;
using System.Configuration;

namespace BToolkitForWPF
{
    /// <summary>
    /// 项目添加引用System.Configuration
    /// 在App.config文件里配置好key：
    /// <appSettings>
    ///    <add key = "account" value=""/>
    ///    <add key = "password" value=""/>
    /// </appSettings>
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 读取客户设置
        /// </summary>
        public static string Get(string key)
        {
            try
            {
                string settingString = ConfigurationManager.AppSettings[key].ToString();
                return settingString;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 更新设置
        /// </summary>
        public static void Set(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ConfigurationManager.AppSettings[key] != null)
            {
                config.AppSettings.Settings.Remove(key);
            }
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
