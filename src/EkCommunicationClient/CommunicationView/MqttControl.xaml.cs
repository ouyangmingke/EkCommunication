using EkCommunication.EkSocket;
using EkCommunication.Mqtt;

using System;
using System.Windows;
using System.Windows.Controls;

namespace EkCommunicationClient.CommunicationView
{
    /// <summary>
    /// SocketControl.xaml 的交互逻辑
    /// </summary>
    public partial class MqttControl : UserControl
    {
        public MqttControl()
        {
            InitializeComponent();
        }
        #region Socket
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            MqttClientHelper.Instance.Conn();

        }
        private void Receive_Click(object sender, RoutedEventArgs e)
        {
            TB_SocketClient.Dispatcher.BeginInvoke(new Action(async () =>
            {
                var ip = Txt_SocketIp.Text;
                var port = int.Parse(Txt_SocketPort.Text);
                var socketState = Cb_socketState.Text;
                var msg = await SocketClientHelper.Instance.ReceiveMessageAsync(socketState, ip, port);

                if (TB_SocketClient.LineCount > 100)
                {
                    TB_SocketClient.Clear();
                }
                TB_SocketClient.Text += $"{msg}\n";
                Scroll_SocketClient.ScrollToEnd();
            }));
        }
        #endregion
    }
}
