using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EkCommunication.RabbitMQ
{
    public class RabbitMQClient
    {
        private readonly string _clientName;

        /// <summary>
        /// 订阅队列
        /// </summary>
        private readonly string _subscriptionQueueName;
        /// <summary>
        /// 相关性ID
        /// </summary>
        private readonly List<string> _correlationIds;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientName">客户端名称</param>
        /// <param name="subscriptionQueueName">订阅队列名称</param>
        /// <param name="correlationId">已读标记列表</param>
        public RabbitMQClient(string clientName, string subscriptionQueueName, List<string> correlationId)
        {
            _clientName = clientName;
            _subscriptionQueueName = subscriptionQueueName;
            _correlationIds = correlationId;
        }
        private IModel _rabbitMQProvider;

        /// <summary>
        /// 接收消息
        /// </summary>
        public void Receive()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();// 连接到 RabbitMQ 节点
            _rabbitMQProvider = connection.CreateModel();// 创建 确认消息 通道

            // 创建/配置消费者
            var consumer = new EventingBasicConsumer(_rabbitMQProvider);


            // 接收完成事件
            consumer.Received += (model, ea) =>
            {
                // 消息属性
                var props = ea.BasicProperties;

                // 判断该消息是否重复使用(事务性判断)
                // 或用作数据筛选(路由即可)
                if (_correlationIds.Any(t => t == props.CorrelationId))
                {
                    _rabbitMQProvider.BasicReject(
                  deliveryTag: ea.DeliveryTag,// 交付标识 
                  true);// 是否重新排队
                    return;
                }

                // 回复消息属性
                var replyProps = _rabbitMQProvider.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;// RPC 响应与请求关联ID
                string response = $"回复消息:{_clientName}";

                var body = ea.Body.ToArray();
                var msg = Encoding.UTF8.GetString(body);
                RabbitMqMessage.OnNotify($"{_clientName} _ {_subscriptionQueueName} 读取到 {msg}");

                var responseBytes = Encoding.UTF8.GetBytes($"{_clientName}发送回复消息");
                if (props.ReplyTo != null)
                {
                    // 发送回复消息
                    _rabbitMQProvider.BasicPublish(
                        exchange: "",
                        routingKey: props.ReplyTo,
                        basicProperties: replyProps,
                        body: responseBytes);
                    RabbitMqMessage.OnNotify($"{_clientName} _ 回复消息 {props.ReplyTo} {response}");
                }

                // 忘记配置 BasicAck 将导致消息被重新传递( 可能看起来像随机重新传递 )
                // 但是 RabbitMQ 将消耗越来越多的内存，因为它无法释放任何未确认的消息。
                // 两种方式配置确认交付完成，消息将被丢弃
                //((EventingBasicConsumer)model).Model.BasicAck(
                //    deliveryTag: ea.DeliveryTag,
                //    multiple: false);
                _rabbitMQProvider.BasicAck(
                deliveryTag: ea.DeliveryTag,// 交付标识  交付标签是单调增长的正整数，并由客户端库呈现。确认交付的客户端库方法将交付标签作为参数。
                multiple: false// 设置为 true 时,RabbitMQ 将确认该渠道所有未完成的交付标签
                );
            };

            // 配置消费者
            var consumerTag = _rabbitMQProvider.BasicConsume(
               queue: _subscriptionQueueName,// 队列名称
               autoAck: false,// 是否自动确认  设置为false 时 需要使用   BasicAck 进行确认
               consumer: consumer);
        }
    }
}
