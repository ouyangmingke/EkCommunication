using EkCommunication.OpcUa.Models;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace EkCommunicationClient.EkViewModel
{
    public class TreeNodeViewModel : TreeNode
    {
        public TreeNodeViewModel(string name)
        {
            NodeName = name;
            Children = new ObservableCollection<TreeNodeViewModel>();
        }
        public ObservableCollection<TreeNodeViewModel> Children { get; set; }
        public void AddChildren(params TreeNodeViewModel[] treeNodes)
        {
            Children.Clear();
            foreach (var item in treeNodes)
            {
                Children.Add(item);
            }
        }
        public void ReplaceChildn(TreeNodeViewModel[] treeNodes) {
            Children.Clear();
            foreach (var item in treeNodes)
            {
                Children.Add(item);
            }
        }
    }
}
