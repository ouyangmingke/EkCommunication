namespace EkCommunication.Modbus.Core
{
    /// <summary>
    /// CRC-16 校验码计算
    /// </summary>
    public class SoftCRC16
    {
        public static bool CheckCRC16(byte[] value)
        {
            return CheckCRC16(value, 160, 1);
        }

        private static bool CheckCRC16(byte[] value, byte CH, byte CL)
        {
            if (value == null) return false;
            if (value.Length < 2) return false;
            var num = value.Length;
            var array = new byte[num - 2];
            Array.Copy(value, 0, array, 0, array.Length);
            var array2 = CRC16(array, CH, CL);
            if (array2[num - 2] == value[num - 2] && array2[num - 1] == value[num - 1]) return true;
            return false;
        }

        public static byte[] CRC16(byte[] value)
        {
            var bytes = CRC16(value, 160, 1);
            var str = SoftBasic.ByteToHexString(bytes, ' ');
            Console.WriteLine(str);
            return bytes;
        }

        private static byte[] CRC16(byte[] value, byte CH, byte CL)
        {
            // 生成数据
            var array = new byte[value.Length + 2];
            value.CopyTo(array, 0);
            var b = byte.MaxValue;
            var b2 = byte.MaxValue;
            for (var i = 0; i < value.Length; i++)
            {
                b = (byte)(b ^ value[i]);
                for (var j = 0; j <= 7; j++)
                {
                    var b3 = b2;
                    var b4 = b;
                    b2 = (byte)(b2 >> 1);// 右移 有符号高位置1 无符号高位值置0
                    b = (byte)(b >> 1);
                    if ((b3 & 1) == 1) b = (byte)(b | 0x80);
                    if ((b4 & 1) == 1)
                    {
                        b2 = (byte)(b2 ^ CH);
                        b = (byte)(b ^ CL);
                    }
                }
            }

            array[array.Length - 2] = b;
            array[array.Length - 1] = b2;
            return array;
        }
    }
}
