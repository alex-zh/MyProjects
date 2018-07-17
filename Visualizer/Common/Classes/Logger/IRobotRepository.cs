using System.Collections.Generic;

namespace Common.Classes.Logger
{
    public interface ILogsRepository
    {
        IEnumerable<LogItem> GetAll();

        IEnumerable<LogItem> GetAll(string robotId);

        void Add(LogItem logItem);

        void Add(LogItem item, string robotId);

        void ClearAll();
    }
}