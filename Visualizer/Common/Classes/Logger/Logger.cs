using System;
using Common.Classes.Robot;
using Common.Robots.Common;

namespace Common.Classes.Logger
{
    public class Logger : ILogger
    {
        private readonly string _robotId;
        private readonly ILogsRepository _repository;
        private readonly TextFileLogger _textFileLogger = new TextFileLogger();

        public Logger(ILogsRepository repository)
        {
            _repository = repository;
        }

        public Logger(string  robotId, ILogsRepository repository)
            : this(repository)
        {
            _robotId = robotId;
            _repository = repository;
        }

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

            if (messageType == LogMessageTypes.Error || messageType == LogMessageTypes.Critical)
            {
                _repository.Add(new LogItem { DateTime = DateTime.Now, MessageType = messageType, Text = text }, _robotId);
            }
        }

        public void LogFormat(LogMessageTypes messageType, string text, params object[] values)
        {
            Log(String.Format(text, values), messageType);
        }


    }
}
