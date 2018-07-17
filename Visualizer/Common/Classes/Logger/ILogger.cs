namespace Common.Classes.Logger
{
    public interface ILogger
    {
        void Info(string text);
        void Warning(string text);
        void Error(string text);

        void Log(string text, LogMessageTypes messageType);
        void LogFormat(LogMessageTypes messageType, string text, params object[] values);
    }
}