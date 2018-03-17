using sfShareLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutineTask
{
    public class Action
    {
        protected sfLog _sfAppLogger;
        protected string _TaskName;
        private string sfLogLevel = Program._sfLogLevel;
        private string sfLogStorageName = Program._sfLogStorageName;
        private string sfLogStorageKey = Program._sfLogStorageKey;
        private string sfLogStorageContainerApp = Program._sfLogStorageContainerApp;

        public Action(string taskName)
        {
            _TaskName = taskName;
            sfLogLevel logLevel = (sfLogLevel)Enum.Parse(typeof(sfLogLevel), sfLogLevel);
            _sfAppLogger = new sfLog(sfLogStorageName, sfLogStorageKey, sfLogStorageContainerApp, logLevel);
        }
        public virtual void Exec()
        {
            /* implement action content */
            /* record action start timestamp to log */
            RecordStartLog();

            /* log sample */
            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendLine("Routine TaskActor : " + _TaskName);
            logMessage.AppendLine("Error Message : unimplement this action.");
            _sfAppLogger.Error(logMessage);

            /* record action end timestamp to log */
            RecordEndLog();
        }
        protected void RecordStartLog()
        {
            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendLine("Start Routine TaskActor: " + _TaskName);
            _sfAppLogger.Info(logMessage);
        }
        protected void RecordEndLog()
        {
            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendLine("End Routine Actor: " + _TaskName);
            _sfAppLogger.Info(logMessage);
        }
    }
}
