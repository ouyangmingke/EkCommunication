using EkTools.EkLog;

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EkCommunication.EkSocket
{
    public class SocketClientHelper
    {
        private SocketClientHelper()
        {
            UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        }
        private static readonly SocketClientHelper Current = new SocketClientHelper();
        public static SocketClientHelper Instance => Current;
        private static ManualResetEvent UdpDone = new ManualResetEvent(true);

        private Socket UdpSocket;

        public async Task<string> ReceiveMessageAsync(string socketState, string ip = "192.168.11.88", int port = 501)
        {

            switch (socketState)
            {
                case SocketState.TCP:
                    return await ReceiveMessageByTcpAsync(ip, port);
                case SocketState.UDP:
                    return await ReceiveMessageByUdpAsync(ip, port);
                default:
                    throw new NotImplementedException();
            }
        }

        private async Task<string> ReceiveMessageByUdpAsync(string ip = "192.168.11.88", int port = 501)
        {
            var res = "";
            try
            {
                UdpDone.WaitOne();
                //  Socket 是否绑定到特定本地端口
                if (!UdpSocket.IsBound)
                {
                    UdpSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                }
                UdpDone.Reset();
                byte[] bytes = new byte[1024];
                var bytesRead = await UdpSocket.ReceiveAsync(bytes, SocketFlags.None);
                res = Encoding.UTF8.GetString(bytes, 0, bytesRead);
            }
            catch (Exception e)
            {
                EkLog.Error(e.Message);
            }
            UdpDone.Set();
            return res;
        }
        private async Task<string> ReceiveMessageByTcpAsync(string ip = "127.0.0.1", int port = 501)
        {
            var stringBuilder = new StringBuilder();
            try
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // 判断是否已绑定端口
                if (!socket.IsBound)
                {
                    // 手动绑定端口 不绑定或端口设置为 0 将由系统分配随机端口
                    socket.Bind(new IPEndPoint(IPAddress.Parse(ip), 888));
                }
                // 与远程主机建立连接
                await socket.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port));
                while (true)
                {
                    byte[] bytes = new byte[1024];
                    // 接收消息
                    var bytesRead = await socket.ReceiveAsync(bytes, SocketFlags.None);
                    // 缓存数据
                    stringBuilder.Append(Encoding.UTF8.GetString(bytes, 0, bytesRead));
                    if (bytesRead == 0)
                    {// 结束连接 这里是读取不到数据便停止
                        socket.Close();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                EkLog.Error(e.Message);
            }
            return stringBuilder.ToString();
        }
    }
}
