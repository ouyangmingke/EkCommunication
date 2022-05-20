using EkCommunication.OpcUa.Models;

using EkCommunicationClient.EkViewModel;
using EkCommunicationClient.Managers;

using Microsoft.Extensions.Logging;

using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace EkCommunicationClient.CommunicationView
{
    /// <summary>
    /// SocketControl.xaml 的交互逻辑
    /// </summary>
    public partial class OpcUaControl : UserControl
    {
        public OpcUaControl()
        {
            InitializeComponent();
        }
        readonly OpcUaToolManager opcUaToolManager = new OpcUaToolManager(new LoggerFactory());
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            var res = opcUaToolManager.Connect(Txt_Address.Text);
            Home.IsEnabled = res;
        }

        /// <summary>
        /// 获取指定节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RelationNode_Click(object sender, RoutedEventArgs e)
        {
            GetServerNodes(TB_NodeId.Text, _TreesRoot);
        }

        TreeNodeViewModel _TreesRoot = new TreeNodeViewModel("TreesRoot");

        private void GetServerNodes(string nodeId, TreeNode trees)
        {
            _TreesRoot.ReplaceChildn(opcUaToolManager.GetAllRelationNodeOfNodeId(nodeId).ToArray());
            ServerBrowser.ItemsSource = _TreesRoot.Children;
        }

        private void Label_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem label)
            {
                var id = label.Header.ToString();
                foreach (var node in _TreesRoot.Children)
                {
                    if (node.NodeName == id)
                    {
                        node.AddChildren(opcUaToolManager.GetAllRelationNodeOfNodeId(node.NodeId).ToArray());
                        //ServerBrowser.ItemsSource = new string[1];
                        //ServerBrowser.ItemsSource = _TreesRoot.Children;

                    }

                }
                label.Dispatcher.Invoke(() =>
                {
                    Thread.Sleep(1000);
                    label.IsExpanded = true;
                    label.IsSelected = true;
                });
                //GetServerNodes(node.NodeId, node);

            }
        }

        private void ServerBrowser_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {


        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (e.Source is TreeView tree)
            {
                if (tree.SelectedItem is TreeNodeViewModel treeNodeViewModel)
                {
                    var node = opcUaToolManager.GetAllRelationNodeOfNodeId(treeNodeViewModel.NodeId).ToArray();
                    NodeInfo.ItemsSource = node;
                    treeNodeViewModel.AddChildren(node);
                }
            }
        }

        private void NodeAttribute_Click(object sender, RoutedEventArgs e)
        {
            opcUaToolManager.GetCurrentNodeAttributes(TB_NodeId.Text);
        }
    }
}
