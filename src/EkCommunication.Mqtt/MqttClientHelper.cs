using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Server;

using System.Text;

namespace EkCommunication.Mqtt
{

    public class MqttClientHelper
    {
        private MqttClientHelper()
        {

        }
        private static readonly MqttClientHelper Current = new MqttClientHelper();
        public static MqttClientHelper Instance => Current;


        private IMqttClient mqttClient;
        public async void Conn()
        {
            if (mqttClient == null || !mqttClient.IsConnected)
            { 
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer("192.168.10.201", 1883)
     .WithClientId(Guid.NewGuid().ToString())
     .WithCredentials("admin", "public").Build();
                mqttClient = new MqttFactory().CreateMqttClient();
                await mqttClient.ConnectAsync(options, CancellationToken.None);
            }
        }
        /// <summary>
        /// 订阅接受消息
        /// </summary>
        /// <returns></returns>
        public async Task ReceiveMessageAsync(string topic = "topic")
        {
            try
            {
                Conn();
                await mqttClient.SubscribeAsync(topic);
                mqttClient.UseApplicationMessageReceivedHandler(msg =>
                {
                    string message = Encoding.UTF8.GetString(msg.ApplicationMessage.Payload);
                    Console.WriteLine(message);
                });

            }
            catch (Exception ex)
            {


            }
        }



        public async Task SendMessageAsync(string topic = "topic", string message = "messge")
        {
            try
            {
                Conn();
                var msg = new MqttApplicationMessageBuilder()
               .WithTopic(topic)
               .WithPayload(message)
               .WithExactlyOnceQoS()
               .WithRetainFlag().Build();
                await mqttClient.PublishAsync(msg);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
