using EkCommunication.EKSerialPort;
using EkCommunication.Modbus.Core;

namespace EkCommunication.Modbus
{
    public class ModbusRTU
    {
        static EKSerialPortClient eKSerialPortClient = new EKSerialPortClient();
        public byte[]? Send(string data)
        {
            var message = SoftBasic.HexStringToBytes(data);
            var bytes = DataProcessing(message);
            var str = SoftBasic.ByteToHexString(bytes, ' ');
            Console.WriteLine(str);
            return eKSerialPortClient.ReadBase(bytes);
        }

        /// <summary>
        /// 读取数据 0x03
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="length">读取长度</param>
        /// <returns></returns>
        public virtual List<byte[]> ReadData(string nodeID, string startIndex, string readLen)
        {
            var value = new List<byte[]>();
            var requestData = nodeID + "03" + startIndex + readLen;
            var bytes = Send(requestData);
            if (bytes == null || !DataValidation(bytes))
            {
                Console.WriteLine("数据验证失败");
                return value;
            }
            var dataLen = bytes[2];
            var data = new byte[dataLen];
            Array.Copy(bytes, 3, data, 0, dataLen);
            return SoftBasic.ArraySplitByLength(data, 2);
        }

        /// <summary>
        /// 数据加工
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual byte[] DataProcessing(byte[] value)
        {
            return SoftCRC16.CRC16(value);
        }

        /// <summary>
        /// 数据验证
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool DataValidation(byte[] value)
        {
            return SoftCRC16.CheckCRC16(value);
        }

    }
}