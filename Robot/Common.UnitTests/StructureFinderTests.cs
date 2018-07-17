using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Builders;
using Common.Classes.General;
using Common.Finders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class StructureFinderTests
    {
        [TestMethod]
        public void Test1()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 10, Close = 15},
                new Candle {Open = 20, Close = 30},
                new Candle {Open = 30, Close = 40},
            };

            var structure = new StructureInfo(new List<StructureInfoItem>
            {
                new StructureInfoItem {Period = 1, Change = 0.5, Atr = 10},
                new StructureInfoItem {Period = 1, Change = 0.5, Atr = 10}
            });

            ;

            var result = new RecursiveStructureFinder(candles, structure).FindAll();

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(2, result[0].Count);
            Assert.AreEqual(0, result[0][0].StartIndex);
            Assert.AreEqual(1, result[0][1].EndIndex);
        }

        [TestMethod]
        public void Test2()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 15, Close = 20},
                new Candle {Open = 25, Close = 15},

                new Candle {Open = 20, Close = 30},
                new Candle {Open = 30, Close = 15},
                new Candle {Open = 20, Close = 30},
                new Candle {Open = 25, Close = 30},
            };

            var structure = new StructureInfo(new List<StructureInfoItem>
            {
                new StructureInfoItem() {Period = 1, Change = 0.5, Atr = 10},
                new StructureInfoItem() {Period = 1, Change = -0.5, Atr = 10},
                new StructureInfoItem() {Period = 1, Change = 0.5, Atr = 10},
            });
            
            var result = new RecursiveStructureFinder(candles, structure).FindAll();


            Assert.AreEqual(1, result.Count);

            var foundStructure = result[0];

            Assert.AreEqual(3, foundStructure.Count);

            Assert.AreEqual(2, foundStructure[0].StartIndex);
            Assert.AreEqual(2, foundStructure[0].EndIndex);

            Assert.AreEqual(3, foundStructure[1].StartIndex);
            Assert.AreEqual(3, foundStructure[1].EndIndex);

            Assert.AreEqual(4, foundStructure[2].StartIndex);
            Assert.AreEqual(4, foundStructure[2].EndIndex);

        }

        [TestMethod]
        public void Test3()
        {
            var candles = new List<Candle>()
            {
                new Candle {Open = 22, Close = 20},
                
                new Candle {Open = 20, Close = 25},
                new Candle {Open = 25, Close = 20},
                new Candle {Open = 20, Close = 30},

                new Candle {Open = 30, Close = 30},
                new Candle {Open = 30, Close = 30},

                new Candle {Open = 20, Close = 25},
                new Candle {Open = 25, Close = 20},
                new Candle {Open = 20, Close = 20},
                new Candle {Open = 20, Close = 30},

            };

            var structure = new StructureInfo(new List<StructureInfoItem>
            {
                new StructureInfoItem() {Period = 1, Change = 0.25, Atr =10},
                new StructureInfoItem() {Period = 1, Change = -0.2, Atr =10},
                new StructureInfoItem() {Period = 1, Change = 0.5, Atr =10},
            });


            var result = new RecursiveStructureFinder(candles, structure).FindAll();

            Assert.AreEqual(2, result.Count);

            var first = result[0];
            Assert.AreEqual(3, first.Count);

            Assert.AreEqual(1, first[0].StartIndex);
            Assert.AreEqual(3, first[2].EndIndex);

            var second = result[1];
            Assert.AreEqual(3, second.Count);

            Assert.AreEqual(6, second[0].StartIndex);
            Assert.AreEqual(9, second[2].EndIndex);

        }

    }
}
