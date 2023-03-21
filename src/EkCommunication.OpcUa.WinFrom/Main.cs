using EkCommunication.OpcUa.OpcUaTool;

using Microsoft.Extensions.Logging;

using Opc.Ua;
using Opc.Ua.Client;

using OpcUaTool;

using System.Text;

namespace WinFormsAppCore
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Load += Main_Load;
            opcClient = new OPCUAHelper();
        }

        private void Main_Load(object? sender, EventArgs e)
        {
            txtAddress.Text = "opc.tcp://192.168.10.201:49320";
            txtTargetNode.Text = "ns=2;s=通道 3.demo.组1";
            ClientUtils.Message += ClientUtils_Message;
        }

        private void ClientUtils_Message(string msg)
        {
            txtDataInfo.Invoke(new Action(() =>
            {
                txtLog.Text += $"{msg}";
            }));
        }

        OPCUAHelper opcClient;

        // 测试连接
        private void button1_Click(object sender, EventArgs e)
        {
            //using (FormBrowseServer form = new FormBrowseServer())
            //{
            //    form.ShowDialog();
            //}
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string address = txtAddress.Text;
            string user = txtUser.Text;
            string pwd = txtPwd.Text;
            if (string.IsNullOrWhiteSpace(user))
            {
                opcClient.OpenConnectOfAnonymous(address);
            }
            else
                opcClient.OpenConnectOfAccount(address, user, pwd);
            if (opcClient.ConnectStatus)
            {
                Console.WriteLine("连接成功");
                btn_GetNodes.Enabled = true;
            }
            else
            {
                btn_GetNodes.Enabled = false;
                Console.WriteLine("连接失败");
            }

        }
        /// <summary>
        /// 获取关联节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_GetNode_Click(object sender, EventArgs e)
        {
            var coll = opcClient.GetAllRelationNodeOfNodeId(txtTargetNode.Text);
            NodeTree.Items.Clear();
            if (coll != null)
            {
                foreach (var item in coll)
                {
                    NodeTree.Items.Add(item.NodeId);
                }
            }

        }
        /// <summary>
        /// 同步读
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSyncRead_Click(object sender, EventArgs e)
        {
            txtDataInfo.Text = "";
            List<NodeId> nodeIds = new List<NodeId>();

            foreach (var item in NodeTree.CheckedItems)
            {
                nodeIds.Add(item.ToString());
            }
            Dictionary<string, DataValue> dataValues = opcClient.GetBatchNodeDatasOfSync(nodeIds);
            var buf = "";
            foreach (var item in dataValues)
            {
                var tmp = $"nodeId:{item.Key}\nvalue:{item.Value}\n";
                buf += tmp;
            }
            txtDataInfo.Text = buf;
        }
        /// <summary>
        /// 异步读【批量读】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnAsyncRead_Click(object sender, EventArgs e)
        {
            txtDataInfo.Text = "";
            List<NodeId> nodeIds = new List<NodeId>();

            foreach (var item in NodeTree.CheckedItems)
            {
                nodeIds.Add(item.ToString());
            }
            Dictionary<string, DataValue> dataValues = await opcClient.GetBatchNodeDatasByDictionaryAsync(nodeIds);
            var buf = "批量异步读开始。。。";
            foreach (var item in dataValues)
            {
                var tmp = $"nodeId:{item.Key}\nvalue:{item.Value}\n";
                buf += tmp;
            }
            txtDataInfo.Text = buf;

        }

        private string[] MonitorNodeTags = null;

        /// <summary>
        /// 批量订阅节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubscription_Click(object sender, EventArgs e)
        {
            MonitorNodeTags = NodeTreeCheckedItems();
            opcClient.BatchNodeIdDatasSubscription("B", MonitorNodeTags, SubCallback);
        }

        /// <summary>
        /// 回调方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="monitoredItem"></param>
        /// <param name="args"></param>
        private void SubCallback(string key, MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs args)
        {
            // 当 InvokeRequired 为 true 时说明有外部线程要访问
            if (InvokeRequired)
            {
                // 委托到主线程执行
                Invoke(new Action<string, MonitoredItem, MonitoredItemNotificationEventArgs>(SubCallback), key, monitoredItem, args);
                return;
            }

            if (key == "A")
            {
                // 如果有多个的订阅值都关联了当前的方法，可以通过key和monitoredItem来区分
                MonitoredItemNotification notification = args.NotificationValue as MonitoredItemNotification;
                if (notification != null)
                {
                    //richTextBox1.Text += notification.Value.WrappedValue.Value.ToString() + "\n";
                }
            }
            else if (key == "B")
            {
                // 需要区分出来每个不同的节点信息
                MonitoredItemNotification notification = args.NotificationValue as MonitoredItemNotification;
                //if (monitoredItem.StartNodeId.ToString() == MonitorNodeTags[0])
                //{
                //    txtDataInfo.Text = notification.Value.WrappedValue.Value.ToString();
                //}
                //else if (monitoredItem.StartNodeId.ToString() == MonitorNodeTags[1])
                //{
                //    txtDataInfo.Text = notification.Value.WrappedValue.Value.ToString();
                //}
                //else if (monitoredItem.StartNodeId.ToString() == MonitorNodeTags[2])
                //{
                //}
                txtDataInfo.Text += $"{monitoredItem.StartNodeId}:{notification.Value.WrappedValue.Value} \n";
            }
        }

        private void btnCloseMo_Click(object sender, EventArgs e)
        {
            opcClient.CancelAllNodeIdDatasSubscription();
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            //string[] nodeids = new string[]
            //{
            //     "ns=2;s=通道 1.设备 1.标记 1",
            //     "ns=2;s=通道 1.设备 1.标记 2"
            //};
            //object[] nodesValues = new object[]
            //{
            //    Convert.ToUInt16(textBox2.Text),
            //    Convert.ToUInt16(textBox3.Text)
            //};
            //bool Flag = opcClient.BatchWriteNodeIds(nodeids, nodesValues);

        }
        /// <summary>
        /// 单次写入数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSlmWriter_Click(object sender, EventArgs e)
        {
            string nodeId = txtTargetNode.Text;
            var value = textBox3.Text;
            //bool Flag = await opcClient.WriteSingleNodeIdOfAsync(nodeId, value);
            bool Flag1 = opcClient.WriteSingleNodeIdOfSync(nodeId, value);
        }
        /// <summary>
        /// 获取节点属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNodeAtrr_Click(object sender, EventArgs e)
        {
            StringBuilder buf = new StringBuilder();
            foreach (var item in NodeTreeCheckedItems())
            {
                OpcUaHelper.OpcNodeAttribute[] opcNodeAttributes = opcClient.GetCurrentNodeAttributes(item);
                buf.AppendLine($"{item}\n属性:");
                foreach (var tmp in opcNodeAttributes)
                {
                    buf.AppendLine($"{tmp.Name}   :  {tmp.Value}");
                }
                buf.AppendLine();
            }
            txtDataInfo.Text = buf.ToString(); ;
        }

        /// <summary>
        /// 获取单个节点的历史数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHisData_Click(object sender, EventArgs e)
        {
            //DateTime startTime = dateTimePicker1.Value;
            //DateTime endTime = dateTimePicker2.Value;

            //string nodeId = "ns=2;s=通道 1.设备 1.标记 1";

            //List<DataValue> results = opcClient.ReadSingleNodeIdHistoryDatas(nodeId, startTime, endTime);
            //if (results == null)
            //{
            //    return;
            //}
            //string buf = "获取节点历史数据。。。\n";
            //foreach (var item in results)
            //{
            //    buf += item.ToString() + "\n";
            //}
            //richTextBox1.Text = buf;
        }

        private void butRead_Click(object sender, EventArgs e)
        {
            var a = opcClient.GetCurrentNodeValue(txtTargetNode.Text);
            txtDataInfo.Text += $"{a}\n";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var uaToolManager = new OpcUaToolManager(new LoggerFactory());
            uaToolManager.Connect(txtAddress.Text);
            txtDataInfo.BeginInvoke(new Action(async () =>
            {
                var t = await uaToolManager.CurrentPartAsync();
                txtDataInfo.Text += $"Res==> {t} \n";
            }));
        }

        private string[] NodeTreeCheckedItems()
        {
            string[] arr = new string[NodeTree.CheckedItems.Count];
            int b = 0;
            foreach (var outstr in NodeTree.CheckedItems)
            {
                arr[b] = outstr.ToString();
                b++;
            }

            return arr;
        }
    }
}