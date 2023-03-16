using EkTools.EkLog;

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EkCommunication.EkTCP
{
    public class TcpServerHelper
    {
        public TcpServerHelper(string ip, int port)
        {
            _iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            listener = new TcpListener(_iPEndPoint);

        }

        readonly IPEndPoint _iPEndPoint;
        TcpListener listener;

        /// <summary>
        /// 启用监听
        /// </summary>
        public async Task StartTcpListenerAsync()
        {
            // 原本是防止端口被占用，进行自动重试
            while (true)
            {
                Thread.Sleep(1000);
                if (NetworkInformation.Instance.PortInUse(_iPEndPoint.Port))
                {
                    throw new Exception($"端口{_iPEndPoint.Address}:{_iPEndPoint.Port}被占用");
                }
                // 开始侦听端口 异步等待 连接断开后再次执行
                await TcpListenerAsync();
            }
        }

        private async Task TcpListenerAsync()
        {
            try
            {
                listener.Start();
                EkLog.Information($"开始监听{listener.LocalEndpoint}");
                while (true)
                {
                    Thread.Sleep(1000);
                    // 检查是否存在挂起的请求
                    if (!listener.Pending())
                        continue;
                    var data = "";
                    int bytesRead = 0;
                    byte[] bytes = new byte[1024];
                    var stringBuilder = new StringBuilder();
                    // 侦听到用户端的连接后 创建 TcpClient 以处理请求
                    using var client = listener.AcceptTcpClient();
                    // 也可创建 Socket 处理请求
                    //var socket = listener.AcceptSocket();
                    var remote = client.Client.RemoteEndPoint;
                    EkLog.Information($"获取到{remote}发送的连接请求");
                    // 获取网络流
                    using var networkStream = client.GetStream();

                    // 因为是异步读取所以没有收到数据时会等待读取
                    // 端口将保持连接状态 
                    while ((bytesRead = await networkStream.ReadAsync(bytes, 0, 1024)) != 0)
                    {
                        var temp = Encoding.UTF8.GetString(bytes, 0, bytesRead);
                        stringBuilder.Append(temp);
                        if (temp.Length > 0)
                        {
                            if (temp.EndsWith("\n"))
                            {
                                data = stringBuilder.ToString().TrimEnd();
                                stringBuilder.Length = 0;
                                await Console.Out.WriteLineAsync($"data:{data}\n");
                                EkLog.Information($"data:{data}", true);
                                #region 向网络流中回写入数据
                                /**
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
                                */
                                #endregion
                            }
                            else
                            {
                                EkLog.Warning($"数据获取格式异常|{temp}|\n", true);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // 一般是端口被占用
                EkLog.Error($"TcpListener异常\n{e.Message}");
            }
            finally
            {
                // 停止侦听
                listener.Stop();
            }
        }
    }
}
