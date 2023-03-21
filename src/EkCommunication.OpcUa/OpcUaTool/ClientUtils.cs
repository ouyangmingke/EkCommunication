namespace EkCommunication.OpcUa.OpcUaTool
{
    public delegate void Message(string msg);
    public static class ClientUtils
    {
        public static event Message Message;

        public static void HandleException(string msg, Exception ex)
        {
            Message?.Invoke($"{msg}\n{ex.Message}\n");
        }
    }
}
