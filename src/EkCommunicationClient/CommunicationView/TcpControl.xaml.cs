using EkCommunication.EkTCP;

using System;
using System.Collections.Generic;
using System.Linq;
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
    /// TcpControl.xaml 的交互逻辑
    /// </summary>
    public partial class TcpControl : UserControl
    {
        public TcpControl()
        {
            InitializeComponent();
        }
        #region TCP
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception($"未实现");

            //var ip = Txt_TcpIp.Text;
            //var port = int.Parse(Txt_TcpPort.Text);
            //var msg = TB_TcpSendClient.Text;
            //Task.Run(async () =>
            //{
            //    await TcpClientHelper.Instance.SendMessageAsync(msg, ip, port);
            //});
        }
        private void Receive_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception($"未实现");

            //TB_TcpClient.Dispatcher.BeginInvoke(new Action(async () =>
            //{
            //    var ip = Txt_TcpIp.Text;
            //    var port = int.Parse(Txt_TcpPort.Text);
            //    while (true)
            //    {
            //        await TcpServerHelper.Instance.StartTcpListenerAsync(ip, port);
            //    }
            //}));
        }
        #endregion
    }
}
