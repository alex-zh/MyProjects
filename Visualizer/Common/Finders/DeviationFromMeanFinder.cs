using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Common.Classes;
using Common.Classes.General;

namespace Common
{
    /// <summary>
    /// Находит все отклонения от средней линии. В результате можем построить распределение отклонений
    /// </summary>
    public class DeviationFromMeanFinder
    {
       private readonly List<Candle> _candles;
        private readonly List<DatePrice> _averageLine;

        public DeviationFromMeanFinder(IEnumerable<Candle> candles)
        {
            _candles = candles.ToList();

             _averageLine = _candles.Ema(11, PriceTypes.Mean).ToList();            
        }

        public IEnumerable<int> GetStatistics(out List<DatePrice> downThresholdLine, out List<DatePrice> upThresholdLine)
        {
            var result = new List<int>();
            downThresholdLine = new List<DatePrice>();
            upThresholdLine = new List<DatePrice>();

            var upResultList = new List<Dictionary<int, double>>();
            var downResultList = new List<Dictionary<int, double>>();
            var upResultListReverse = new List<Dictionary<int, double>>();
            var downResultListReverse = new List<Dictionary<int, double>>();
            
            int downTotalCount = 0;
            int upTotalCount=0;

            for (int i = 0; i < 121; i++)
            {
                downThresholdLine.Add(_averageLine[i]);
                upThresholdLine.Add(_averageLine[i]);
            }

            for (int i = 121; i < _candles.Count; i++)
            {
                var downThreshold = FindThreshold(i, false);
                var upThreshold = FindThreshold(i, true);
                downThreshold.Percent = downThreshold.Percent;
                downThreshold.Value = downThreshold.Value;

                upThreshold.Percent = upThreshold.Percent;
                upThreshold.Value = upThreshold.Value;

                var meanValue = _averageLine[i].Value;
                var currentCandle = _candles[i];
                
                downThresholdLine.Add(new DatePrice
                {
                    Date = _averageLine[i].Date, 
                    Value = _averageLine[i].Value+ downThreshold.Value
                });

                upThresholdLine.Add(new DatePrice
                {
                    Date = _averageLine[i].Date,
                    Value = _averageLine[i].Value + upThreshold.Value
                });

                //выход + заполение линий отклонения
                if (i >= _candles.Count - 21) continue;

                var deviation = GetDeviation(currentCandle, meanValue);

                if (deviation.Percent < downThreshold.Percent)
                {
                    var value = meanValue + downThreshold.Value ;
                    //if (currentCandle.Low <= value)
                    {
                        result.Add(i);
                        downTotalCount++;
                        //если минимум свечи больше значения по которому мы пытаеся посчитать, это неправильно т.к входа не будет
                        downResultList.Add(GetBehaviourAfterCandleNormal(i, 20, value, false));
                        downResultListReverse.Add(GetBehaviourAfterCandleReverse(i, 20, value, false));    
                    }
                    
                }
                else if (deviation.Percent > upThreshold.Percent)
                {
                    var value = meanValue + upThreshold.Value;
                    //if (currentCandle.High >= value)
                    {
                        result.Add(i);
                        upTotalCount++;
                        upResultList.Add(GetBehaviourAfterCandleNormal(i, 20, value, true));
                        upResultListReverse.Add(GetBehaviourAfterCandleReverse(i, 20, value, true));    
                    }                    
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
                    var upCount = upResultList.Count(x => x.Where(d => d.Key <= candleIndex).Select(d => d.Value).Min() <= (-1) * testChange);
                    var downCount = downResultList.Count(x => x.Where(d => d.Key <= candleIndex).Select(d => d.Value).Max() >= testChange);

                    var upCountReverse = upResultListReverse.Count(x => 
                                                                    (candleIndex == 1 || x.Where(d => d.Key < candleIndex).Select(d => d.Value).Min() > (-1) * testChange) && 
                                                                    x.Where(d => d.Key <= candleIndex).Select(d => d.Value).Max() >= testChange);

                    var downCountReverse = downResultListReverse.Count(x =>
                                                             (candleIndex == 1 || x.Where(d => d.Key < candleIndex).Select(d => d.Value).Max() < testChange) &&
                                                            x.Where(d => d.Key <= candleIndex).Select(d => d.Value).Min() <= (-1) * testChange);

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

            return result;
        }

        private DeviationPoint FindThreshold(int candleIndex, bool isUpThreshold)
        {
            var all = FindAll(candleIndex - 50, candleIndex);
            //var last = all.Select(x=>new DeviationPoint(){Percent = Math.Abs(x.Percent), Value = Math.Abs(x.Value)}).OrderByDescending(x =>  x.Percent).Take(all.Count() / 10).Last();
            var last = new DeviationPoint()
            {
                Percent = all.Average(x => Math.Abs(x.Percent)),
                Value = all.Average(x => Math.Abs(x.Value))
            };

            if (isUpThreshold)
            {
                return last;
            }

            return new DeviationPoint(){Percent = last.Percent*-1, Value = last.Value*-1};
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

        public IEnumerable<DistributionBin> GetBins()
        {
            var all = FindAll().Select(x=>x.Percent);
            var minValue = all.Min();
            var maxValue = all.Max();

            var bins = new List<DistributionBin>();

            //var step = (maxValue - minValue)/40;
            var step = 0.08;
            
            var lowerBound = minValue;
            var upperBound = minValue+step;

            while (upperBound< maxValue)
            {
                var count = all.Count(x => x >= lowerBound && x < upperBound);

                bins.Add(new DistributionBin(){ MinValue = lowerBound, MaxValue = upperBound, Count = count});

                lowerBound = upperBound;
                upperBound = upperBound + step;
            }

            return bins;
        }

        public IEnumerable<DeviationPoint> FindAll()
        {
            return FindAll(21, _candles.Count());
        }

        public IEnumerable<DeviationPoint> FindAll(int startIndex, int endIndex)
        {
            var result = new List<DeviationPoint>();

            for (int i = Math.Max(0, startIndex); i < endIndex; i++)
            {
                var deviations = GetAllDeviations(_candles[i], _averageLine[i].Value);

                result.AddRange(deviations);
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

    public class DeviationPoint
    {
        public double Value { get; set; }
        public double Percent { get; set; }
        public double ShadowValue { get; set; }
    }

    public class DeviationInfo
    {
        public double MeanValue { get; set; }
        public double Percent { get; set; }
        public double Value { get; set; }
    }
}