using System;
using System.IO;

namespace BitMexConnector
{
    public enum LogMessageTypes
    {
        Info,
        Warning,
        Error,
        Critical
    }

    public class TextFileLogger: ILogger
    {
        private readonly string _logsFolderPath;
        private readonly string _fileName;
        private const string DefaultLogsPath = "Logs";
        private const string DefaultFileName = "Log";

        public TextFileLogger(string fileName, string logsFolderPath = DefaultLogsPath)
        {            
            _fileName = fileName;
            _logsFolderPath = logsFolderPath;

            if (Directory.Exists(_logsFolderPath) == false)
            {
                Directory.CreateDirectory(_logsFolderPath);
            }
        }

        private static readonly object SyncObject = new object();

        public void Info(string text)
        {
            Log(text, LogMessageTypes.Info);
        }

        public void Error(string text)
        {
            Log(text, LogMessageTypes.Error);
        }

        private void Log(string text, LogMessageTypes messageType)
        {
            var fileName = GetFileName();

            lock (SyncObject)
            {
                File.AppendAllText(fileName, GetLogEntry(text, messageType));
            }
        }

        private string GetFileName()
        {
            var logFileName = !String.IsNullOrEmpty(_fileName) ? _fileName : DefaultFileName;
            var dt = DateTime.Now;
            return $"{_logsFolderPath}\\{logFileName}_{dt.Year}_{dt.Month}_{dt.Day}.log";
        }

        private string GetLogEntry(string text, LogMessageTypes messageType)
        {            
            return $"{DateTime.Now.ToString("HH:mm:ss")}: {messageType.GetFirstLetter()}: {text}\r\n";
        }
    }

    public static class EnumExtentions
    {
        public static string GetFirstLetter(this LogMessageTypes messageType)
        {
            return messageType.ToString().Substring(0, 1).ToUpper();
        }      

    }
}