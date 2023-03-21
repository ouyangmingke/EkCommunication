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
            txtTargetNode.Text = "ns=2;s=ͨ�� 3.demo.��1";
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

        // ��������
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
                Console.WriteLine("���ӳɹ�");
                btn_GetNodes.Enabled = true;
            }
            else
            {
                btn_GetNodes.Enabled = false;
                Console.WriteLine("����ʧ��");
            }

        }
        /// <summary>
        /// ��ȡ�����ڵ�
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
        /// ͬ����
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
        /// �첽������������
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
            var buf = "�����첽����ʼ������";
            foreach (var item in dataValues)
            {
                var tmp = $"nodeId:{item.Key}\nvalue:{item.Value}\n";
                buf += tmp;
            }
            txtDataInfo.Text = buf;

        }

        private string[] MonitorNodeTags = null;

        /// <summary>
        /// �������Ľڵ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubscription_Click(object sender, EventArgs e)
        {
            MonitorNodeTags = NodeTreeCheckedItems();
            opcClient.BatchNodeIdDatasSubscription("B", MonitorNodeTags, SubCallback);
        }

        /// <summary>
        /// �ص�����
        /// </summary>
        /// <param name="key"></param>
        /// <param name="monitoredItem"></param>
        /// <param name="args"></param>
        private void SubCallback(string key, MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs args)
        {
            // �� InvokeRequired Ϊ true ʱ˵�����ⲿ�߳�Ҫ����
            if (InvokeRequired)
            {
                // ί�е����߳�ִ��
                Invoke(new Action<string, MonitoredItem, MonitoredItemNotificationEventArgs>(SubCallback), key, monitoredItem, args);
                return;
            }

            if (key == "A")
            {
                // ����ж���Ķ���ֵ�������˵�ǰ�ķ���������ͨ��key��monitoredItem������
                MonitoredItemNotification notification = args.NotificationValue as MonitoredItemNotification;
                if (notification != null)
                {
                    //richTextBox1.Text += notification.Value.WrappedValue.Value.ToString() + "\n";
                }
            }
            else if (key == "B")
            {
                // ��Ҫ���ֳ���ÿ����ͬ�Ľڵ���Ϣ
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
            //     "ns=2;s=ͨ�� 1.�豸 1.��� 1",
            //     "ns=2;s=ͨ�� 1.�豸 1.��� 2"
            //};
            //object[] nodesValues = new object[]
            //{
            //    Convert.ToUInt16(textBox2.Text),
            //    Convert.ToUInt16(textBox3.Text)
            //};
            //bool Flag = opcClient.BatchWriteNodeIds(nodeids, nodesValues);

        }
        /// <summary>
        /// ����д������
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
        /// ��ȡ�ڵ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNodeAtrr_Click(object sender, EventArgs e)
        {
            StringBuilder buf = new StringBuilder();
            foreach (var item in NodeTreeCheckedItems())
            {
                OpcUaHelper.OpcNodeAttribute[] opcNodeAttributes = opcClient.GetCurrentNodeAttributes(item);
                buf.AppendLine($"{item}\n����:");
                foreach (var tmp in opcNodeAttributes)
                {
                    buf.AppendLine($"{tmp.Name}   :  {tmp.Value}");
                }
                buf.AppendLine();
            }
            txtDataInfo.Text = buf.ToString(); ;
        }

        /// <summary>
        /// ��ȡ�����ڵ����ʷ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHisData_Click(object sender, EventArgs e)
        {
            //DateTime startTime = dateTimePicker1.Value;
            //DateTime endTime = dateTimePicker2.Value;

            //string nodeId = "ns=2;s=ͨ�� 1.�豸 1.��� 1";

            //List<DataValue> results = opcClient.ReadSingleNodeIdHistoryDatas(nodeId, startTime, endTime);
            //if (results == null)
            //{
            //    return;
            //}
            //string buf = "��ȡ�ڵ���ʷ���ݡ�����\n";
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