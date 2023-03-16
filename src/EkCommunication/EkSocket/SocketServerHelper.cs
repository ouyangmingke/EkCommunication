using EkTools;
using EkTools.EkLog;

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EkCommunication.EkSocket
{
    /// <summary>
    /// Socket 服务端主要是监听
    /// </summary>
    public class SocketServerHelper
    {
        public SocketServerHelper(string ip, int port)
        {
            _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }
        Socket _tcpSocket;
        Socket _udpSocket;
        readonly IPEndPoint _iPEndPoint;

        private static ManualResetEvent UdpDone = new ManualResetEvent(true);

        public async Task ReceiveMessageByUdpAsync()
        {
            if (NetworkInformation.Instance.PortInUse(_iPEndPoint.Port))
            {
                throw new Exception($"端口{_iPEndPoint.Address}:{_iPEndPoint.Port}被占用");
            }
            try
            {
                UdpDone.WaitOne();
                // 判断是否已绑定端口
                if (!_udpSocket.IsBound)
                {
                    _udpSocket.Bind(_iPEndPoint);
                }
                UdpDone.Reset();
                while (true)
                {
                    byte[] bytes = new byte[1024];
                    //var client = await _udpSocket.AcceptAsync();
                    // 读取不到数据便停止
                    while (await _udpSocket.ReceiveAsync(bytes, SocketFlags.None) != 0)
                    {
                        await Console.Out.WriteLineAsync($"读取到数据{Encoding.UTF8.GetString(bytes)}");
                    }
                }
            }
            catch (Exception e)
            {
                EkLog.Error(e.Message);
            }
            UdpDone.Set();
        }


        public async Task ReceiveMessageByTcpAsync()
        {
            if (NetworkInformation.Instance.PortInUse(_iPEndPoint.Port))
            {
                throw new Exception($"端口{_iPEndPoint.Address}:{_iPEndPoint.Port}被占用");
            }

            try
            {
                // 判断是否已绑定端口
                if (!_tcpSocket.IsBound)
                {
                    // 手动绑定端口 不绑定或端口设置为 0 将由系统分配随机端口
                    _tcpSocket.Bind(_iPEndPoint);
                    _tcpSocket.Listen();
                }
                while (true)
                {
                    byte[] bytes = new byte[1024];
                    var client = await _tcpSocket.AcceptAsync();
                    // 读取不到数据便停止
                    while (await client.ReceiveAsync(bytes, SocketFlags.None) != 0)
                    {
                        await Console.Out.WriteLineAsync($"读取到数据{Encoding.UTF8.GetString(bytes)}");
                    }
                }
            }
            catch (Exception e)
            {
                // 一般是端口被占用
            }
        }
    }
}
