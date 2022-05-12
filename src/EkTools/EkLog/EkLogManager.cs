using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkTools.EkLog
{
    public class EkLogManager
    {
        private EkLogManager() { }

        private static readonly EkLogManager Current = new EkLogManager();
        public static EkLogManager Instance => Current;

        public void DeleteLogs()
        {
            var bastPath = Directory.GetCurrentDirectory();
            var logsPath = Path.Combine(bastPath, "Logs");
            if (!Directory.Exists(logsPath))
            {
                return;
            }
            var files = Directory.GetFiles(logsPath, "*.log");
            var now = DateTime.UtcNow.AddDays(-5);
            foreach (var filePath in files)
            {
                var file = new FileInfo(filePath);
                if (file.CreationTimeUtc >= now) continue;

                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    EkLog.Warning($"删除日志失败{e.Message}");
                }
            }
        }

        private void Instance_Error(string msg, bool show)
        {
            EkLog.Error(msg, show);
        }

        private void Instance_Debug(string msg, bool show)
        {
            EkLog.Debug(msg, show);
        }

        private void Instance_Warn(string msg, bool show)
        {
            EkLog.Warning(msg, show);
        }
    }
}
