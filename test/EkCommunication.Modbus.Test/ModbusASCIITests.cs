using EkCommunication.Modbus.Core;

using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EkCommunication.Modbus.Tests
{
    [TestClass()]
    public class ModbusASCIITests
    {
        [TestMethod()]
        public void SendTest()
        {
            var modbusASCII = new ModbusASCII();
            var node = "01";// 节点ID
            var use = "03";// 功能码
            var startIndex = "0000";// 读取开始地址
            var readLen = "0004";// 读取长度
            var date = node + use + startIndex + readLen;

            //var collection = modbusASCII.ReadData(node, startIndex, readLen);
            var bytes = modbusASCII.Send(date);
            bytes = SoftBasic.AsciiBytesToBytes(bytes);
            var dataLen = bytes[2];
            var data = new byte[dataLen];
            Array.Copy(bytes, 3, data, 0, dataLen);
            var collection = SoftBasic.ArraySplitByLength(data, 2);
            foreach (var item in collection)
            {
                var value = item[0] * 256 + item[1];
                Console.WriteLine(value);
            }
            //var bytes = modbusASCII.Send(date);
            //var bytes2 = SoftBasic.AsciiBytesToBytes(bytes);

            //if (SoftLRC.CheckLRC(bytes2))
            //{

            //    var str = SoftBasic.ByteToHexString(bytes, ' ');
            //    Console.WriteLine(str);
            //}
            //else
            //{
            //    Console.WriteLine("数据无效");
            //}

        }
    }
}