using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Tests
{
    [TestClass]
    public class SeriesCrossingFinderTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var a1 = new[] { 2, 3, 4 };
            var a2 = new[] { 1, 2, 3, 4, 5 };

            var s1 = a1.Select(x => new DatePrice { Value = x });
            var s2 = a2.Select(x => new DatePrice { Value = x });

           // var result = new MovingCrossingFinder().Find(s1, s2).ToList();

            //Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void TestMethod2()
        {
            var a1 = new[] { 2, 3, 4, 5, 6, 6, 6 };
            var a2 = new[] { 1, 2, 3, 7, 7, 5 };

            var s1 = a1.Select(x => new DatePrice { Value= x });
            var s2 = a2.Select(x => new DatePrice { Value = x });

           // var result = new MovingCrossingFinder().Find(s1, s2).ToList();

            //Assert.AreEqual(2, result.Count());

            //Assert.AreEqual(5, result[0].FirstPrice);
            //Assert.AreEqual(7, result[0].SecondPrice);
            //Assert.AreEqual(6, result[1].FirstPrice);
            //Assert.AreEqual(5, result[1].SecondPrice);
        }
    }
}
