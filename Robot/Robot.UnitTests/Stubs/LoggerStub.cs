using Common.Classes.Logger;

namespace Robot.UnitTests.Stubs
{
    internal class LoggerStub : ILogger
    {
        public void Info(string text)
        {
            throw new System.NotImplementedException();
        }

        public void Error(string text)
        {
            throw new System.NotImplementedException();
        }

        public void Log(string text, LogMessageTypes messageType)
        {        
        }

        public void LogFormat(LogMessageTypes messageType, string text, params object[] values)
        {
            
        }

        public void Log(string robotName, string text, LogMessageTypes messageType)
        {            
        }

        public void Warning(string text)
        {
            
        }
    }
}