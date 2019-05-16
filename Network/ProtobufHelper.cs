using ProtoBuf;
using System;
using System.IO;

namespace BToolkitForWPF.Network
{
    class ProtobufHelper
    {
        /// <summary>
        /// 编码
        /// </summary>
        public static byte[] Encode<T>(T obj)
        {
            byte[] msgdata;
            using (Stream stream = new MemoryStream())
            {
                Serializer.Serialize<T>(stream, obj);
                msgdata = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(msgdata, 0, msgdata.Length);
            }
            return msgdata;
        }

        /// <summary>
        /// 解码
        /// </summary>
        public static T Decode<T>(byte[] body)
        {
            T obj = default(T);
            try
            {
                using (Stream stream = new MemoryStream(body))
                {
                    obj = Serializer.Deserialize<T>(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("------解码失败：" + ex.ToString());
            }
            return obj;
        }
    }
}
