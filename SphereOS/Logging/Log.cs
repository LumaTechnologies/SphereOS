using System;
using System.Collections.Generic;

namespace SphereOS.Logging
{
    internal static class Log
    {
        internal static List<LogEvent> Logs = new List<LogEvent>();

        internal static void Info(string source, string message)
        {
            LogEvent logEvent = new LogEvent(LogPriority.Info, source, message);
            Logs.Add(logEvent);
        }

        internal static void Warning(string source, string message)
        {
            LogEvent logEvent = new LogEvent(LogPriority.Warning, source, message);
            Logs.Add(logEvent);
        }

        internal static void Error(string source, string message)
        {
            LogEvent logEvent = new LogEvent(LogPriority.Error, source, message);
            Logs.Add(logEvent);
        }
    }
}
