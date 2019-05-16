using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Windows.Threading;

namespace BToolkitForWPF.Network
{
    /// <summary>
    /// Socket服务端
    /// </summary>
    public class SocketServer
    {
        /// <summary>
        /// 消息包缓存区最大长度
        /// </summary>
        public const int BytesMaxLength = 1024 * 1024 * 2;//2M缓冲区


        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">监听的IP地址</param>
        /// <param name="port">监听的端口</param>
        public SocketServer(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        /// <summary>
        /// 构造函数,监听IP地址默认为本机0.0.0.0
        /// </summary>
        /// <param name="port">监听的端口</param>
        public SocketServer(int port)
        {
            _ip = "0.0.0.0";
            _port = port;
        }

        #endregion

        #region 内部成员

        private Socket _socket = null;
        private string _ip = "";
        private int _port = 0;
        private bool _isListen = true;
        private Dispatcher mainThreadDispatcher = Dispatcher.CurrentDispatcher;

        private void StartListen()
        {
            try
            {
                _socket.BeginAccept(asyncResult =>
                {
                    try
                    {
                        Socket newSocket = _socket.EndAccept(asyncResult);

                        //马上进行下一轮监听,增加吞吐量
                        if (_isListen)
                            StartListen();

                        SocketConnection newClient = new SocketConnection(mainThreadDispatcher, newSocket, this)
                        {
                            HandleSentMsg = HandleSentMsg == null ? null : new Action<byte[], SocketConnection, SocketServer>(HandleSentMsg),
                            HandleReceiveMsg = HandleReceiveMsg == null ? null : new Action<byte[], SocketConnection, SocketServer>(HandleReceiveMsg),
                            HandleClientClose = HandleClientClose == null ? null : new Action<SocketConnection, SocketServer>(HandleClientClose),
                            HandleException = HandleException == null ? null : new Action<Exception>(HandleException)
                        };

                        newClient.StartRecMsg();
                        ClientList.AddLast(newClient);

                        mainThreadDispatcher.Invoke(()=> {
                            HandleNewClientConnected?.Invoke(this, newClient);
                        });
                    }
                    catch (Exception ex)
                    {
                        mainThreadDispatcher.Invoke(()=> {
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

        #endregion

        #region 外部接口

        /// <summary>
        /// 开始服务，监听客户端
        /// </summary>
        public void StartServer(Action<bool> Callback)
        {
            try
            {
                //实例化套接字（ip4寻址协议，流式传输，TCP协议）
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //创建ip对象
                IPAddress address = IPAddress.Parse(_ip);
                //创建网络节点对象包含ip和port
                IPEndPoint endpoint = new IPEndPoint(address, _port);
                //将 监听套接字绑定到 对应的IP和端口
                _socket.Bind(endpoint);
                //设置监听队列长度为Int32最大值(同时能够处理连接请求数量)
                _socket.Listen(int.MaxValue);
                //开始监听客户端
                StartListen();

                Callback?.Invoke(true);
            }
            catch (Exception ex)
            {
                HandleException?.Invoke(ex);
                Callback?.Invoke(false);
            }
        }

        /// <summary>
        /// 所有连接的客户端列表
        /// </summary>
        public LinkedList<SocketConnection> ClientList { get; set; } = new LinkedList<SocketConnection>();

        /// <summary>
        /// 关闭指定客户端连接
        /// </summary>
        /// <param name="theClient">指定的客户端连接</param>
        public void CloseClient(SocketConnection theClient)
        {
            theClient.Close();
        }

        #endregion

        #region 公共事件

        /// <summary>
        /// 异常处理程序
        /// </summary>
        public Action<Exception> HandleException { get; set; }

        #endregion

        #region 服务端事件

        /// <summary>
        /// 当新客户端连接后执行
        /// </summary>
        public Action<SocketServer, SocketConnection> HandleNewClientConnected { get; set; }

        #endregion

        #region 客户端连接事件

        /// <summary>
        /// 客户端连接发送消息后调用
        /// </summary>
        public Action<byte[], SocketConnection, SocketServer> HandleSentMsg { get; set; }

        /// <summary>
        /// 客户端连接接受新的消息后调用
        /// </summary>
        public Action<byte[], SocketConnection, SocketServer> HandleReceiveMsg { get; set; }

        /// <summary>
        /// 客户端连接关闭后回调
        /// </summary>
        public Action<SocketConnection, SocketServer> HandleClientClose { get; set; }

        #endregion
    }
}
