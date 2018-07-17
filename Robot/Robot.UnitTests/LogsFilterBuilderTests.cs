using System.Collections.Generic;
using System.Linq;
using Common.Classes.Logger;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Robot.UnitTests
{
    [TestClass]
    public class LogsFilterBuilderTests
    {       
        [TestMethod]
        public void LogsFilterBuilderTests_Test1()
        {
            var logs = new List<LogItem>()
            {
                new LogItem {Text = "1", MessageType = LogMessageTypes.Error},
                new LogItem {Text = "2", MessageType = LogMessageTypes.Error},
                new LogItem {Text = "3", MessageType = LogMessageTypes.Warning}
            };

            var filter = new LogsFilterExpressionBuilder().Build();

            var filteredLogs = logs.Where(filter).ToList();

            Assert.AreEqual(2, filteredLogs.Count());
            Assert.AreEqual("1", filteredLogs[0].Text);
            Assert.AreEqual("2", filteredLogs[1].Text);

            filter = new LogsFilterExpressionBuilder().Build(true, true);

            filteredLogs = logs.Where(filter).ToList();

            Assert.AreEqual(3, filteredLogs.Count());
        }

        [TestMethod]
        public void LogsFilterBuilderTests_Test2()
        {
            var logs = new List<LogItem>
            {
                new LogItem {Text= "1", MessageType = LogMessageTypes.Error},
                new LogItem {Text= "2", MessageType = LogMessageTypes.Error},
                new LogItem {Text= "3", MessageType = LogMessageTypes.Warning},
                new LogItem {Text= "4", MessageType = LogMessageTypes.Warning},
                new LogItem {Text= "5", MessageType = LogMessageTypes.Info}
            };

            var filter = new LogsFilterExpressionBuilder().Build(false, true);

            var filteredLogs = logs.Where(filter).ToList();

            Assert.AreEqual(2, filteredLogs.Count());
            Assert.AreEqual("3", filteredLogs[0].Text);
            Assert.AreEqual("4", filteredLogs[1].Text);

            filter = new LogsFilterExpressionBuilder().Build(false, true, true);

            filteredLogs = logs.Where(filter).ToList();

            Assert.AreEqual("3", filteredLogs.Count());
            Assert.AreEqual("3", filteredLogs[0].Text);
            Assert.AreEqual("4", filteredLogs[1].Text);
            Assert.AreEqual("5", filteredLogs[2].Text);
        }
    }
}
