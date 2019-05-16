using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BToolkitForWPF
{
    public class ImageHelper
    {
        /// <summary>
        /// 修改图片尺寸
        /// </summary>
        public static BitmapSource ResizeImage(BitmapSource source, int newWidth, int newHeight)
        {
            System.Drawing.Size newSize = new System.Drawing.Size(newWidth, newHeight);
            Bitmap sourceBitmap = BitmapSourceToBitmap(source);
            Bitmap newBitmap = new Bitmap(sourceBitmap, newSize);
            BitmapSource bitmapSource = BitmapToBitmapSource(newBitmap);
            return bitmapSource;
        }

        /// <summary>
        /// 从OpenFileDialog中获取图片Base64
        /// </summary>
        public static string GetBase64FromOpenFile(OpenFileDialog openFileDialog)
        {
            Stream ms = openFileDialog.OpenFile();
            try
            {
                byte[] bytes = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(bytes, 0, Convert.ToInt32(ms.Length));
                return Convert.ToBase64String(bytes);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 从BitmapSource转成Base64
        /// </summary>
        public static string BitmapSourceToBase64(BitmapSource source)
        {
            try
            {
                Bitmap bitmap = BitmapSourceToBitmap(source);
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Jpeg);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    ms.Close();
                    return Convert.ToBase64String(arr);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 从BitmapImage转成BitmapImage
        /// </summary>
        public static Bitmap BitmapSourceToBitmap(BitmapSource source)
        {
            try
            {
                Bitmap bitmap = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                BitmapData data = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                    ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride); bitmap.UnlockBits(data);
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        /// <summary>
        /// 从Bitmap转成ImageSource
        /// </summary>
        public static BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {
            try
            {
                IntPtr ip = bitmap.GetHbitmap();
                BitmapSource bitmapSource = null;
                bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap( ip, IntPtr.Zero, Int32Rect.Empty,
                     BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));
                DeleteObject(ip);
                return bitmapSource;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 从Bytes转成BitmapImage
        /// </summary>
        public static BitmapImage BytesToBitmapImage(byte[] bytes)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(bytes);
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 从Base64转成BitmapImage
        /// </summary>
        public static BitmapImage Base64ToBitmapImage(string base64)
        {
            try
            {
                byte[] streamBase = Base64ToBytes(base64);
                return BytesToBitmapImage(streamBase);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 从Base64转成Bytes
        /// </summary>
        public static byte[] Base64ToBytes(string base64)
        {
            try
            {
                string imagebase64 = base64.Substring(base64.IndexOf(",") + 1);
                return Convert.FromBase64String(imagebase64);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 保存Base64图片到本地（fullPath：包括文件名和后缀名）
        /// </summary>
        public static void SaveBase64ToFile(string base64, string fullPath)
        {
            try
            {
                byte[] bytes = Base64ToBytes(base64);
                SaveBytesToFile(bytes, fullPath);
            }
            catch { }
        }

        /// <summary>
        /// 保存Bytes图片到本地（fullPath：包括文件名和后缀名）
        /// </summary>
        public static void SaveBytesToFile(byte[] bytes, string fullPath)
        {
            string path = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllBytes(fullPath, bytes);
        }

        /// <summary>
        /// 读取本地图片到Bytes
        /// </summary>
        public static byte[] ReadFileToBytes(string fullPath)
        {
            try
            {
                return File.ReadAllBytes(fullPath);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 读取本地图片到BitmapImage
        /// </summary>
        public static BitmapImage ReadFileToBitmapImage(string fullPath)
        {
            byte[] bytes = ReadFileToBytes(fullPath);
            return BytesToBitmapImage(bytes);
        }

    }
}
