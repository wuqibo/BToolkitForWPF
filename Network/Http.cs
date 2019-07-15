using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace BToolkitForWPF.Network
{
    class Http
    {
        public enum ResultType
        {
            NoNetwork, Success, Error
        }
        public delegate void HttpCallbackAction(ResultType resultType, string body);

        /// <summary>
        /// HttpCallback(ResultType resultType, string result)
        /// </summary>
        public static void Post(string url, Dictionary<string, string> formData, HttpCallbackAction HttpCallback)
        {
            Post(url, formData, null, HttpCallback);
        }

        /// <summary>
        /// HttpCallback(ResultType resultType, string result)
        /// </summary>
        public static async void Post(string url, Dictionary<string, string> formData, Dictionary<string, string> headers, HttpCallbackAction HttpCallback)
        {
            HttpContent httpContent = new FormUrlEncodedContent(formData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            httpContent.Headers.ContentType.CharSet = "utf-8";
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    httpContent.Headers.Add(header.Key, header.Value);
                }
            }
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.PostAsync(url, httpContent);
                    string statusCode = response.StatusCode.ToString();
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        HttpCallback(ResultType.Success, result);
                    }
                    else
                    {
                        HttpCallback(ResultType.Error, response.Content.ToString());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// HttpCallback(ResultType resultType, string result)
        /// </summary>
        public static void Get(string url, HttpCallbackAction HttpCallback)
        {
            Get(url, null, HttpCallback);
        }
        /// <summary>
        /// HttpCallback(ResultType resultType, string result)
        /// </summary>
        public static async void Get(string url, Dictionary<string, string> headers, HttpCallbackAction HttpCallback)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    string statusCode = response.StatusCode.ToString();
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        HttpCallback(ResultType.Success, result);
                    }
                    else
                    {
                        HttpCallback(ResultType.Error, response.Content.ToString());
                    }
                }
            }
            catch
            {
                HttpCallback(ResultType.Error, "");
            }
        }

        /// <summary>
        /// 获取本机内网IP
        /// </summary>
        public static string GetInternalIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        /// <summary>
        /// 获取本机外网IP
        /// </summary>
        public static string GetExternalIP()
        {
            string ip = "";
            try
            {
                WebClient MyWebClient = new WebClient();
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;
                Byte[] pageData = MyWebClient.DownloadData("http://www.net.cn/static/customercare/yourip.asp");
                string pageHtml = Encoding.Default.GetString(pageData);
                int beginSub = pageHtml.IndexOf("<h2>") + 4;
                int endSub = pageHtml.IndexOf("</h2>");
                string[] ipArr = pageHtml.Substring(beginSub, endSub - beginSub).Split(',');
                ip = ipArr[0];
            }
            catch { }
            return ip;
        }

        /// <summary>
        /// 查看当前是否为IPV6网络
        /// </summary>
        public static bool IsIPv6
        {
            get
            {
                IPAddress[] address = Dns.GetHostAddresses("www.baidu.com");
                return address[0].AddressFamily == AddressFamily.InterNetworkV6;
            }
        }
    }
}
