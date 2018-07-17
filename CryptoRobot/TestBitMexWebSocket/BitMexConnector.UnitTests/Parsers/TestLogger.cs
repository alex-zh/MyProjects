using Common.Classes.Logger;

namespace BitMexConnector.UnitTests.Parsers
{
    public class TestLogger : ILogger
    {
        public void Info(string operation)
        {
            
        }

        public void Error(string websocketClosedUnexpectedly)
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