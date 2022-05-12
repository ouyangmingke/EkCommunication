
using EkTools.EkLog;

using System.Net.Sockets;
using System.Text;

namespace EkCommunication.EkTCP
{

    public class TcpClientHelper
    {
        private TcpClientHelper()
        {

        }
        private static readonly TcpClientHelper Current = new TcpClientHelper();
        public static TcpClientHelper Instance => Current;

        AutoResetEvent autoResetEvent = new AutoResetEvent(true);
        public async Task<string> ReceiveMessageAsync(string ip, int port)
        {
            var res = "";
            try
            {
                autoResetEvent.WaitOne();
                // 创建 TCP 客户端实例并连接到指定 终结点
                var client = new TcpClient(ip, port);
                EkLog.Information($"创建tcp客户端 连接目标为： {client.Client.RemoteEndPoint}");
                // 获取当前连接的网络传输流
                var networkStream = client.GetStream();
                int bytesRead = 0;
                int x = 0;
                var stringBuilder = new StringBuilder();
                while (x < 5)
                {
                    // 等待 服务端发送数据 若
                    if (networkStream.DataAvailable)
                    {
                        byte[] bytes = new byte[1024];
                        bytesRead = await networkStream.ReadAsync(bytes, 0, 1024);
                        var temp = Encoding.UTF8.GetString(bytes, 0, bytesRead);
                        // 拼接获取的数据
                        stringBuilder.Append(temp);
                        if (temp.EndsWith("EKEND"))
                        {
                            break;
                        }
                    }
                    // 超时停止读取
                    Thread.Sleep(1000);
                    x++;
                }
                res = stringBuilder.ToString();
                EkLog.Information("读取完成");
                networkStream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                EkLog.Error(e.Message);
            }
            autoResetEvent.Set();
            return res;
        }
    }
}
