using System;

namespace Common.Classes.Logger
{
    public class LoggerToFile : ILogger
    {        
        private readonly TextFileLogger _textFileLogger = new TextFileLogger();

        public void Info(string text)
        {
            Log(text, LogMessageTypes.Info);
        }

        public void Warning(string text)
        {
            Log(text, LogMessageTypes.Warning);
        }

        public void Error(string text)
        {
            Log(text, LogMessageTypes.Error);
        }

        public void Log(string text, LogMessageTypes messageType)
        {
            _textFileLogger.Log(text, messageType);        
        }

        public void LogFormat(LogMessageTypes messageType, string text, params object[] values)
        {
            Log(String.Format(text, values), messageType);
        }

        
    }
}