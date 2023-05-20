using EkTools.EkLog;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EkCommunication.EkTCP
{

    public class TcpClientHelper
    {
        public TcpClientHelper(string ip, int port)
        {
            _targetIpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            tcpClient = new TcpClient();
        }
        /// <summary>
        /// 目标地址
        /// </summary>
        readonly IPEndPoint _targetIpEndPoint;
        private TcpClient tcpClient;

        ~TcpClientHelper()
        {
            tcpClient.Dispose();
        }
        /// <summary>
        /// 线程锁定
        /// </summary>
        AutoResetEvent autoResetEvent = new AutoResetEvent(true);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<bool> SendMessage(byte[] data)
        {
            try
            {
                autoResetEvent.WaitOne();
                // 创建 TCP 客户端实例并连接到指定 终结点
                try
                {
                    if (!tcpClient.Connected)
                        tcpClient.Connect(_targetIpEndPoint);
                }
                catch (Exception w)
                {
                    Console.WriteLine("连接失败" + w.Message + "\n");
                    autoResetEvent.Set();
                    return false;
                }
                // 获取当前连接的网络传输流
                var networkStream = tcpClient.GetStream();
                //byte[] res = Encoding.UTF8.GetBytes(message);
                //写入数据
                await networkStream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n");
                // 断开连接后需要创建新的TCP客户端
                tcpClient = new TcpClient();
                return false;
            }
            autoResetEvent.Set();
            return true;
        }

        public async Task<byte[]> ReadAsync(byte[] data)
        {
            try
            {
                if (!tcpClient.Connected)
                    tcpClient.Connect(_targetIpEndPoint);
            }
            catch (Exception w)
            {
                Console.WriteLine("连接失败" + w.Message + "\n");
                autoResetEvent.Set();
            }
            // 获取当前连接的网络传输流
            var networkStream = tcpClient.GetStream();
            await networkStream.WriteAsync(data, 0, data.Length);
            var memoryStream = new MemoryStream();
            var bytes = new byte[1024];
            int bytesRead=0;
            while ((bytesRead = await networkStream.ReadAsync(bytes, 0, 1024)) != 0)
            {
                if (bytesRead > 0)
                {
                    memoryStream.Write(bytes, 0, bytesRead);
                    break;
                }
            }
            var value = memoryStream.ToArray();
            memoryStream.Dispose();
            return value;

        }
    }
}
