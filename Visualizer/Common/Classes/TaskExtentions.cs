using System.Threading.Tasks;
using Common.Classes.Logger;

namespace Common.Classes
{
    public static class TaskExtentions
    {
        public static void LogExceptions(this Task task, ILogger logger)
        {
            task.ContinueWith(t =>
            {
                var aggException = t.Exception.Flatten();
                foreach (var exception in aggException.InnerExceptions)
                    logger.Log(exception.ToString(), LogMessageTypes.Error);
            },
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}