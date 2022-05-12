namespace EkTools.EkLog
{
    /// <summary>
    /// 日志通知
    /// </summary>
    /// <param name="msg"></param>
    public delegate void EkLogNotify(string msg);
    public static class EkLogEventBus
    {
        public static event EkLogNotify EkLogNotify;
        public static void OnNotify(string msg)
        {
            EkLogNotify?.Invoke(msg);
        }
    }
}
