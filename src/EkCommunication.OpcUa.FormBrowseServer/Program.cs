namespace EkCommunication.OpcUa.FormBrowseServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var formBrowseServer = new OpcUaHelper.Forms.FormBrowseServer("opc.tcp://192.168.10.201:49320");
            formBrowseServer.ShowDialog();
        }
    }
}
