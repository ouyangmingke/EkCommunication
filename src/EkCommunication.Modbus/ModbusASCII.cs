using EkCommunication.EKSerialPort;
using EkCommunication.Modbus.Core;

namespace EkCommunication.Modbus
{
    /// <summary>
    /// Modbus-ASCII 协议
    /// </summary>
    public class ModbusASCII : ModbusRTU
    {
        static EKSerialPortClient eKSerialPortClient = new EKSerialPortClient();
        protected override byte[] DataProcessing(byte[] value)
        {
            value = SoftLRC.LRC(value);
            value = SoftBasic.BytesToAsciiBytes(value);
            var array = SoftBasic.SpliceByteArray(
                new byte[1] { 58 },// 标头 ':'
                value,
                new byte[2] { 13, 10 }// 结束符 '\r''\n'
                );
            return array;
        }

        protected override bool DataValidation(byte[] value)
        {
            value = SoftBasic.AsciiBytesToBytes(value);
            return SoftLRC.CheckLRC(value);
        }
    }
}
