
using EkCommunicationClient.CommunicationView;

using EkTools.EkLog;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EkCommunicationClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            EkLogEventBus.EkLogNotify += EkLogEventBus_EkLogNotify;
            Loaded += MainWindow_Loaded;
        }

        private void EkLogEventBus_EkLogNotify(string msg)
        {
            TB_Log.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (TB_Log.LineCount > 100)
                {
                    TB_Log.Clear();
                }
                TB_Log.Text += $"{msg}\n";
                Scroll_Log.ScrollToEnd();
            }));
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var list = new List<string>();

            for (int i = 0; i < 2; i++)
            {
                list.Add($"{i}DAAAAA");
            }
            listBoxCars.ItemsSource = list;
            //var lb = new TcpControl();
            //Top1.Content = lb;
        }

        private void CloseItem_Click(object sender, RoutedEventArgs e)
        {
            Tc_Main.Items.Clear();
        }
        /// <summary>
        /// 资源字典
        /// </summary>
        ResourceDictionary myResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("/EkCommunicationClient;component/EkWpfStyle/TabControl.xaml", UriKind.RelativeOrAbsolute) // 指定样式文件的路径
        };
        private void JumpPage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                switch (button.Content)
                {
                    case "Socket":
                        foreach (var item in Tc_Main.Items)
                        {
                            if (item is TabItem selec)
                            {
                                if (selec.Header.ToString() == "Socket")
                                {
                                    Tc_Main.SelectedItem = selec;
                                    return;
                                }
                            }
                        }
                        var socketItem = new TabItem();
                        socketItem.Header = "Socket";
                        socketItem.Content = new SocketControl();
                        socketItem.Style = myResourceDictionary["EKTabItem"] as Style;
                        Tc_Main.Items.Add(socketItem);
                        Tc_Main.SelectedItem = socketItem;
                        break;
                    case "TCP":
                        foreach (var item in Tc_Main.Items)
                        {
                            if (item is TabItem selec)
                            {
                                if (selec.Header.ToString() == "TCP")
                                {
                                    Tc_Main.SelectedItem = selec;
                                    return;
                                }
                            }
                        }
                        var tcpItem = new TabItem();
                        tcpItem.Header = "TCP";
                        tcpItem.Content = new TcpControl();
                        tcpItem.Style = myResourceDictionary["EKTabItem"] as Style;
                        Tc_Main.Items.Add(tcpItem);
                        Tc_Main.SelectedItem = tcpItem;
                        break;
                    case "UDP":
                        foreach (var item in Tc_Main.Items)
                        {
                            if (item is TabItem selec)
                            {
                                if (selec.Header.ToString() == "UDP")
                                {
                                    Tc_Main.SelectedItem = selec;
                                    return;
                                }
                            }
                        }
                        var udpItem = new TabItem();
                        udpItem.Header = "UDP";
                        udpItem.Style = myResourceDictionary["EKTabItem"] as Style;
                        udpItem.Content = new UdpControl();
                        Tc_Main.Items.Add(udpItem);
                        Tc_Main.SelectedItem = udpItem;
                        break;
                    case "Mqtt":
                        foreach (var item in Tc_Main.Items)
                        {
                            if (item is TabItem selec)
                            {
                                if (selec.Header.ToString() == "Mqtt")
                                {
                                    Tc_Main.SelectedItem = selec;
                                    return;
                                }
                            }
                        }
                        var mqttItem = new TabItem();
                        mqttItem.Header = "Mqtt";
                        mqttItem.Style = myResourceDictionary["EKTabItem"] as Style;
                        mqttItem.Content = new MqttControl();
                        Tc_Main.Items.Add(mqttItem);
                        Tc_Main.SelectedItem = mqttItem;
                        break;                    
                    case "OpcUa":
                        foreach (var item in Tc_Main.Items)
                        {
                            if (item is TabItem selec)
                            {
                                if (selec.Header.ToString() == "OpcUa")
                                {
                                    Tc_Main.SelectedItem = selec;
                                    return;
                                }
                            }
                        }
                        var OpcUaItem = new TabItem();
                        OpcUaItem.Header = "OpcUa";
                        OpcUaItem.Style = myResourceDictionary["EKTabItem"] as Style;
                        OpcUaItem.Content = new OpcUaControl();
                        Tc_Main.Items.Add(OpcUaItem);
                        Tc_Main.SelectedItem = OpcUaItem;
                        break;
                    default:
                        break;

                }
            }
        }
    }
}
