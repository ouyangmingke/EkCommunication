using System.IO.Ports;

namespace EkCommunication.EKSerialPort
{
    public class EKSerialPortClient
    {
        static SerialPort _SerialPort = new SerialPort();
        static string _port = "COM4";
        public EKSerialPortClient()
        {
            Init(_SerialPort, _port);
        }

        void Init(SerialPort serialPort, string portName)
        {
            var tf = false;
            // 检索当前计算机的有效端口 判断目标串口是否存在
            foreach (var item in SerialPort.GetPortNames())
            {
                if (item == portName)
                {
                    tf = true;
                }
            }
            if (!tf)
            {
                throw new Exception("端口不存在！！！");
            }
            serialPort.PortName = portName;// 串口号
            serialPort.BaudRate = 9600;// 波特率
            serialPort.Parity = Parity.Even;// 奇偶校验
            serialPort.DataBits = 8;// 数据长度
            serialPort.StopBits = StopBits.One;// 停止位
            serialPort.Handshake = Handshake.None;// 握手信号
            serialPort.ReadTimeout = 500;// 读取超时
            serialPort.WriteTimeout = 500;// 写入超时
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[]? ReadBase(byte[] data, bool sendOnly = false)
        {
            _SerialPort.Open();
            Send(_SerialPort, data);
            if (!sendOnly)
            {
                return Receive(_SerialPort);
            }
            //调用此方法会 SerialPort 关闭 对象并清除接收缓冲区和传输缓冲区。
            _SerialPort.Close();
            return null;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="datas"></param>
        public void Send(SerialPort serialPort, byte[] data)
        {
            if (data != null && data.Length != 0)
                try
                {
                    serialPort.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                }
        }
        /// <summary>
        /// 等待时间
        /// </summary>
        private int sleepTime = 20;

        /// <summary>
        /// 读取超时
        /// </summary>
        public int ReceiveTimeout { get; set; } = 5000;

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        public byte[] Receive(SerialPort serialPort)
        {
            var array = new byte[1024];
            var memoryStream = new MemoryStream();
            var now = DateTime.Now;
            while (true)
            {
                Thread.Sleep(sleepTime);
                try
                {
                    if (serialPort.BytesToRead >= 1)
                    {
                        var count = serialPort.Read(array, 0, array.Length);
                        if (count > 0)
                        {
                            memoryStream.Write(array, 0, count);
                            continue;
                        }
                    }
                    if ((DateTime.Now - now).TotalMilliseconds > ReceiveTimeout)
                    {
                        memoryStream.Dispose();
                        Console.WriteLine("超时");
                        return null;
                    }
                    // 未获取数据继续读取
                    if (memoryStream.Length <= 0) continue;
                }
                catch (Exception ex)
                {
                    memoryStream.Dispose();
                    Console.WriteLine("异常");
                }
                break;
            }
            var value = memoryStream.ToArray();
            memoryStream.Dispose();
            return value;
        }
    }
}