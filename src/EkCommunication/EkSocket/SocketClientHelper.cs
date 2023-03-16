using EkTools.EkLog;

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EkCommunication.EkSocket
{
    /// <summary>
    /// Socket 客户端是请求连接
    /// </summary>
    public class SocketClientHelper
    {
        public SocketClientHelper(string ip, int port)
        {
            _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _targetIpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }
        Socket _tcpSocket;
        Socket _udpSocket;
        /// <summary>
        /// 目标地址
        /// </summary>
        readonly IPEndPoint _targetIpEndPoint;

        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="msg"></param>
        public async Task SendMessageByUdpAsync(string msg)
        {
            try
            {
                // 创建 Socket 客户端 使用 Ipv4 解析地址 数据报类型  Udp 网络协议
                byte[] sendbuf = Encoding.UTF8.GetBytes(msg);
                await _udpSocket.SendToAsync(sendbuf, SocketFlags.None, _targetIpEndPoint);
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
        public async Task<bool> SendMessageByTcpAsync(string msg)
        {
            //close_wait  结束发送后端口会进入close_wait 状态 (TCP 特性 提起结束端将等待 另一方的确认)
            try
            {
                // 创建 Socket 客户端 使用 Ipv4 解析地址 字节流类型  Tcp 网络协议
                // 手动绑定端口 不绑定或端口设置为 0 将由系统分配随机端口  
                // 如果服务器端口不变 
                // _tcpSocket 若未连接则连接到指定端口
                if (!_tcpSocket.Connected)
                    _tcpSocket.Connect(_targetIpEndPoint);
                byte[] sendbuf = Encoding.UTF8.GetBytes(msg);
                var rc = await _tcpSocket.SendToAsync(sendbuf, SocketFlags.None, _targetIpEndPoint);
                // rc<0 发送错误 rc == 0 发送成功
                EkLog.Information($"数据发送成功");
                #region 等待服务端回复数据
                /**
                var stringBuilder = new StringBuilder();
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(1000);
                    byte[] bytes = new byte[1024];
                    var bytesReadLenght = await _Socket.ReceiveAsync(bytes, SocketFlags.None);
                    // 缓存数据
                    stringBuilder.Append(Encoding.UTF8.GetString(bytes, 0, bytesReadLenght));
                    if (bytesReadLenght > 0)
                    {
                        await Console.Out.WriteLineAsync($"获取到回复数据:{stringBuilder}");
                        //break;
                    }
                }
                */
                #endregion
            }
            catch (Exception e)
            {
                // 防止断开连接导致端口被关闭
                _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                EkLog.Error($"Socket发送TCP数据失败{e.Message}");
                return false;
            }
            return true;
        }
    }
}
