using Common.Classes.Logger;

namespace Robot.DataLayer.UnitTests
{
    internal class LoggerStub : ILogger
    {
        public void Info(string text)
        {
        }

        public void Warning(string text)
        {
        }

        public void Error(string text)
        {
        }

        public void Log(string text, LogMessageTypes messageType)
        {
        }

        public void LogFormat(LogMessageTypes messageType, string text, params object[] values)
        {
        }
    }
}