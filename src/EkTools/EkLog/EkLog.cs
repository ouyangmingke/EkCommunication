namespace EkTools.EkLog
{
    public static class EkLog
    {
        public static void Information(string msg, bool isShow = true)
        {
            if (isShow)
                EkLogEventBus.OnNotify(msg);
        }
        public static void Debug(string msg, bool isShow = false)
        {
            if (isShow)
                EkLogEventBus.OnNotify(msg);
        }
        public static void Warning(string msg, bool isShow = false)
        {
            if (isShow)
                EkLogEventBus.OnNotify(msg);
        }
        public static void Error(string msg, bool isShow = true)
        {
            if (isShow)
                EkLogEventBus.OnNotify(msg);
        }
    }
}
