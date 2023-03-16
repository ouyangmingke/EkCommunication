using EkCommunication.RabbitMQ;

using System;
using System.Windows;
using System.Windows.Controls;

namespace EkCommunicationClient.CommunicationView
{
    /// <summary>
    /// Control.xaml 的交互逻辑
    /// </summary>
    public partial class RabbitMQControl : UserControl
    {
        public RabbitMQControl()
        {
            InitializeComponent();
            Loaded += RabbitMQ_Loaded;
        }

        private RabbitMQServer rabbitMQServer;
        private RabbitMQClient rabbitMQClient;
        private void RabbitMQ_Loaded(object sender, RoutedEventArgs e)
        {
            //RabbitMqMessage.RabbitMqNotify += RabbitMqMessage_RabbitMqNotify; 
            //rabbitMQServer = new RabbitMQServer("Service");
            //rabbitMQClient = new RabbitMQClient("Service");
            //rabbitMQServer.Init();
            //rabbitMQServer.ReplyQueue();
            //rabbitMQClient.Receive();
        }

        private void RabbitMqMessage_RabbitMqNotify(string msg)
        {
            TB_Client.Dispatcher.BeginInvoke(new Action(async () =>
            {
      
                if (TB_Client.LineCount > 100)
                {
                    TB_Client.Clear();
                }
                TB_Client.Text += $"{msg}\n";
                Scroll_Client.ScrollToEnd();
            }));
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            rabbitMQServer.Send(TB_SendClient.Text);
        }
     
    }
}
