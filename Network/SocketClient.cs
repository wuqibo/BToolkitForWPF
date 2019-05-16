using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Threading;

namespace BToolkitForWPF.Network
{
    /// <summary>
    /// Socket客户端
    /// </summary>
    public class SocketClient
    {
        #region 构造函数

        /// <summary>
        /// 构造函数,连接服务器IP地址默认为本机127.0.0.1
        /// </summary>
        /// <param name="port">监听的端口</param>
        public SocketClient(int port)
        {
            _ip = "127.0.0.1";
            _port = port;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">监听的IP地址</param>
        /// <param name="port">监听的端口</param>
        public SocketClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ipAndPort">监听的IP:Port地址</param>
        public SocketClient(string ipAndPort)
        {
            string[] arr = ipAndPort.Split(':');
            _ip = arr[0];
            _port = int.Parse(arr[1]);
        }

        #endregion

        #region 内部成员

        private Socket _socket = null;
        private string _ip = "";
        private int _port = 0;
        private bool _isRec = true;
        private SocketMsgPacker msgPacker;

        private bool IsSocketConnected()
        {
            bool part1 = _socket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (_socket.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
        private Dispatcher mainThreadDispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// 开始接受客户端消息
        /// </summary>
        private void StartRecMsg()
        {
            try
            {
                byte[] container = new byte[SocketServer.BytesMaxLength];
                _socket.BeginReceive(container, 0, container.Length, SocketFlags.None, asyncResult =>
                {
                    try
                    {
                        int length = _socket.EndReceive(asyncResult);

                        //马上进行下一轮接受，增加吞吐量
                        if (length > 0 && _isRec && IsSocketConnected())
                        {
                            StartRecMsg();
                        }

                        if (length > 0)
                        {
                            byte[] recBytes = new byte[length];
                            Array.Copy(container, 0, recBytes, 0, length);
                            msgPacker.GetMsgPackage(recBytes, (byte[] bytes) =>
                            {
                                mainThreadDispatcher.Invoke(() =>
                                {
                                    HandleReceiveMsg?.Invoke(bytes, this);
                                });
                            });
                        }
                        else
                        {
                            Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        mainThreadDispatcher.Invoke(() =>
                        {
                            HandleException?.Invoke(ex);
                        });
                        Close();
                    }
                }, null);
            }
            catch (Exception ex)
            {
                mainThreadDispatcher.Invoke(() =>
                {
                    HandleException?.Invoke(ex);
                });
                Close();
            }
        }

        #endregion

        #region 外部接口

        /// <summary>
        /// 开始服务，连接服务端
        /// </summary>
        public void StartClient(Action<bool> Callback)
        {
            try
            {
                //实例化 套接字 （ip4寻址协议，流式传输，TCP协议）
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //创建 ip对象
                IPAddress address = IPAddress.Parse(_ip);
                //创建网络节点对象 包含 ip和port
                IPEndPoint endpoint = new IPEndPoint(address, _port);
                //将 监听套接字  绑定到 对应的IP和端口
                _socket.BeginConnect(endpoint, asyncResult =>
                {
                    try
                    {
                        _socket.EndConnect(asyncResult);
                        msgPacker = new SocketMsgPacker();
                        //开始接受服务器消息
                        StartRecMsg();
                        //连接成功初始化消息池

                        if (Callback != null)
                        {
                            mainThreadDispatcher.Invoke(() =>
                            {
                                Callback(true);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        mainThreadDispatcher.Invoke(() =>
                        {
                            HandleException?.Invoke(ex);
                            Callback?.Invoke(false);
                        });
                    }
                }, null);
            }
            catch (Exception ex)
            {
                mainThreadDispatcher.Invoke(() =>
                {
                    HandleException?.Invoke(ex);
                    Callback?.Invoke(false);
                });
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes">数据字节</param>
        public void Send(byte[] bytes)
        {
            try
            {
                //添加头部
                byte[] addedHead = msgPacker.AddHead(bytes);

                _socket.BeginSend(addedHead, 0, addedHead.Length, SocketFlags.None, asyncResult =>
                {
                    try
                    {
                        int length = _socket.EndSend(asyncResult);
                        mainThreadDispatcher.Invoke(() =>
                        {
                            HandleSentMsg?.Invoke(bytes, this);
                        });
                    }
                    catch (Exception ex)
                    {
                        mainThreadDispatcher.Invoke(() =>
                        {
                            HandleException?.Invoke(ex);
                        });
                    }
                }, null);
            }
            catch (Exception ex)
            {
                mainThreadDispatcher.Invoke(() =>
                {
                    HandleException?.Invoke(ex);
                });
            }
        }

        /// <summary>
        /// 发送字符串（默认使用UTF-8编码）
        /// </summary>
        /// <param name="msgStr">字符串</param>
        public void Send(string msgStr)
        {
            Send(Encoding.UTF8.GetBytes(msgStr));
        }

        /// <summary>
        /// 发送字符串（使用自定义编码）
        /// </summary>
        /// <param name="msgStr">字符串消息</param>
        /// <param name="encoding">使用的编码</param>
        public void Send(string msgStr, Encoding encoding)
        {
            Send(encoding.GetBytes(msgStr));
        }

        /// <summary>
        /// 传入自定义属性
        /// </summary>
        public object Property { get; set; }

        /// <summary>
        /// 关闭与服务器的连接
        /// </summary>
        public void Close()
        {
            try
            {
                _isRec = false;
                _socket.Disconnect(false);
                mainThreadDispatcher.Invoke(() =>
                {
                    HandleClientClose?.Invoke(this);
                });
            }
            catch (Exception ex)
            {
                mainThreadDispatcher.Invoke(() =>
                {
                    HandleException?.Invoke(ex);
                });
            }
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 处理已发送消息的委托
        /// </summary>
        public Action<byte[], SocketClient> HandleSentMsg { get; set; }

        /// <summary>
        /// 处理接受消息的委托
        /// </summary>
        public Action<byte[], SocketClient> HandleReceiveMsg { get; set; }

        /// <summary>
        /// 客户端被主动关闭后回调
        /// </summary>
        public Action<SocketClient> HandleClientClose { get; set; }

        /// <summary>
        /// 异常处理程序
        /// </summary>
        public Action<Exception> HandleException { get; set; }

        #endregion
    }
}
