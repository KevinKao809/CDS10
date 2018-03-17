using sfShareLib;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace IoTHubEventProcessor.Utilities
{
    public class ConsoleLog
    {
        private static bool _log_level_info = false;

        //private static Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        static sfLogLevel logLevel = (sfLogLevel)Enum.Parse(typeof(sfLogLevel), ConfigurationManager.AppSettings["sfLogLevel"]);
        public static sfLog _sfAppLogger = new sfLog(ConfigurationManager.AppSettings["sfLogStorageName"], ConfigurationManager.AppSettings["sfLogStorageKey"], ConfigurationManager.AppSettings["sfLogStorageContainerApp"], logLevel);

        public static void WriteDocDBLogToConsole(string format, params object[] args)
        {
            writeConsoleLog(ConsoleColor.Green, format, args);            
        }

        public static void WriteMessageAlarmLogToConsole(string format, params object[] args)
        {
            writeConsoleLog(ConsoleColor.Yellow, format, args);
        }

        public static void WriteMessageAlarmErrorLogToConsole(string format, params object[] args)
        {
            writeConsoleLog(ConsoleColor.Red, format, args);
        }

        public static void WriteToConsole(string format, params object[] args)
        {
            writeConsoleLog(ConsoleColor.White, format, args);
        }

        [Conditional("DEBUG")]
        public static void WriteMessageAlarmLogToConsoleInfo(string format, params object[] args)
        {
            if(_log_level_info)
                writeConsoleLog(ConsoleColor.Yellow, format, args);
        }

        public static void WriteBlobLogInfo(string format, params object[] args)
        {
            _sfAppLogger.Info(build(format, args));
        }

        public static void WriteBlobLogDebug(string format, params object[] args)
        {
            _sfAppLogger.Debug(build(format, args));
        }

        public static void WriteBlobLogWarn(string format, params object[] args)
        {
            _sfAppLogger.Warn(build(format, args));
        }

        public static void WriteBlobLogError(string format, params object[] args)
        {
            _sfAppLogger.Error(build(format, args));
        }

        private static StringBuilder build(string format, params object[] args)
        {
            StringBuilder logMessage = new StringBuilder();
            logMessage.Append("IoTHubEventProcessor (" + Program._IoTHubAlias + ") ");
            logMessage.AppendFormat(format, args);

            return logMessage;
        }

        [Conditional("DEBUG")]
        private static void writeConsoleLog(ConsoleColor color, string format, params object[] args)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(format, args);
            Console.ResetColor();
        }
    }
}
