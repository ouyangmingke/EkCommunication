using EkTools.EkLog;

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EkCommunication.EkSocket
{
    public class SocketServerHelper
    {
        private SocketServerHelper() { }
        private static readonly SocketServerHelper Current = new SocketServerHelper();
        public static SocketServerHelper Instance => Current;

        public async Task SendMessageAsync(string socketState, string msg, string ip = "192.168.11.88", int port = 501)
        {
            switch (socketState)
            {
                case SocketState.TCP:
                    await SendMessageByTcpAsync(msg, ip, port);
                    break;
                case SocketState.UDP:
                    await SendMessageByUdpAsync(msg, ip, port);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="msg"></param>
        private async Task SendMessageByUdpAsync(string msg, string ip = "192.168.11.88", int port = 501)
        {
            try
            {
                // 创建 Socket 客户端 使用 Ipv4 解析地址 数据报类型  Udp 网络协议
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                byte[] sendbuf = Encoding.UTF8.GetBytes(msg);
                await socket.SendToAsync(sendbuf, SocketFlags.None, new IPEndPoint(IPAddress.Parse(ip), port));
                socket.Close();
            }
            catch (Exception e)
            {
                EkLog.Error($"Socket发送Udp数据失败{e.Message}");
            }
        }

        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="msg"></param>
        private async Task SendMessageByTcpAsync(string msg, string ip = "127.0.0.1", int port = 501)
        {
            try
            {
                // 创建 Socket 客户端 使用 Ipv4 解析地址 字节流类型  Udp 网络协议
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //await socket.ConnectAsync(ip, port);
                socket.Bind(new IPEndPoint(IPAddress.Parse(ip), 666));
                socket.Listen(1000);
                var client = await socket.AcceptAsync();
                EkLog.Information($"获取到 {client.RemoteEndPoint}发送的连接请求");
                byte[] sendbuf = Encoding.UTF8.GetBytes(msg);
                await client.SendToAsync(sendbuf, SocketFlags.None, new IPEndPoint(IPAddress.Parse(ip), port));
                socket.Close();
                client.Close();
            }
            catch (Exception e)
            {
                EkLog.Error($"Socket发送TCP数据失败{e.Message}");
            }
        }
    }
}
