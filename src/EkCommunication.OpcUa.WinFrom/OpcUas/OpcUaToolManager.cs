using EkCommunication.OpcUa.OpcUaTool;

using Microsoft.Extensions.Logging;

using Opc.Ua;

namespace EkCommunication.OpcUa.WinFrom.OpcUas
{
    public class OpcUaToolManager
    {
        protected ILoggerFactory LoggerFactory { get; set; }
        protected ILogger Logger => LoggerFactory.CreateLogger(nameof(OpcUaToolManager));

        private readonly OPCUAHelper oPCUAHelper;
        public OpcUaToolManager(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            ClientUtils.Message += (msg) =>
            {
                Logger.LogWarning(msg);
            };
            oPCUAHelper = new OPCUAHelper();
            for (int i = 1; i < 11; i++)
            {
                BiesseModels.Add(new BiesseModel($"PROG_{i}"));
            }
        }

        /// <summary>
        /// 读取值
        /// </summary>
        private readonly List<BiesseModel> BiesseModels = new List<BiesseModel>();
        public bool Connect(string serverUrl)
        {
            oPCUAHelper.OpenConnectOfAnonymous(serverUrl);
            Console.WriteLine("连接状态" + oPCUAHelper.ConnectStatus);
            return oPCUAHelper.ConnectStatus;
        }

        private static List<NodeId> Path(string group)
        {
            return new List<NodeId> {
            $"ns=2;s=MES.MES_MACHINE.PROGRAMS.{group}.ID_PROGRAM",// 程序
            $"ns=2;s=MES.MES_MACHINE.PROGRAMS.{group}.PROGRAM"// 板件码
            };
        }

        /// <summary>
        /// 获取当前加工板件
        /// </summary>
        /// <returns></returns>
        public async Task<string> CurrentPartAsync()
        {
            foreach (var item in BiesseModels)
            {
                item.ProGramId = 0;
                item.ProGram = "";
                var nodes = Path(item.BiesseModelName);
                var values = await oPCUAHelper.GetBatchNodeDatasAsync(nodes);
                try
                {
                    if (values != null)
                    {
                        item.ProGramId = (int)values[0].Value;
                        item.ProGram = (string)values[1].Value;
                    }
                }
                catch (Exception ex)
                {
                    var msg = $"数据转换失败检查NodeId是否正确{item.BiesseModelName}\n{ex.Message}";
                    Console.WriteLine(msg);
                    Logger?.LogError(msg);
                }
            }
            var max = BiesseModels.MaxBy(t => t.ProGramId);
            return max == null ? "1232" : max.ProGram;
        }
    }
}