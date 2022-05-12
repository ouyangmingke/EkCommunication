using EkTools.EkLog;

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EkCommunication.EkUDP
{
    public class UdpClientHelper
    {
        private static readonly UdpClientHelper Current = new UdpClientHelper();
        public static UdpClientHelper Instance => Current;

        AutoResetEvent SendEvent = new AutoResetEvent(true);

        public async Task SendMessageAsync(string data, string ip, int port)
        {
            SendEvent.WaitOne();
            try
            {
                var udpClient = new UdpClient(ip, port);
                var sendBytes = Encoding.UTF8.GetBytes(data);
                var len = await udpClient.SendAsync(sendBytes, sendBytes.Length);
            }
            catch (Exception e)
            {
                EkLog.Error(e.Message);
            }
            SendEvent.Set();
        }
        AutoResetEvent ReceiveEvent = new AutoResetEvent(true);
        public async Task<string> ReceiveMessageAsync(string ip, int port)
        {
            ReceiveEvent.WaitOne();
            var msg = "";
            try
            {
                UdpClient listener = new UdpClient(port);
                // 等待接收广播
                while (true)
                {
                    var result = await listener.ReceiveAsync();
                    var groupEP = result.RemoteEndPoint;
                    byte[] bytes = result.Buffer;
                    msg = $"From{groupEP} \n Msg {Encoding.UTF8.GetString(bytes, 0, bytes.Length)}";
                    EkLog.Information($"获取UDP消息{msg}");
                    break;
                }
                listener.Close();
            }
            catch (Exception e)
            {
                EkLog.Error(e.Message);
            }
            ReceiveEvent.Set();
            return msg;
        }
    }
}
