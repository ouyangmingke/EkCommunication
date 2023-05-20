namespace EkCommunication.Modbus.Tests
{
    [TestClass()]
    public class ModbusRTUTests
    {

        public string IntToHexString(int data)
        {
            var c1 = data / 256;
            var c2 = data % 256;
            return $"{c1:x2}{c2:x2}";
        }
        /// <summary>
        /// Modbus RTU 读取数据
        /// </summary>
        [TestMethod()]
        public void SendTest()
        {
            var modbusRTU = new ModbusRTU();
            //  数据需要使用16进制
            var node = "01";// 节点ID 一字节长度
            var startIndex = IntToHexString(0);// 读取开始地址
            var readLen = IntToHexString(10);  // 读取长度 
            var collection = modbusRTU.ReadData(node, startIndex, readLen);
            foreach (var item in collection)
            {
                var value = item[0] * 256 + item[1];
                Console.WriteLine(value);
            }
        }
    }
}