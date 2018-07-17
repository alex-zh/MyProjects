using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Strategies.UnitTests
{
    [TestClass]
    public class DeviationCorrectionTests
    {
        //[TestMethod]
        //public void Test1()
        //{
        //    var candles = new[]{ new Candle{ Open = 1, Close= 2}, 
        //                         new Candle{ Open = 3, Close= 6},
        //                         new Candle{ Open = 1, Close= 3}};

        //    var volatilityCalculator = new VolatilityCalculatorStub(candles);

        //    var deviationCorrector = new DeviationCorrector(candles, volatilityCalculator);
        //    deviationCorrector.InitVolatilityGroups();

        //    Assert.AreEqual(3, deviationCorrector._volatilityGroups.Count);

        //    Assert.AreEqual(0, deviationCorrector._volatilityGroups[0].MinValue);
        //    Assert.AreEqual(1, deviationCorrector._volatilityGroups[0].MaxValue);

        //    Assert.AreEqual(1, deviationCorrector._volatilityGroups[1].MinValue);
        //    Assert.AreEqual(2, deviationCorrector._volatilityGroups[1].MaxValue);

        //    Assert.AreEqual(2, deviationCorrector._volatilityGroups[2].MinValue);
        //    Assert.AreEqual(double.MaxValue, deviationCorrector._volatilityGroups[2].MaxValue);
        //}

        //[TestMethod]
        //public void Test2()
        //{
        //    var candles = new List<Candle>();

        //    for (int i = 0; i < 100; i++)
        //    {
        //        var open = 100;
        //        var close = 110 + 2 * (i % 3);
                
        //        candles.Add(new Candle(){Open =  open, Close = close});
        //    }

        //    var volatilityCalculator = new VolatilityCalculatorStub(candles);

        //    var deviationCorrector = new DeviationCorrector(candles, volatilityCalculator);
        //    deviationCorrector.InitVolatilityGroups();

        //    Assert.AreEqual(3, deviationCorrector._volatilityGroups.Count);

        //    Assert.AreEqual(0, deviationCorrector._volatilityGroups[0].MinValue);
        //    Assert.AreEqual(10, deviationCorrector._volatilityGroups[0].MaxValue);

        //    Assert.AreEqual(10, deviationCorrector._volatilityGroups[1].MinValue);
        //    Assert.AreEqual(12, deviationCorrector._volatilityGroups[1].MaxValue);

        //    Assert.AreEqual(12, deviationCorrector._volatilityGroups[2].MinValue);
        //    Assert.AreEqual(double.MaxValue, deviationCorrector._volatilityGroups[2].MaxValue);

        //}
     
    }
}
