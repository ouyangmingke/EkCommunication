namespace EkCommunication.RabbitMQ
{
    /// <summary>
    /// 日志通知
    /// </summary>
    /// <param name="msg"></param>
    public delegate void RabbitMqNotify(string msg);
    public static class RabbitMqMessage
    {
        public static event RabbitMqNotify RabbitMqNotify;
        public static void OnNotify(string msg)
        {
            RabbitMqNotify?.Invoke(msg);
        }
    }
}
