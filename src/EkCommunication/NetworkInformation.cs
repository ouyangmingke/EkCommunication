using System.Net.NetworkInformation;
using System.Net;

namespace EkCommunication
{
    /// <summary>
    /// 网络信息
    /// </summary>
    internal class NetworkInformation
    {
        private NetworkInformation() { }

        private static readonly NetworkInformation Current = new NetworkInformation();
        public static NetworkInformation Instance => Current;

        /// <summary>
        /// 端口是否使用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool PortInUse(int port)
        {
            bool inUse = false;
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
            if (ipEndPoints.Any(t => t.Port == port))
            {
                inUse = true;
            }
            return inUse;
        }
    }
}
