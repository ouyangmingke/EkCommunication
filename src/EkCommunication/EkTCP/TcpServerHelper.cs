
using EkTools.EkLog;

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EkCommunication.EkTCP
{
    public class TcpServerHelper
    {
        private TcpServerHelper() { }

        private static readonly TcpServerHelper Current = new TcpServerHelper();
        public static TcpServerHelper Instance => Current;
        AutoResetEvent autoResetEvent = new AutoResetEvent(true);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        public async Task SendMessageAsync(string data, string ip, int port)
        {
            try
            {
                autoResetEvent.WaitOne();
                // 创建 TCP 监视
                var listener = new TcpListener(IPAddress.Parse(ip), port);
                // 启动监听 TCP 端口上的传入请求
                listener.Start();
                EkLog.Information($"开始监听{listener.LocalEndpoint}");
                while (true)
                {
                    Thread.Sleep(1000);
                    // 检查是否存在挂起的请求
                    if (!listener.Pending())
                    {
                        EkLog.Information("无链接请求");
                        continue;
                    }
                    // 侦听到用户端的连接后 创建 TcpClient 以处理请求
                    var client = listener.AcceptTcpClient();
                    // 也可创建 Socket 处理请求
                    //var socket = listener.AcceptSocket();
                    var remote = client.Client.RemoteEndPoint;
                    EkLog.Information($"获取到{remote}发送的连接请求");
                    // 获取网络流
                    var networkStream = client.GetStream();
                    // 向网络流中写入数据
                    for (int i = 0; i < 3; i++)
                    {
                        byte[] byteTime = Encoding.UTF8.GetBytes($"{i} Ip:{remote} time:{DateTime.Now} msg: {data}\n");
                        await networkStream.WriteAsync(byteTime, 0, byteTime.Length);
                        // 模拟系统运行
                        Thread.Sleep(200);
                    }
                    // 写入结束标识符
                    byte[] end = Encoding.UTF8.GetBytes($"EKEND");
                    await networkStream.WriteAsync(end, 0, end.Length);
                    // 关闭网络流
                    networkStream.Close();
                    // 关闭 TCP 客户端
                    client.Close();
                    break;
                }
                EkLog.Information("数据写入完成,停止侦听");
                // 停止侦听
                listener.Stop();
            }
            catch (Exception e)
            {
                EkLog.Error(e.Message);
            }
            autoResetEvent.Set();
        }
    }
}
