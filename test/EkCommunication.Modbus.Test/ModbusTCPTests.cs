using Microsoft.VisualStudio.TestTools.UnitTesting;
using EkCommunication.Modbus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EkCommunication.EkTCP;
using EkCommunication.Modbus.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EkCommunication.Modbus.Tests
{
    [TestClass()]
    public class ModbusTCPTests
    {
        [TestMethod()]
        public async Task ReadDataTest()
        {
            TcpClientHelper tcpClientHelper = new TcpClientHelper("127.0.0.1", 502);
            var message = SoftBasic.HexStringToBytes("00 01 00 00 00 06 01 03 00 00 00 02");
            var collection
                    = await tcpClientHelper.ReadAsync(message);
            await Console.Out.WriteLineAsync(SoftBasic.ByteToHexString(collection));
        }
        public string IntToHexString(int data)
        {
            var c1 = data / 256;
            var c2 = data % 256;
            return $"{c1:x2}{c2:x2}";
        }
        [TestMethod()]
        public async Task ReadDataTest1()
        {
            var modbusTCP = new ModbusTCP();
            //  数据需要使用16进制
            var node = "01";// 节点ID 一字节长度
            var startIndex = IntToHexString(0);// 读取开始地址
            var readLen = IntToHexString(2);  // 读取长度 
            var collection = await modbusTCP.ReadDataAsync(node, startIndex, readLen);
            foreach (var item in collection)
            {
                var value = item[0] * 256 + item[1];
                Console.WriteLine(value);
            }
        }
    }
}