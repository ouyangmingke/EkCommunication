using EkCommunication.EkSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EkCommunicationClient.CommunicationView
{
    /// <summary>
    /// SocketControl.xaml 的交互逻辑
    /// </summary>
    public partial class SocketControl : UserControl
    {
        public SocketControl()
        {
            InitializeComponent();
        }
        #region Socket
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            
            var ip = Txt_SocketIp.Text;
            var port = int.Parse(Txt_SocketPort.Text);
            var msg = TB_SocketSendClient.Text;
            var socketState = Cb_socketState.Text;
            //Task.Run(async () =>
            //{
            //    await SocketClientHelper.Instance.SendMessageAsync(socketState, msg, ip, port);
            //});
        }
        private void Receive_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception($"未实现");

            //TB_SocketClient.Dispatcher.BeginInvoke(new Action(async () =>
            //{
            //    var ip = Txt_SocketIp.Text;
            //    var port = int.Parse(Txt_SocketPort.Text);
            //    var socketState = Cb_socketState.Text;
            //    var msg = await SocketServerHelper.Instance.ReceiveMessageAsync(socketState, ip, port);

            //    if (TB_SocketClient.LineCount > 100)
            //    {
            //        TB_SocketClient.Clear();
            //    }
            //    TB_SocketClient.Text += $"{msg}\n";
            //    Scroll_SocketClient.ScrollToEnd();
            //}));
        }
        #endregion
    }
}
