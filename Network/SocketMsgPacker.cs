using System;
using System.Collections.Generic;
using System.Net;

namespace BToolkitForWPF.Network
{
    public class SocketMsgPacker
    {
        /// <summary>
        /// 头部长度
        /// </summary>
        private const int HeadLength = 4;
        /// <summary>
        /// 收到的字节池
        /// </summary>
        private List<byte> bytesPool = new List<byte>();

        /// <summary>
        /// 添加头部
        /// </summary>
        public byte[] AddHead(byte[] sendBytes)
        {
            byte[] byteLen = IntToBytes(sendBytes.Length);
            List<byte> all = new List<byte>();
            //包长
            all.AddRange(byteLen);
            //包体
            all.AddRange(sendBytes);
            return all.ToArray();
        }

        /// <summary>
        /// 从接收到的字节中根据包头分析出完整的包
        /// </summary>
        public void GetMsgPackage(byte[] recBytes, Action<byte[]> FullPackageCallback)
        {
            //压入字节池
            bytesPool.AddRange(recBytes);
            //取出所有包
            while (bytesPool.Count > HeadLength)
            {
                //取出头部得到消息体的长度
                List<byte> head = bytesPool.GetRange(0, HeadLength);
                int bodyLeng = BytesToInt(head.ToArray());

                //尝试取出完整的消息体（如果长度不够则等待下一条消息再处理）
                if (bodyLeng > 0 && bytesPool.Count - HeadLength >= bodyLeng)
                {
                    //去掉消息头
                    bytesPool.RemoveRange(0, HeadLength);
                    //按长度得到消息体
                    List<byte> body = bytesPool.GetRange(0, bodyLeng);
                    bytesPool.RemoveRange(0, bodyLeng);
                    FullPackageCallback(body.ToArray());
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 整型转byte[]
        /// </summary>
        public static byte[] IntToBytes(int data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian)
            {
                //转成大端
                Array.Reverse(bytes);
            }
            return bytes;
        }

        /// <summary>
        /// byte[]转整型
        /// </summary>
        public static int BytesToInt(byte[] data)
        {
            int value = BitConverter.ToInt32(data, 0);
            if (BitConverter.IsLittleEndian)
            {
                //转成大端
                return IPAddress.HostToNetworkOrder(value);
            }
            else
            {
                return value;
            }
        }
    }
}
