using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.General;

namespace Common
{
    /// <summary>
    /// Если мы выше мувинга, смотрим будет ли закрытие через n дней выше текущего на x%
    /// </summary>
    public class AboveBelowMovingProfitFinder
    {
       private readonly List<Candle> _candles;
       private readonly List<DatePrice> ema11List;
       private readonly List<DatePrice> ema21List;

        public AboveBelowMovingProfitFinder(IEnumerable<Candle> candles)
        {
            _candles = candles.ToList();

           ema11List = _candles.Ema(11).ToList();
           ema21List = _candles.Ema(21).ToList();
        
        }

        public void GetStatistics()
        {
            var upResultList = new List<Dictionary<int, double>>();
            var downResultList = new List<Dictionary<int, double>>();
            var upResultListReverse = new List<Dictionary<int, double>>();
            var downResultListReverse = new List<Dictionary<int, double>>();

            int downTotalCount = 0;
            int upTotalCount=0;

            for (int i = 22; i < _candles.Count-21; i++)
            {
                var meanValue = ema11List[i].Value;
                var previousCandle = _candles[i-1];
                var currentCandle = _candles[i];
                var value = currentCandle.Close;

                if (meanValue > ema21List[i].Value && currentCandle.Close > meanValue && previousCandle.Close>meanValue)
                {
                    upTotalCount++;
                    upResultList.Add(GetBehaviourAfterCandleNormal(i, 20,value, false));
                    upResultListReverse.Add(GetBehaviourAfterCandleReverse(i, 20, currentCandle.Close, false));  
                }
                else if (meanValue < ema21List[i].Value && currentCandle.Close < meanValue && previousCandle.Close < meanValue)
                {
                    downTotalCount++;
                    //если минимум свечи больше значения по которому мы пытаеся посчитать, это неправильно т.к входа не будет
                    downResultList.Add(GetBehaviourAfterCandleNormal(i, 20, value, true));
                    downResultListReverse.Add(GetBehaviourAfterCandleReverse(i, 20, value, true));    
                }              
            }

            var dumper = new Dumper<object>(@"C:\result\result.txt");


            dumper.WriteLine("Buy count: " + upTotalCount+ "\n");
            dumper.WriteLine("Sell count: " + downTotalCount + "\n");

            for (int i = 1; i < 6; i++)
            {
               var  testChange = 0.2 * i;

                dumper.WriteLine("\nChange: " + testChange + "\n");
                
              var resultHeaderString = string.Empty;
                var resultUpValue1String = string.Empty;
                var resultUpValue2String = string.Empty;
                var resultUpReverseValue1String = string.Empty;
                var resultUpReverseValue2String = string.Empty;

                var resultDownValue1String = string.Empty;
                var resultDownValue2String = string.Empty;
                var resultDownReverseValue1String = string.Empty;
                var resultDownReverseValue2String = string.Empty;

                for (int candleIndex = 1; candleIndex < 20; candleIndex++)
                {
                    var upCount = upResultList.Count(x => x.Where(d => d.Key <= candleIndex).Select(d => d.Value).Max() >= testChange);
                    var downCount = downResultList.Count(x => x.Where(d => d.Key <= candleIndex).Select(d => d.Value).Min() <= -1* testChange);

                    var upCountReverse = upResultListReverse.Count(x => 
                                                                    (candleIndex == 1 || x.Where(d => d.Key < candleIndex).Select(d => d.Value).Max() < testChange) &&
                                                                    x.Where(d => d.Key <= candleIndex).Select(d => d.Value).Min() <= -1 * testChange);

                    var downCountReverse = downResultListReverse.Count(x =>
                                                             (candleIndex == 1 || x.Where(d => d.Key < candleIndex).Select(d => d.Value).Min() > -1*testChange) &&
                                                                x.Where(d => d.Key <= candleIndex).Select(d => d.Value).Max() >= testChange);

                    resultHeaderString = resultHeaderString  + candleIndex + "; ";
                    resultUpValue1String = resultUpValue1String + upCount + "; ";
                    resultUpValue2String =  resultUpValue2String + (upCount * 100 / upTotalCount) + "%; ";

                    resultUpReverseValue1String = resultUpReverseValue1String + upCountReverse + "; ";
                    resultUpReverseValue2String = resultUpReverseValue2String + (upCountReverse * 100 / upTotalCount) + "%; ";

                    resultDownValue1String =  resultDownValue1String+downCount + "; ";
                    resultDownValue2String = resultDownValue2String + (downCount * 100 / downTotalCount) + "%; ";

                    resultDownReverseValue1String = resultDownReverseValue1String + downCountReverse + "; ";
                    resultDownReverseValue2String = resultDownReverseValue2String + (downCountReverse * 100 / downTotalCount) + "%; ";
                }

                dumper.WriteLine("\n Buy \n");
                dumper.WriteLine(resultHeaderString);
                dumper.WriteLine(resultUpValue1String);
                dumper.WriteLine(resultUpValue2String);
                dumper.WriteLine("Reverse");
                dumper.WriteLine(resultUpReverseValue1String);
                dumper.WriteLine(resultUpReverseValue2String);
                
                dumper.WriteLine("\n Sell \n");
                dumper.WriteLine(resultHeaderString);
                dumper.WriteLine(resultDownValue1String);
                dumper.WriteLine(resultDownValue2String);
                dumper.WriteLine("Reverse");
                dumper.WriteLine(resultDownReverseValue1String);
                dumper.WriteLine(resultDownReverseValue2String);
                
                
            }
        }

        private Dictionary<int, double> GetBehaviourAfterCandleNormal(int candleIndex, int countOfCandels, double theshold, bool up)
        {
            var candle = _candles[candleIndex];
            var result = new Dictionary<int, double>();

            for (int i = 1; i < countOfCandels; i++)
            {
                var nextCandleIndex = candleIndex + i;
                if (nextCandleIndex >= _candles.Count) break;

                var nextCandleValue = _candles[nextCandleIndex].Close;
                if (up)
                {
                    nextCandleValue = _candles[nextCandleIndex].Low;
                }
                else
                {
                    nextCandleValue = _candles[nextCandleIndex].High;
                }

                var difference = (nextCandleValue - theshold) * 100 / theshold;

                result.Add(i, difference);
            }

            return result;
        }

        private Dictionary<int, double> GetBehaviourAfterCandleReverse(int candleIndex, int countOfCandels, double theshold, bool up)
        {
            var candle = _candles[candleIndex];
            var result = new Dictionary<int, double>();

            for (int i = 1; i < countOfCandels; i++)
            {
                var nextCandleIndex = candleIndex + i;
                if (nextCandleIndex >= _candles.Count) break;

                var nextCandleValue = _candles[nextCandleIndex].Close;
                if (up)
                {
                    nextCandleValue = _candles[nextCandleIndex].High;
                }
                else
                {
                    nextCandleValue = _candles[nextCandleIndex].Low;
                }

                var difference = (nextCandleValue - theshold) * 100 / theshold;

                result.Add(i, difference);
            }

            return result;
        }

       
        private IEnumerable<DeviationPoint> GetAllDeviations(Candle candle, double meanPrice)
        {
            var  result = new List<DeviationPoint>();

            if (candle.Low >= meanPrice)
            {
                var dp = new DeviationPoint();
                dp.Percent = (candle.High - meanPrice)*100/meanPrice;
                dp.Value = candle.High - meanPrice;
                dp.ShadowValue = candle.High - Math.Max(candle.Close, candle.Open);

                result.Add(dp);
            }
            else if(candle.High <= meanPrice)
            {
                var dp = new DeviationPoint
                {
                    Percent = (candle.Low - meanPrice)*100/meanPrice,
                    Value = candle.Low - meanPrice,
                    ShadowValue =  Math.Max(candle.Close, candle.Open)- candle.Low
                };
                result.Add(dp);
            }
            else
            {
                var dp = new DeviationPoint
                {
                    Percent = (candle.High - meanPrice) * 100 / meanPrice,
                    Value = candle.High - meanPrice,
                    ShadowValue = candle.High - Math.Max(candle.Close, candle.Open)
                };
                result.Add(dp);
                dp = new DeviationPoint
                {
                    Percent = (candle.Low - meanPrice) * 100 / meanPrice,
                    Value = candle.Low - meanPrice,
                    ShadowValue = Math.Max(candle.Close, candle.Open) - candle.Low
                };
                result.Add(dp);
                
            }

            return result;
        }

        /// <summary>
        /// Если свеча "пересекает" средную пока возвращаем ноль, потом возможно нужно тоже 2 значения возвращать
        /// </summary>
        public DeviationInfo GetDeviation(Candle candle, double meanPrice)
        {
            var result = new DeviationInfo();
            result.MeanValue = meanPrice;

            if (candle.Low >= meanPrice)
            {
                result.Value = candle.High;                
                result.Percent = (candle.High - meanPrice) * 100 / meanPrice;
            }
            if (candle.High <= meanPrice)
            {
                result.Value = candle.Low;                
                result.Percent = (candle.Low - meanPrice) * 100 / meanPrice;
            }

            return result;
        }
    }  
}