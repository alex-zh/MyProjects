using System;
using FinancialIdeasTester;
using FinancialIdeasTester.Classes;
using FinancialIdeasTester.Rules.BrentToUsdRubRatio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class BrentToUsdRubRatioRuleTests
    {
        [TestMethod]
        public void BrentAndUsdRubHaveTheSameValues_ConstantRatio_ZeroChange()
        {
            var brentData = new DataHolder();
            var usdrubData = new DataHolder();
            var random = new Random();

            for (int i = 0; i < 33; i++)
            {
                var date = DateTime.Now.AddDays(i);
                double value = random.Next(90, 110);

                brentData.Candles.Add(new Candle() {Close = value, Date = date});
                usdrubData.Candles.Add(new Candle() {Close = value/3, Date = date});
            }

            var rule = new BrentToUsdRubRatioRule(brentData, usdrubData);
            var result = rule.Calculate();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0.0, result[0].RatioChange);

            var usdRubChange = (usdrubData.Candles[32].Close - usdrubData.Candles[31].Close)/
                               usdrubData.Candles[31].Close;

            Assert.AreEqual(usdRubChange, result[0].UsdRubChange);
        }

        [TestMethod]
        public void BrentAndUsdRubHaveTheSameValues()
        {
            var brentData = new DataHolder();
            var usdrubData = new DataHolder();
            var random = new Random();

            for (int i = 0; i < 14; i++)
            {
                var date = DateTime.Now.AddDays(i);

                double brentValue = 1;
                double usdRubValue = 1;

                if (i > 9)
                {
                    brentValue = 2;
                }

                brentData.Candles.Add(new Candle() { Close = brentValue, Date = date });
                usdrubData.Candles.Add(new Candle() { Close = usdRubValue, Date = date });
            }

            var rule = new BrentToUsdRubRatioRule(brentData, usdrubData, 10, 3);
            var result = rule.Calculate();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].RatioChange);            
        }
    }
}