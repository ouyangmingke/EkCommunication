using RabbitMQ.Client;

using System;
using System.Text;
using System.Threading.Channels;

namespace EkCommunication.RabbitMQ
{
    public class RabbitMQServer
    {
        private readonly string _ServerName;
        /// <summary>
        /// 发送队列
        /// </summary>
        private readonly string _queueName;
        /// <summary>
        /// 回复队列
        /// </summary>
        private readonly string _replyQueueName;
        /// <summary>
        /// 目标客户端
        /// </summary>
        private readonly string _correlationId;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="queueName">发布队列名称</param>
        /// <param name="replyQueueName">回复队列名称</param>
        /// <param name="correlationId">关联 ID</param>
        public RabbitMQServer(string serverName, string queueName, string replyQueueName, string correlationId)
        {
            _ServerName = serverName;
            _queueName = queueName;
            _replyQueueName = replyQueueName;
            _correlationId = correlationId;
            RabbitMQProvider();
        }

        /// <summary>
        /// 信道  出现异常后信道将不可用
        /// </summary>
        private IModel _rabbitMQChannel;

        private void RabbitMQProvider()
        {
            var factory = new ConnectionFactory() { HostName = "192.168.10.201", UserName = "admin", Password = "public" };
            var connection = factory.CreateConnection();// 连接到 RabbitMQ 节点
            _rabbitMQChannel = connection.CreateModel();// 创建信道
        }

        /// <summary>
        /// 队列声明
        /// 创建队列并将队列绑定到指定交换器
        /// </summary>
        public void QueueDeclare()
        {
            // 声明发布队列
            // 声明后不允许 使用不同参数重新定义现有队列
            // 并且会向任何尝试这样做的程序返回错误
            var queueDeclareOk = _rabbitMQChannel.QueueDeclare(
                queue: _queueName,// 队列名称
                durable: false,// 标记是否将队列持久化 
                exclusive: false,
                autoDelete: false,
                arguments: null);
            //try
            //{
            //    _rabbitMQChannel.ExchangeDeclarePassive("logs");
            //}
            //catch (Exception e)
            //{
            //    //出现异常后信道将不可用
            //}
            // 创建一个队列交换器
            _rabbitMQChannel.ExchangeDeclare(
            exchange: "logs", // 交换器名称
            type: ExchangeType.Topic);// 交换器类型

            // 将队列绑定到交换器
            _rabbitMQChannel.QueueBind(
                queue: _queueName,// 队列
                exchange: "logs",// 交换器
                routingKey: "routing.#"); // 路由键
            // 把 MyExchangeName 交换机中的消息 根据路由键 转发到 logs 交换机 
            // 注意目标交换机类型 
            _rabbitMQChannel.ExchangeBind(
                "logs",
                "MyExchangeName",
                "MyApp.Product.StockChange"
                );
            // 声明回调队列
            _rabbitMQChannel.QueueDeclare(
                queue: _replyQueueName,
                durable: false,// 标记是否将队列持久化 
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var queueName = _rabbitMQChannel.QueueDeclare();

        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void Send(string msg)
        {
            // 设置在工作者 处理完前不要发送新数据
            // 注意事项
            // 如果所有工作人员都很忙，队列可能会被填满。
            // 可增加更多的工人，或者有一些其他的策略。
            //_rabbitMQChannel.BasicQos(
            //    prefetchSize: 0,// 预读取大小
            //    prefetchCount: 1, // 预读取数量
            //    global: false);// 全局化

            // 创建消息属性
            var properties = _rabbitMQChannel.CreateBasicProperties();
            properties.Persistent = false;// 标记是否将消息持久化

            // 回复队列
            properties.ReplyTo = _replyQueueName;
            // RPC 响应与请求关联ID
            // 也可使用 Guid.NewGuid().ToString();
            properties.CorrelationId = _correlationId;

            var message = "Hello World!" + msg;
            var body = Encoding.UTF8.GetBytes(message);

            var routingKey = $"routing.{_ServerName}.T1";
            // 发送消息

            _rabbitMQChannel.BasicPublish(
            exchange: "logs",// 交换器，消息将发至指定交换器
            routingKey: routingKey,//路由键 
            basicProperties: properties,// 消息属性
            body: body);
            RabbitMqMessage.OnNotify($"{_ServerName}_{_queueName} routingKey： {routingKey} Mes:{message}");
            // 如果没有服务器没有收到消息可能是代理没有足够的空间(默认情况下最少需要50M)
        }
    }
}
