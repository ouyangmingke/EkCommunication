using System.Text;

namespace EkCommunication.Modbus.Core
{
    public class SoftBasic
    {
        private static readonly List<char> hexCharList = new List<char>
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };
        /// <summary>
        ///     字节数据转化成16进制表示的字符串 ->
        ///     Byte data into a string of 16 binary representations
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string ByteToHexString(byte[] InBytes)
        {
            return ByteToHexString(InBytes, (char)0);
        }
        /// <summary>
        ///     将16进制的字符串转化成Byte数据，将检测每2个字符转化，也就是说，中间可以是任意字符 ->
        ///     Converts a 16-character string into byte data, which will detect every 2 characters converted, that is, the middle
        ///     can be any character
        /// </summary>
        /// <param name="hex">十六进制的字符串，中间可以是任意的分隔符</param>
        /// <returns>转换后的字节数组</returns>
        /// <remarks>参数举例：AA 01 34 A8</remarks>
        public static byte[] HexStringToBytes(string hex)
        {
            // 转换为大写字符
            hex = hex.ToUpper();
            var ms = new MemoryStream();
            for (var i = 0; i < hex.Length; i++)
            {
                // 防止数据长度为单数
                if (i + 1 < hex.Length)
                {
                    var one = hex[i];// 两位一组
                    var tow = hex[i + 1];
                    // 确认字符是否为16进制字符
                    if (hexCharList.Contains(one) && hexCharList.Contains(tow))
                    {
                        // 这是一个合格的字节数据
                        var a = hexCharList.IndexOf(one) * 16;
                        var b = hexCharList.IndexOf(tow);
                        var c = (byte)(a + b);
                        ms.WriteByte(c);
                        i++;
                    }
                }
            }
            var result = ms.ToArray();
            ms.Dispose();
            return result;
        }

        /// <summary>
        ///     字节数据转化成16进制表示的字符串 ->
        ///     Byte data into a string of 16 binary representations
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <param name="segment">分割符</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string ByteToHexString(byte[] InBytes, char segment)
        {
            var sb = new StringBuilder();
            foreach (var InByte in InBytes)
                if (segment == 0) sb.Append(string.Format("{0:X2}", InByte));
                else sb.Append(string.Format("{0:X2}{1}", InByte, segment));

            if (segment != 0 && sb.Length > 1 && sb[sb.Length - 1] == segment) sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        #region Byte[] and AsciiByte[] transform

        /// <summary>
        ///     将原始的byte数组转换成ascii格式的byte数组 ->
        ///     Converts the original byte newArray to an ASCII-formatted byte newArray
        /// </summary>
        /// <param name="inBytes">等待转换的byte数组</param>
        /// <returns>转换后的数组</returns>
        public static byte[] BytesToAsciiBytes(byte[] inBytes)
        {
            return Encoding.ASCII.GetBytes(ByteToHexString(inBytes));
        }

        /// <summary>
        /// 将ascii格式的byte数组转换成原始的byte数组 ->
        /// Converts an ASCII-formatted byte array to the original byte array
        /// </summary>
        /// <param name="inBytes">等待转换的byte数组</param>
        /// <returns>转换后的数组</returns>
        public static byte[] AsciiBytesToBytes(byte[] inBytes)
        {
            return HexStringToBytes(Encoding.ASCII.GetString(inBytes));
        }
        #endregion
        #region Byte[] Splice
        /// <summary>
        ///     拼接2个字节数组成一个数组 ->
        ///     Splicing 2 bytes to to an newArray
        /// </summary>
        /// <param name="bytes1">数组一</param>
        /// <param name="bytes2">数组二</param>
        /// <returns>拼接后的数组</returns>
        public static byte[] SpliceTwoByteArray(byte[] bytes1, byte[] bytes2)
        {
            if (bytes1 == null && bytes2 == null) return null;
            if (bytes1 == null) return bytes2;
            if (bytes2 == null) return bytes1;

            var buffer = new byte[bytes1.Length + bytes2.Length];
            bytes1.CopyTo(buffer, 0);
            bytes2.CopyTo(buffer, bytes1.Length);
            return buffer;
        }
        #endregion

        /// <summary>
        /// 按顺序拼接
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] SpliceByteArray(params byte[][] bytes)
        {
            var newArrayLength = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                var obj = bytes[i];
                if (obj != null && obj.Length != 0) newArrayLength += bytes[i].Length;
            }

            var nowPosition = 0;
            var newArray = new byte[newArrayLength];
            for (var j = 0; j < bytes.Length; j++)
            {
                var obj2 = bytes[j];
                if (obj2 != null && obj2.Length != 0)
                {
                    bytes[j].CopyTo(newArray, nowPosition);
                    nowPosition += bytes[j].Length;
                }
            }
            return newArray;
        }

        #region Array Expaned

        /// <summary>
        ///     将指定的数据按照指定长度进行分割，例如int[10]，指定长度4，就分割成int[4],int[4],int[2]，然后拼接list
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="array">等待分割的数组</param>
        /// <param name="length">指定的长度信息</param>
        /// <returns>分割后结果内容</returns>
        public static List<T[]> ArraySplitByLength<T>(T[] array, int length)
        {
            if (array == null) return new List<T[]>();

            var result = new List<T[]>();
            var index = 0;
            while (index < array.Length)
                if (index + length < array.Length)
                {
                    var tmp = new T[length];
                    Array.Copy(array, index, tmp, 0, length);
                    index += length;
                    result.Add(tmp);
                }
                else
                {
                    var tmp = new T[array.Length - index];
                    Array.Copy(array, index, tmp, 0, tmp.Length);
                    index += length;
                    result.Add(tmp);
                }

            return result;
        }

        #endregion
    }
}
