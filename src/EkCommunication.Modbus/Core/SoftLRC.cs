using System;

namespace EkCommunication.Modbus.Core
{
    public class SoftLRC
    {
        public static byte[] LRC(byte[] value)
        {
            if (value == null) return null;
            var num = 0;
            for (var i = 0; i < value.Length; i++) num += value[i];

            num %= 256;// 丢弃进位

            num = 256 - num;

            var bytes = new byte[1]
            {
                (byte) num
            };
            return SoftBasic.SpliceTwoByteArray(value, bytes);
        }
  
        /// <summary>
        /// LRC 校验
        /// 需要将设备数据转回 hex 数据再校验
        /// </summary>
        /// <param name="value">设备回复数据</param>
        /// <returns></returns>
        public static bool CheckLRC(byte[] value)
        {
            if (value == null) return false;
            var num = value.Length;
            var array = new byte[num - 1];
            Array.Copy(value, 0, array, 0, array.Length);
            var array2 = LRC(array);
            if (array2[num - 1] == value[num - 1]) return true;
            return false;
        }
    }
}
