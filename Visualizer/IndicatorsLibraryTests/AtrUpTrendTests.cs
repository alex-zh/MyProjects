using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsLibraryTests
{
    [TestClass]
    public class AtrUpTrendTests
    {
        [TestMethod]
        [DeploymentItem("DataForTests\\TrendTests.xlsx")]
        public void Test1()
        {
            string filePath = @"DataForTests\\TrendTests.xlsx";

            //var candles = new ExcelLoader(filePath).Load("Test1");

            //Assert.AreEqual(3, result);            
        }     
    }
}
