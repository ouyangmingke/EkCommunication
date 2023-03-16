using EkTools.EkLog;

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EkCommunication.EkUDP
{
    public class UdpClientHelper
    {
        public UdpClientHelper(string ip, int port)
        {
            _iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            udpClient = new UdpClient();
        }
        UdpClient udpClient;
        readonly IPEndPoint _iPEndPoint;

        AutoResetEvent SendEvent = new AutoResetEvent(true);

        public async Task<bool> SendMessageAsync(string data)
        {
            SendEvent.WaitOne();
            try
            {
                // 防止占用过多端口
                if (!udpClient.Client.Connected)
                {
                    udpClient.Connect(_iPEndPoint);
                }
                var sendBytes = Encoding.UTF8.GetBytes(data);
                await udpClient.SendAsync(sendBytes, sendBytes.Length);
            }
            catch (Exception e)
            {
                // 常见异常：当前客户端已创建连接
                EkLog.Error(e.Message);
                return false;
            }
            SendEvent.Set();
            return true;
        }

        AutoResetEvent ReceiveEvent = new AutoResetEvent(true);

        bool IsBind = false;

        public async Task ReceiveMessageAsync()
        {
            if (IsBind)
            {
                //避免重复调用
                return;
            }
            IsBind = true;
            ReceiveEvent.WaitOne();
            try
            {
                udpClient.Client.Bind(_iPEndPoint);
                // 等待接收广播
                while (true)
                {
                    var result = await udpClient.ReceiveAsync();
                    var groupEP = result.RemoteEndPoint;
                    byte[] bytes = result.Buffer;
                    var msg = $"From:{groupEP} \n Msg {Encoding.UTF8.GetString(bytes)}";
                    await Console.Out.WriteLineAsync($"获取UDP消息{msg}");
                }
            }
            catch (Exception e)
            {
                // 常见异常：当前端口已被绑定
                EkLog.Error(e.Message);
            }
            ReceiveEvent.Set();
        }
    }
}
