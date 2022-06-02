using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Collections.Generic;
using System.Text;

namespace EkCommunication.RabbitMQ
{
    public class RabbitMQServer
    {
        private readonly string _ServerId;
        private readonly string _correlationId;

        public RabbitMQServer(string serverId)
        {
            _ServerId = serverId;
            _correlationId = nameof(RabbitMQServer);
        }

        private IModel _sendChannel;
        private IModel _replyChannel;

        public void Init()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();// 连接到 RabbitMQ 节点
            // 创建 发送消息 通道
            _sendChannel = connection.CreateModel();
            // 声明一个队列 声明后不允许 使用不同参数重新定义现有队列
            // 并且会向任何尝试这样做的程序返回错误
            var queueDeclareOk = _sendChannel.QueueDeclare(queue: _ServerId,// 队列名称
                                  durable: false,// 标记是否将队列持久化 
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            // 创建一个队列交换器
            _sendChannel.ExchangeDeclare(
                exchange: "logs", // 交换器名称
                type: ExchangeType.Topic);
            // 将队列绑定到交换器
            _sendChannel.QueueBind(queue: _ServerId,// 队列
                         exchange: "logs",// 交换器
                         routingKey: "#"); // 绑定键

            _replyChannel = connection.CreateModel();// 创建 确认消息 通道

            _replyChannel.QueueDeclare(queue: "replyQueueName",// 队列名称
           durable: false,// 标记是否将队列持久化 
           exclusive: false,
           autoDelete: false,
           arguments: null);

        }

        public void Send(string msg)
        {


            // 设置在工作者 处理完前不要发送新数据
            // 注意事项
            // 如果所有工作人员都很忙，队列可能会被填满。
            // 可增加更多的工人，或者有一些其他的策略。
            //channel.BasicQos(
            //    prefetchSize: 0,// 预读取大小
            //    prefetchCount: 1,// 预读取数量
            //    global: false);// 全局化

            // 创建消息属性
            var properties = _sendChannel.CreateBasicProperties();
            properties.Persistent = false;// 标记是否将消息持久化

            #region 接收回复消息
            // 回复路由键
            properties.ReplyTo = "replyQueueName";
            // RPC 响应与请求关联ID  也可使用 Guid.NewGuid().ToString();
            properties.CorrelationId = _correlationId;
            #endregion

            string message = "Hello World!" + msg;
            var body = Encoding.UTF8.GetBytes(message);
            _sendChannel.BasicPublish(exchange: "logs",// 交换器
                                 routingKey: _ServerId,//路由键 
                                 basicProperties: properties,
                                 body: body);
            RabbitMqMessage.OnNotify($" [x] Sent {msg}  {_ServerId}");
            // 如果没有服务器没有收到消息可能是代理没有足够的空间(默认情况下最少需要50M)
        }

        public void ReplyQueue()
        {
            var consumer = new EventingBasicConsumer(_replyChannel);
            consumer.Received += (model, ea) =>
            {
                RabbitMqMessage.OnNotify($"获取到确认消息{ea.DeliveryTag}");
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == _correlationId)
                {
                    RabbitMqMessage.OnNotify($"确认回复消息=》{response}  {ea.DeliveryTag}");
                    // 只有相关联的消息 才给予回复
                    // 配置交完成
                    _replyChannel.BasicAck(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);
                }

            };
            // 设置交付确认
            _replyChannel.BasicConsume(
                consumer: consumer,
                queue: "replyQueueName",
                autoAck: false);// 回复消息不使用自动确认
        }
    }
}
