namespace BitMexConnector
{
    public interface ILogger
    {        
        void Info(string text);
        void Error(string text);
    }
}