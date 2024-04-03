using EkCommunication.EkSocket;
using EkCommunication.EkTCP;
using EkCommunication.EkUDP;
using EkCommunication.RabbitMQ;

using Serilog;
using Serilog.Events;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("Logs/logs.txt"))
                .CreateLogger();
            RabbitMqMessage.RabbitMqNotify += (string text) =>
            {
                Log.Warning(text);
            };

        }
        readonly SocketServerHelper tcpClientHelper = new SocketServerHelper("192.168.10.87", 888);
        int ind = 0;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Task.Run(async () =>
            {
                try
                {

                    var rabbitMQServer = new RabbitMQServer("服务端1号", "PublishQueue", "ReplyQueue", "T1");
                    //var car3 = new RabbitMQClient("客户端3号", "PublishQueue", new List<string> { });
                    //car3.Receive();

                    //for (int i = 0; i < 10; i++)
                    //{
                    //    rabbitMQServer.Send(DateTime.Now.ToString());
                    //}



                    rabbitMQServer.QueueDeclare();
                    //var cIds = new List<string> {
                    //"T1",
                    //"T2",
                    //"T3",
                    //"T4",
                    //};
                    //var car1 = new RabbitMQClient("客户端1号", "PublishQueue", cIds);
                    //car1.Receive();
                    //var car2 = new RabbitMQClient("客户端2号", "PublishQueue", new List<string> { });
                    //car2.Receive();

                }
                catch (System.Exception e)
                {

                }

                //Client.Send("hhh");
            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Task.Run(async () =>
            {
                try
                {
                }
                catch (System.Exception e)
                {

                }
            });
            //Task.Run(() => { new KafakaConsumer().ReceiveMessage(); });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
        }
    }
}
