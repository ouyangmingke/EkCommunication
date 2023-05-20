using EkCommunication.EKSerialPort;
using EkCommunication.EkTCP;
using EkCommunication.Modbus.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkCommunication.Modbus
{
    public class ModbusTCP
    {
        TcpClientHelper tcpClientHelper = new TcpClientHelper("127.0.0.1", 502);
        static long workId = 0;
        public async Task<byte[]?> SendAsync(string data)
        {
            var message = SoftBasic.HexStringToBytes(data);
            return await tcpClientHelper.ReadAsync(message);
        }
        public string IntToHexString(long data)
        {
            var c1 = data / 256;
            var c2 = data % 256;
            return $"{c1:x2}{c2:x2}";
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="nodeID"></param>
        /// <param name="startIndex"></param>
        /// <param name="readLen"></param>
        /// <returns></returns>
        public virtual async Task<List<byte[]>> ReadDataAsync(string nodeID, string startIndex, string readLen)
        {
            var modbusData = nodeID + "03" + startIndex + readLen;
            var requestData = $"{IntToHexString(workId++)} 00 00 00 06 {modbusData}";
            Console.WriteLine(requestData);
            var bytes = await SendAsync(requestData);
            var str = SoftBasic.ByteToHexString(bytes, ' ');
            Console.WriteLine(str);
            var dataLen = bytes[8];
            var data = new byte[dataLen];
            Array.Copy(bytes, 9, data, 0, dataLen);
            return SoftBasic.ArraySplitByLength(data, 2);
        }
    }
}
