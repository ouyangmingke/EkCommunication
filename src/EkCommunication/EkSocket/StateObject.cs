using System.Net.Sockets;
using System.Text;

namespace EkCommunication.EkSocket
{
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    public class SocketState
    {
        public const string TCP = "TCP";
        public const string UDP = "UDP";
    }
}
