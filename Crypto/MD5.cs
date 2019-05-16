using System.Security.Cryptography;
using System.Text;

namespace BToolkitForWPF.Crypto
{
    public class MD5
    {
        public static string Encrypt(string inputStr, bool upperCase = false)
        {
            if (inputStr != null)
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputStr);
                return Encrypt(inputBytes, upperCase);
            }
            return null;
        }

        public static string Encrypt(byte[] inputBytes, bool upperCase = false)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                if (upperCase)
                {
                    sb.Append(hash[i].ToString("X2"));//字母大写
                }
                else
                {
                    sb.Append(hash[i].ToString("x2"));//字母小写
                }
            }
            return sb.ToString();
        }
    }
}
