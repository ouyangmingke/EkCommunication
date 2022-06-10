using System.IO.Ports;

namespace EkCommunication.EKSerialPort
{
    public class EKSerialPortClient
    {
        static SerialPort _SendSerialPort = new SerialPort();
        static SerialPort _ReceiveSerialPort = new SerialPort();
        public EKSerialPortClient()
        {
            Init(_SendSerialPort, "COM3");
            Init(_ReceiveSerialPort, "COM4");
        }

        void Init(SerialPort serialPort, string portName)
        {
            serialPort.PortName = portName;// 串口号
            serialPort.BaudRate = 9600;// 波特率
            serialPort.Parity = Parity.None;// 奇偶校验
            serialPort.DataBits = 8;// 数据长度
            serialPort.StopBits = StopBits.One;// 停止位
            serialPort.Handshake = Handshake.None;// 握手信号
            serialPort.ReadTimeout = 500;// 读取超时
            serialPort.WriteTimeout = 500;// 写入超时
        }

        public void Send(string msg)
        {
            // 检索当前计算机的有效端口 判断目标串口是否存在
            foreach (var item in SerialPort.GetPortNames())
            {
                if (item == "COM4") {
                    _SendSerialPort.Open();
                    _SendSerialPort.WriteLine(msg);
                    _SendSerialPort.Close();
                }
            }
        
        }

        public void Receive()
        {
            _ReceiveSerialPort.Open();
            while (true)
            {
                try
                {
                    string message = _ReceiveSerialPort.ReadLine();
                    Console.WriteLine($"读取到{message}");
                }
                catch (TimeoutException)
                {

                }
            }

        }
    }
}