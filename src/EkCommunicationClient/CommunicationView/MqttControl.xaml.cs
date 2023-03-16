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
        private void Conn_Click(object sender, RoutedEventArgs e)
        {
            MqttClientHelper.Instance.Conn();

        }
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            TB_Topic.Dispatcher.BeginInvoke(new Action(async () =>
            {

                await MqttClientHelper.Instance.SendMessageAsync(TB_Topic.Text.Trim(), TB_Content.Text.Trim());

            }));
        }
        private void Receive_Click(object sender, RoutedEventArgs e)
        {
            TB_Topic.Dispatcher.BeginInvoke(new Action(async () =>
            {
                await MqttClientHelper.Instance.ReceiveMessageAsync(TB_Topic.Text.Trim());
            }));
        }


    }
}
