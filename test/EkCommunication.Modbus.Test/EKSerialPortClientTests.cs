using Microsoft.VisualStudio.TestTools.UnitTesting;
using EkCommunication.EKSerialPort;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using EkCommunication.Modbus.Core;

namespace EkCommunication.EKSerialPort.Tests
{
    [TestClass()]
    public class EKSerialPortClientTests
    {
        static SerialPort _SerialPort = new SerialPort();
        public EKSerialPortClientTests()
        {
            Init(_SerialPort, "COM5");
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
            serialPort.BaudRate = 115100;// 波特率
            serialPort.Parity = Parity.Even;// 奇偶校验
            serialPort.DataBits = 8;// 数据长度
            serialPort.StopBits = StopBits.One;// 停止位
            serialPort.Handshake = Handshake.None;// 握手信号
            serialPort.ReadTimeout = 100;// 读取超时
            serialPort.WriteTimeout = 100;// 写入超时
        }
        public string IntToHexString(int data)
        {
            var c1 = data / 256;
            var c2 = data % 256;
            return $"{c1:x2}{c2:x2}";
        }
        [TestMethod()]
        public void SendTest()
        {

            Queue<string> strings = new Queue<string>();
            //strings.Enqueue("00 00 00");
            strings.Enqueue("10 00 00");
            strings.Enqueue("00 10 00");
            strings.Enqueue("00 00 10");
            var back = "00 00 00";
            var groupAddress = "00 00";
            var deviceAddress = "00 01";
            var port = "00";
            var gn = "99";
            var typ = "01";
            var reserve = "00 00";
            int x = 0;
            int ind = 0;
            while (x++ < 10)
            {
                var groupSum = 3;
                var len = IntToHexString(3 * groupSum); // 颜色数据长度
                var extend = IntToHexString(2);// 复制灯光样式
                var co = "";// 一组灯光样式

                for (int i = 0; i < groupSum; i++)
                {

                    var temp = strings.Dequeue();
                    strings.Enqueue(temp);
                    co += temp + " ";
                }
                var ac = $"{groupAddress} {deviceAddress} {port} {gn} {typ} {reserve} {len} {extend} {co}";
                var data = $"DD 55 EE{ac} AA BB";
                Console.WriteLine(data);
                var message = SoftBasic.HexStringToBytes(data);
                Send(message);
                var temp1 = strings.Dequeue();
                strings.Enqueue(temp1);
                Thread.Sleep(1000);
            }



        }
        public void Send(byte[] data)
        {
            _SerialPort.Open();

            if (data != null && data.Length != 0)
                try
                {
                    _SerialPort.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                }
            _SerialPort.Close();

        }
    }
}