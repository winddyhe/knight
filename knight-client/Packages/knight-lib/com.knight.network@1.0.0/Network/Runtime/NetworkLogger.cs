using System;
using TouchSocket.Core;
using Knight.Core;

namespace Knight.Framework.Network
{
    public class NetworkLogger : ILog
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;

        public void Log(LogLevel rLogLevel, object rSource, string rMessage, Exception rException)
        {
            if (rLogLevel == LogLevel.Debug)
            {
                LogManager.Log(rLogLevel);
            }
            else if (rLogLevel == LogLevel.Info)
            {
                LogManager.LogRelease(rLogLevel);
            }
            else if (rLogLevel == LogLevel.Warning)
            {
                LogManager.LogWarning(rLogLevel);
            }
            else if (rLogLevel == LogLevel.Error || rLogLevel == LogLevel.Critical)
            {
                LogManager.LogError(rLogLevel);
            }
            else
            {
                LogManager.Log(rLogLevel);
            }
        }
    }
}
