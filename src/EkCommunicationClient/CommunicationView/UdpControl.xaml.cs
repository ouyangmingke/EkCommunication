using EkCommunication.EkSocket;
using EkCommunication.EkTCP;
using EkCommunication.EkUDP;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EkCommunicationClient.CommunicationView
{
    /// <summary>
    /// UdpControl.xaml 的交互逻辑
    /// </summary>
    public partial class UdpControl : UserControl
    {
        public UdpControl()
        {
            InitializeComponent();
            Loaded += UdpControl_Loaded;
        }

        private void UdpControl_Loaded(object sender, RoutedEventArgs e)
        {
            Notes.Content = "Udp需要先等待接收再发送消息";
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            var ip = Txt_UdpIp.Text;
            var port = int.Parse(Txt_UdpPort.Text);
            var msg = TB_UdpSendClient.Text;
            Task.Run(async () =>
            {
                //await   SocketServerHelper.Instance.SendMessageAsync("UDP", msg, ip, port);
                await UdpClientHelper.Instance.SendMessageAsync(msg, ip, port);
                //AsynchronousClient.StartClient();
            });
        }
        private void Receive_Click(object sender, RoutedEventArgs e)
        {
            TB_UdpClient.Dispatcher.BeginInvoke(new Action(() =>
           {
               var ip = Txt_UdpIp.Text;
               var port = int.Parse(Txt_UdpPort.Text);
               Task.Run(async () => { await UdpClientHelper.Instance.ReceiveMessageAsync(ip, port); });
           }));
        }


    }
}
