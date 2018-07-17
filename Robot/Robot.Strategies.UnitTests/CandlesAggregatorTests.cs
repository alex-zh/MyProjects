using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Strategies.Agents;

namespace Robot.Strategies.UnitTests
{
    [TestClass]
    public class CandlesAggregatorTests
    {
        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod]
        public void AggregateDays_Test0()
        {
            var settings = new AggregationSettings()
            {
                AggregationInteral = AggregationInteral.Day,
                DateTimeType = CandleDateTimeType.StartOfCandle
            };

            var candlesAggregator = new CandlesAggregator(settings);

            var candles = new List<Candle>();

            var result = candlesAggregator.Aggregate(candles);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void AggregateDays_Test1()
        {
            var settings = new AggregationSettings()
            {
                AggregationInteral = AggregationInteral.Day,
                DateTimeType = CandleDateTimeType.StartOfCandle
            };

            var candlesAggregator = new CandlesAggregator(settings);

            var firstDateTime = new DateTime(2017, 12, 5, 12, 20, 0);

            var candles = new[]
            {
                new Candle {Open = 1, Close = 2, Low = 1, High = 2, Date = firstDateTime},
                new Candle() {Open = 1, Close = 2, Low = -1, High = 2, Date = firstDateTime.AddHours(1)},
                new Candle()  {Open = 1, Close = 4, Low = 1, High = 5, Date = firstDateTime.AddHours(2)},
            }.ToList();

            var result = candlesAggregator.Aggregate(candles);

            Assert.AreEqual(1, result.Count);

            var first = result[0];

            Assert.AreEqual(1, first.Open);
            Assert.AreEqual(4, first.Close);
            Assert.AreEqual(-1, first.Low);
            Assert.AreEqual(5, first.High);
            Assert.AreEqual(firstDateTime, first.Date);
        }

        [TestMethod]
        public void AggregateDays_Test2()
        {
            var settings = new AggregationSettings()
            {
                AggregationInteral = AggregationInteral.Day,
                DateTimeType = CandleDateTimeType.StartOfCandle
            };

            var candlesAggregator = new CandlesAggregator(settings);

            var firstDateTime = new DateTime(2017, 12, 5, 12, 20, 0);
            var secondDateTime = new DateTime(2017, 12, 6, 12, 20, 0);

            var candles = new[]
            {
                new Candle {Open = 1, Close = 2, Low = 1, High = 2, Date = firstDateTime},
                new Candle() {Open = 1, Close = 2, Low = -1, High = 2, Date = firstDateTime.AddHours(1)},
                new Candle()  {Open = 1, Close = 4, Low = 1, High = 5, Date = firstDateTime.AddHours(2)},
                new Candle() {Open = 6, Close = 8, Low = 1, High = 12, Date = secondDateTime},
                new Candle() {Open = 13, Close = 14, Low = -2, High = 14, Date = secondDateTime.AddHours(1)},
            }.ToList();

            var result = candlesAggregator.Aggregate(candles);

            Assert.AreEqual(2, result.Count);

            var first = result[0];
            var second = result[1];

            Assert.AreEqual(1, first.Open);
            Assert.AreEqual(4, first.Close);
            Assert.AreEqual(-1, first.Low);
            Assert.AreEqual(5, first.High);
            Assert.AreEqual(firstDateTime, first.Date);

            Assert.AreEqual(6, second.Open);
            Assert.AreEqual(14, second.Close);
            Assert.AreEqual(-2, second.Low);
            Assert.AreEqual(14, second.High);
            Assert.AreEqual(secondDateTime, second.Date);
        }

        [TestMethod]
        public void AggregateDays_Test3()
        {
            var settings = new AggregationSettings()
            {
                AggregationInteral = AggregationInteral.Day,
                DateTimeType = CandleDateTimeType.StartOfCandle
            };

            var candlesAggregator = new CandlesAggregator(settings);

            var firstDateTime = new DateTime(2017, 12, 5, 12, 20, 0);
            var secondDateTime = new DateTime(2017, 12, 6, 12, 20, 0);
            var thirdDateTime = new DateTime(2017, 12, 7, 12, 20, 0);

            var candles = new[]
            {
                new Candle {Open = 1, Close = 2, Low = 1, High = 2, Date = firstDateTime},
                new Candle() {Open = 1, Close = 2, Low = -1, High = 2, Date = firstDateTime.AddHours(1)},
                new Candle()  {Open = 1, Close = 4, Low = 1, High = 5, Date = firstDateTime.AddHours(2)},
                new Candle() {Open = 6, Close = 8, Low = 1, High = 12, Date = secondDateTime},
                new Candle() {Open = 13, Close = 14, Low = -2, High = 14, Date = secondDateTime.AddHours(1)},
                 new Candle() {Open = -1, Close = 12, Low = -6, High = 12, Date = thirdDateTime},
                new Candle() {Open = 12 , Close = 14, Low = 12, High = 14, Date = thirdDateTime.AddHours(1)},
                new Candle() {Open = 15 , Close = 10, Low = -7, High = 24, Date = thirdDateTime.AddHours(2)},
            }.ToList();

            var result = candlesAggregator.Aggregate(candles);

            Assert.AreEqual(3, result.Count);

            var first = result[0];
            var second = result[1];
            var third = result[2];

            Assert.AreEqual(1, first.Open);
            Assert.AreEqual(4, first.Close);
            Assert.AreEqual(-1, first.Low);
            Assert.AreEqual(5, first.High);
            Assert.AreEqual(firstDateTime, first.Date);

            Assert.AreEqual(6, second.Open);
            Assert.AreEqual(14, second.Close);
            Assert.AreEqual(-2, second.Low);
            Assert.AreEqual(14, second.High);
            Assert.AreEqual(secondDateTime, second.Date);

            Assert.AreEqual(-1, third.Open);
            Assert.AreEqual(10, third.Close);
            Assert.AreEqual(-7, third.Low);
            Assert.AreEqual(24, third.High);
            Assert.AreEqual(thirdDateTime, third.Date);
        }

        [TestMethod]
        public void AggregateDays_Test4()
        {
            var settings = new AggregationSettings()
            {
                AggregationInteral = AggregationInteral.Day,
                DateTimeType = CandleDateTimeType.EndOfCandle
            };

            var candlesAggregator = new CandlesAggregator(settings);

            var firstDateTime = new DateTime(2017, 12, 5, 12, 20, 0);
            var secondDateTime = new DateTime(2017, 12, 6, 12, 20, 0);
            var thirdDateTime = new DateTime(2017, 12, 7, 12, 20, 0);

            var candles = new[]
            {
                new Candle {Open = 1, Close = 2, Low = 1, High = 2, Date = firstDateTime.AddHours(-2)},
                new Candle() {Open = 1, Close = 2, Low = -1, High = 2, Date = firstDateTime.AddHours(-1)},
                new Candle()  {Open = 1, Close = 4, Low = 1, High = 5, Date = firstDateTime},
                new Candle() {Open = 6, Close = 8, Low = 1, High = 12, Date = secondDateTime.AddHours(-1)},
                new Candle() {Open = 13, Close = 14, Low = -2, High = 14, Date = secondDateTime},
                 new Candle() {Open = -1, Close = 12, Low = -6, High = 12, Date = thirdDateTime.AddHours(-2)},
                new Candle() {Open = 12 , Close = 14, Low = 12, High = 14, Date = thirdDateTime.AddHours(-1)},
                new Candle() {Open = 15 , Close = 10, Low = -7, High = 24, Date = thirdDateTime},
            }.ToList();

            var result = candlesAggregator.Aggregate(candles);

            Assert.AreEqual(3, result.Count);

            var first = result[0];
            var second = result[1];
            var third = result[2];

            Assert.AreEqual(1, first.Open);
            Assert.AreEqual(4, first.Close);
            Assert.AreEqual(-1, first.Low);
            Assert.AreEqual(5, first.High);
            Assert.AreEqual(firstDateTime, first.Date);

            Assert.AreEqual(6, second.Open);
            Assert.AreEqual(14, second.Close);
            Assert.AreEqual(-2, second.Low);
            Assert.AreEqual(14, second.High);
            Assert.AreEqual(secondDateTime, second.Date);

            Assert.AreEqual(-1, third.Open);
            Assert.AreEqual(10, third.Close);
            Assert.AreEqual(-7, third.Low);
            Assert.AreEqual(24, third.High);
            Assert.AreEqual(thirdDateTime, third.Date);
        }

        [TestMethod]
        public void AggregateHours_Test0()
        {
            var settings = new AggregationSettings()
            {
                AggregationInteral = AggregationInteral.Hour,
                DateTimeType = CandleDateTimeType.StartOfCandle
            };

            var candlesAggregator = new CandlesAggregator(settings);

            var candles = new List<Candle>();

            var result = candlesAggregator.Aggregate(candles);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void AggregateHours_Test1()
        {
            var settings = new AggregationSettings()
            {
                AggregationInteral = AggregationInteral.Hour,
                DateTimeType = CandleDateTimeType.StartOfCandle
            };

            var candlesAggregator = new CandlesAggregator(settings);

            var firstDateTime = new DateTime(2017, 12, 5, 12, 20, 0);

            var candles = new[]
            {
                new Candle {Open = 1, Close = 2, Low = 1, High = 2, Date = firstDateTime},
                new Candle() {Open = 1, Close = 2, Low = -1, High = 2, Date = firstDateTime.AddMinutes(5)},
                new Candle()  {Open = 1, Close = 4, Low = 1, High = 5, Date = firstDateTime.AddMinutes(5)},
            }.ToList();

            var result = candlesAggregator.Aggregate(candles);

            Assert.AreEqual(1, result.Count);

            var first = result[0];

            Assert.AreEqual(1, first.Open);
            Assert.AreEqual(4, first.Close);
            Assert.AreEqual(-1, first.Low);
            Assert.AreEqual(5, first.High);
            Assert.AreEqual(firstDateTime, first.Date);
            Assert.AreEqual(firstDateTime.Hour, first.Date.Hour);
        }

        [TestMethod]
        public void AggregateHours_Test2()
        {
            var settings = new AggregationSettings()
            {
                AggregationInteral = AggregationInteral.Hour,
                DateTimeType = CandleDateTimeType.StartOfCandle
            };

            var candlesAggregator = new CandlesAggregator(settings);

            var firstDateTime = new DateTime(2017, 12, 5, 12, 20, 0);
            var secondDateTime = new DateTime(2017, 12, 5, 13, 20, 0);
            var thirdDateTime = new DateTime(2017, 12, 6, 12, 20, 0);

            var candles = new[]
            {
                new Candle {Open = 1, Close = 2, Low = 1, High = 2, Date = firstDateTime},
                new Candle {Open = 1, Close = 2, Low = -1, High = 2, Date = firstDateTime.AddMinutes(1)},
                new Candle {Open = 1, Close = 4, Low = 1, High = 5, Date = firstDateTime.AddMinutes(5)},

                new Candle {Open = 6, Close = 8, Low = 1, High = 12, Date = secondDateTime},
                new Candle() {Open = 13, Close = 14, Low = -2, High = 14, Date = secondDateTime.AddMinutes(1)},

                new Candle() {Open = -1, Close = 12, Low = -6, High = 12, Date = thirdDateTime},
                new Candle() {Open = 12 , Close = 14, Low = 12, High = 14, Date = thirdDateTime.AddMinutes(1)},
                new Candle() {Open = 15 , Close = 10, Low = -7, High = 24, Date = thirdDateTime.AddMinutes(5)},

            }.ToList();

            var result = candlesAggregator.Aggregate(candles);

            Assert.AreEqual(3, result.Count);

            var first = result[0];
            var second = result[1];
            var third = result[2];

            Assert.AreEqual(1, first.Open);
            Assert.AreEqual(4, first.Close);
            Assert.AreEqual(-1, first.Low);
            Assert.AreEqual(5, first.High);
            Assert.AreEqual(firstDateTime, first.Date);

            Assert.AreEqual(6, second.Open);
            Assert.AreEqual(14, second.Close);
            Assert.AreEqual(-2, second.Low);
            Assert.AreEqual(14, second.High);
            Assert.AreEqual(secondDateTime, second.Date);

            Assert.AreEqual(-1, third.Open);
            Assert.AreEqual(10, third.Close);
            Assert.AreEqual(-7, third.Low);
            Assert.AreEqual(24, third.High);
            Assert.AreEqual(thirdDateTime, third.Date);
        }

        [TestMethod]
        public void AggregateHours_Test3()
        {
            var settings = new AggregationSettings()
            {
                AggregationInteral = AggregationInteral.Hour,
                DateTimeType = CandleDateTimeType.EndOfCandle
            };

            var candlesAggregator = new CandlesAggregator(settings);

            var firstDateTime = new DateTime(2017, 12, 5, 12, 20, 0);
            var secondDateTime = new DateTime(2017, 12, 5, 13, 20, 0);
            var thirdDateTime = new DateTime(2017, 12, 6, 12, 20, 0);
            var fourthDateTime = new DateTime(2017, 12, 7, 12, 20, 0);

            var candles = new[]
            {
                new Candle {Open = 1, Close = 2, Low = 1, High = 2, Date = firstDateTime.AddMinutes(-1)},
                new Candle {Open = 1, Close = 2, Low = -1, High = 2, Date = firstDateTime.AddMinutes(-2)},
                new Candle {Open = 1, Close = 4, Low = 1, High = 5, Date = firstDateTime},

                new Candle {Open = 6, Close = 8, Low = 1, High = 12, Date = secondDateTime.AddMinutes(-2)},
                new Candle() {Open = 13, Close = 14, Low = -2, High = 24, Date = secondDateTime.AddMinutes(-1)},
                new Candle() {Open = 13, Close = 14, Low = -2, High = 14, Date = secondDateTime},

                new Candle() {Open = -1, Close = 12, Low = -6, High = 12, Date = thirdDateTime.AddMinutes(-2)},
                new Candle() {Open = 12 , Close = 14, Low = 12, High = 14, Date = thirdDateTime.AddMinutes(-1)},
                new Candle() {Open = 15 , Close = 10, Low = -7, High = 24, Date = thirdDateTime},

                new Candle() {Open = 4, Close = 1, Low = -10, High = 0, Date = fourthDateTime.AddMinutes(-2)},
                new Candle() {Open = 5 , Close = 2, Low = -11, High = 1, Date = fourthDateTime.AddMinutes(-1)},
                new Candle() {Open = 6 , Close = 3, Low = -12, High = 2, Date = fourthDateTime},

            }.ToList();

            var result = candlesAggregator.Aggregate(candles);

            Assert.AreEqual(4, result.Count);

            var first = result[0];
            var second = result[1];
            var third = result[2];
            var fourth = result[3];

            Assert.AreEqual(1, first.Open);
            Assert.AreEqual(4, first.Close);
            Assert.AreEqual(-1, first.Low);
            Assert.AreEqual(5, first.High);
            Assert.AreEqual(firstDateTime, first.Date);

            Assert.AreEqual(6, second.Open);
            Assert.AreEqual(14, second.Close);
            Assert.AreEqual(-2, second.Low);
            Assert.AreEqual(24, second.High);
            Assert.AreEqual(secondDateTime, second.Date);

            Assert.AreEqual(-1, third.Open);
            Assert.AreEqual(10, third.Close);
            Assert.AreEqual(-7, third.Low);
            Assert.AreEqual(24, third.High);
            Assert.AreEqual(thirdDateTime, third.Date);

            Assert.AreEqual(4, fourth.Open);
            Assert.AreEqual(3, fourth.Close);
            Assert.AreEqual(-12, fourth.Low);
            Assert.AreEqual(2, fourth.High);
            Assert.AreEqual(fourthDateTime, fourth.Date);
        }

    }
}