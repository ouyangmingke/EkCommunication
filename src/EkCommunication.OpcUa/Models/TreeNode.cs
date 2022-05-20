namespace EkCommunication.OpcUa.Models
{
    /// <summary>
    /// OpcUa 树节点对象
    /// </summary>
    public abstract class TreeNode
    {
        public string NodeName { get; set; }
        public string NodeType { get; set; }
        public string NodeId { get; set; }
        public string NodeValue { get; set; }
    }
}
