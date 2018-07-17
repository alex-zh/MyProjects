using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class TwoDimensionsDictionaryTests
    {
        [TestMethod]
        public void Test1()
        {
            var dictionary = new TwoDimensionsDictionary<int, int, string>(() => "NA");
            dictionary[1, 2] = "1_2";
            dictionary[3, 4] = "3_4";

            Assert.AreEqual("1_2", dictionary[1, 2]);
            Assert.AreEqual("3_4", dictionary[3, 4]);
            Assert.AreEqual("NA", dictionary[2, 1]);
        }

        [TestMethod]
        public void Test2()
        {
            var dictionary = new TwoDimensionsDictionary<string, string, int>(() => 0);
            dictionary["1", "2"] = 12;
            dictionary["3", "4"] = 34;

            Assert.AreEqual(12, dictionary["1", "2"]);
            Assert.AreEqual(34, dictionary["3", "4"]);
            Assert.AreEqual(0, dictionary["1", "1"]);
        }

        [TestMethod]
        public void Test3()
        {
            var dictionary = new TwoDimensionsDictionary<int, int, string>(() => "NA");
            dictionary[1, 1] = "11";
            dictionary[1, 2] = "12";
            dictionary[1, 3] = "13";
            dictionary[2, 1] = "21";

            Assert.AreEqual(2, dictionary.RowsCount);
            Assert.AreEqual(3, dictionary.ColumnsCount);
        }
    }
}
