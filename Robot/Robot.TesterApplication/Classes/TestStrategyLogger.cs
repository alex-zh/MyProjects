using System;
using System.Windows;
using Common.Classes;
using Common.Classes.Logger;

namespace Robot.TesterApplication.Classes
{
    internal class TestStrategyLogger : ILogger
    {
        private readonly TextFileLogger _textFileLogger = new TextFileLogger("C:\\CryptoData\\Logs");

        public void Info(string text)
        {
            Log(text,LogMessageTypes.Info);
        }

        public void Error(string text)
        {
            Log(text, LogMessageTypes.Error);
        }

        public void Log(string text, LogMessageTypes messageType)
        {
            if (messageType == LogMessageTypes.Info)
            {
                _textFileLogger.Log(text, LogMessageTypes.Info);
            }
            else if (messageType == LogMessageTypes.Error || messageType == LogMessageTypes.Critical)
                MessageBox.Show(text);
        }

        public void LogFormat(LogMessageTypes messageType, string text, params object[] values)
        {
            Log(String.Format(text, values), messageType);
        }

        public void Log(string robotName, string text, LogMessageTypes messageType)
        {
            if (messageType == LogMessageTypes.Info)
            {
               _textFileLogger.Log(text, LogMessageTypes.Info);
            }
            else if (messageType == LogMessageTypes.Error || messageType == LogMessageTypes.Critical)
            {
                MessageBox.Show(text);
            }
        }
       
    }
}