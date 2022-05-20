using EkCommunication.OpcUa.Models;
using EkCommunication.OpcUa.OpcUaTool;

using EkCommunicationClient.EkViewModel;

using Microsoft.Extensions.Logging;

using Opc.Ua;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace EkCommunicationClient.Managers
{
    public class OpcUaToolManager
    {
        protected readonly ILogger Logger;

        private readonly OPCUAHelper _oPCUAHelper;
        public OpcUaToolManager(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger(nameof(OpcUaToolManager));
            _oPCUAHelper = new OPCUAHelper(loggerFactory);
        }

        public bool Connect(string serverUrl)
        {
            _oPCUAHelper.OpenConnectOfAnonymous(serverUrl);
            return _oPCUAHelper.ConnectStatus;
        }

        /// <summary>
        /// 指定节点数据
        /// </summary>
        /// <returns></returns>
        public async Task<T> ReadNodeAsync<T>(string nodeId)
        {
            var value = await _oPCUAHelper.GetCurrentNodeValueOfAsync<T>(nodeId);
            return value;
        }


        public async Task<T> WriteNodeAsync<T>(string nodeId, T value)
        {
            return default(T);
        }

        public List<TreeNodeViewModel> GetAllRelationNodeOfNodeId(string nodeId)
        {
            var treeNodes = new List<TreeNodeViewModel>();
            var descriptions = _oPCUAHelper.GetAllRelationNodeOfNodeId(nodeId);
            foreach (var description in descriptions)
            {
                var node = new TreeNodeViewModel(description.DisplayName.ToString())
                {
                    NodeId = description.NodeId.ToString(),
                    NodeType = description.TypeId.ToString()
                };
                node.AddChildren(new TreeNodeViewModel("kong"));
                treeNodes.Add(
                  node
                );
            };
            return treeNodes;
        }

        public void GetCurrentNodeAttributes(string nodeId)
        {
            _oPCUAHelper.GetCurrentNodeAttributes(nodeId);
        }
    }
}