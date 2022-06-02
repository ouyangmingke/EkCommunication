using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Text;
using System.Threading;

namespace EkCommunication.RabbitMQ
{
    public class RabbitMQClient
    {
        private readonly string _clientId;
        public RabbitMQClient(string clientId)
        {
            _clientId = clientId;
        }
        private IModel _receiveChannel;

        public void Receive()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();// 连接到 RabbitMQ 节点
            _receiveChannel = connection.CreateModel();// 创建 确认消息 通道
            _receiveChannel.QueueDeclare(queue: _clientId,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(_receiveChannel);
            // 接收完成事件
            consumer.Received += (model, ea) =>
            {
                var props = ea.BasicProperties;// 消息属性
                var replyProps = _receiveChannel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;// RPC 响应与请求关联ID
                string response = null;
                try
                {
                    var body = ea.Body.ToArray();
                    var msg = Encoding.UTF8.GetString(body);
                    RabbitMqMessage.OnNotify($" [x] Received {msg}  {_clientId}" );
                    response = $"{_clientId}==》{props.CorrelationId}=> {msg}";
                }
                catch (Exception e)
                {
                    RabbitMqMessage.OnNotify(" [.] " + e.Message);
                    response = "";
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    // 发送回复消息
                    _receiveChannel.BasicPublish(
                        exchange: "",
                        routingKey: props.ReplyTo,// 回调队列名称
                        basicProperties: replyProps,
                        body: responseBytes);


                    RabbitMqMessage.OnNotify($"接收并回复完成 {ea.DeliveryTag}");
                    // 忘记配置 BasicAck 将导致消息被重新传递( 可能看起来像随机重新传递 )
                    // 但是 RabbitMQ 将消耗越来越多的内存，因为它无法释放任何未确认的消息。
                    // 两种方式配置确认交付完成，消息将被丢弃
                    //((EventingBasicConsumer)model).Model.BasicAck(
                    //    deliveryTag: ea.DeliveryTag,
                    //    multiple: false);
                    _receiveChannel.BasicAck(
                        deliveryTag: ea.DeliveryTag,// 交付标识  交付标签是单调增长的正整数，并由客户端库呈现。确认交付的客户端库方法将交付标签作为参数。
                        multiple: false// 设置为 true 时,RabbitMQ 将确认该渠道所有未完成的交付标签
                        );

                }
            };
            // 设置交付确认
            var consumerTag = _receiveChannel.BasicConsume(queue: _clientId,// 队列名称
                                  autoAck: false,// 是否自动确认  设置为false 时 需要使用   BasicAck 进行确认
                                  consumer: consumer);
        }
    }
}
