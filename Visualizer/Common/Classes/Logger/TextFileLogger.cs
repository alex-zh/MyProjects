using System;
using System.IO;
using Common.Classes.Robot;
using Common.Robots.Common;

namespace Common.Classes.Logger
{
    public class TextFileLogger
    {
        private readonly string _logsFolderPath;
        private const string DefaultLogsPath = "Logs";
        private const string DefaultFileName = "General";

        public TextFileLogger(string logsFolderPath = DefaultLogsPath)
        {
            _logsFolderPath = logsFolderPath;

            if (Directory.Exists(_logsFolderPath) == false)
            {
                Directory.CreateDirectory(_logsFolderPath);
            }
        }

        private static readonly object SyncObject = new object();        

        public void Log(string text, LogMessageTypes messageType)
        {
            Log(null, text, messageType);
        }

        public void Log(RobotNames robotName, string text, LogMessageTypes messageType)
        {
            Log(robotName.ToString(), text, messageType);
        }

        private void Log(string robotName, string text, LogMessageTypes messageType)
        {
            var fileName = GetFileName(robotName);

            lock (SyncObject)
            {
                File.AppendAllText(fileName, GetLogEntry(text, messageType));
            }
        }

        private string GetFileName(string robotName)
        {
            var logFileName = robotName ?? DefaultFileName;

            var dt = DateTime.Now;
            return $"{_logsFolderPath}\\{logFileName}_{dt.Year}_{dt.Month}_{dt.Day}.log";
        }

        private string GetLogEntry(string text, LogMessageTypes messageType)
        {
            if(messageType == LogMessageTypes.Info)
                return $"{text}\r\n";

            return $"{DateTime.Now.ToString("HH:mm:ss")}: {messageType.GetFirstLetter()}: {text}\r\n";
        }
    }
}
