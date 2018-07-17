using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Classes;
using Core;

namespace Strategies
{
    public class DeviationCorrector
    {
        private readonly int VolatilityGroupsCount = 3;
        private readonly IVolatilityCalculator _volatilityCalculator;
        private readonly DeviationFinder _deviationFinder;
        public List<VolatilityGroup> _volatilityGroups = new List<VolatilityGroup>();
        public List<Coefficient> _coefficients = new List<Coefficient>
        {
            new Coefficient(){Value = 1.5}, 
            new Coefficient(){Value = 2}, 
            new Coefficient(){Value = 2.5}, 
            new Coefficient(){Value = 3}            
        };
        //VolatilityGroup -> Coeffient -> candle index <-> change

        private readonly Dictionary<VolatilityGroup, Dictionary<Coefficient, List<Dictionary<int, double>>>> _result = 
            new Dictionary<VolatilityGroup, Dictionary<Coefficient, List<Dictionary<int, double>>>>();

        private readonly List<Candle> _candles;
        private Dumper<object> _dumper;

        public DeviationCorrector(IEnumerable<Candle> candles, 
                                  IVolatilityCalculator volatilityCalculator, 
                                  DeviationFinder deviationFinder)
        {
            _candles = candles.ToList();
            _volatilityCalculator = volatilityCalculator;
            _deviationFinder = deviationFinder;
            _dumper = new Dumper<object>(@"C:\result\result.txt");
        }

        public void Calculate()
        {
            InitVolatilityGroups();

            foreach (var volatilityGroup in _volatilityGroups)
            {
                var resultForGroup = CalculateForGroup(volatilityGroup);

                _result.Add(volatilityGroup, resultForGroup);                
            }

            WriteAggregatedResult();
        }

        private void WriteAggregatedResult()
        {
            foreach (var volatilityGroup in _result.Keys)
            {
                _dumper.WriteLine(volatilityGroup.ToString() + "\n\n");

                foreach (var coefficient in _result[volatilityGroup].Keys)
                {
                    var allCandlesResult = _result[volatilityGroup][coefficient];

                    if (allCandlesResult.Any() == false) continue;

                    var aggregatedResult = new Dictionary<int, double>();
                    
                    for (int i = 1; i < 10; i++)
                    {
                        var aggregatedValue = allCandlesResult.Where(x => x.ContainsKey(i)).Average(x => x[i]);
                        aggregatedResult.Add(i, aggregatedValue);
                    }

                    _dumper.WriteLine("\n\n Coefficient: " + coefficient.Value.ToString() + "; Count: " + allCandlesResult.Count + "\n\n");

                    var resultLineHeader = string.Empty;
                    var resultLineValue = string.Empty;

                    for (int i = 0; i < 10; i++)
                    {
                        resultLineHeader = resultLineHeader + i + ";";
                        var value = " ;";
                        if (aggregatedResult.ContainsKey(i))
                        {
                            value = aggregatedResult[i] + ";";
                        }
                        resultLineValue = resultLineValue + value;
                    }
                    _dumper.WriteLine(resultLineHeader);
                    _dumper.WriteLine(resultLineValue);
                }
            }
        }

        private Dictionary<Coefficient, List<  Dictionary<int, double>>> CalculateForGroup(VolatilityGroup volatilityGroup)
        {
            var resultForGroup = new Dictionary<Coefficient, List<Dictionary<int, double>>>();

            foreach (var coefficient in _coefficients)
            {
                var resultForCoefficient = CalculateForCoefficient(volatilityGroup, coefficient);
                resultForGroup.Add(coefficient, resultForCoefficient);
            }
            return resultForGroup;
        }


        private List<Dictionary<int, double>> CalculateForCoefficient(VolatilityGroup volatilityGroup, Coefficient coefficient)
        {
            var allCandlesResult = new List<Dictionary<int, double>>();

            for (int i = 0; i < _candles.Count; i++)
            {
                var currentVolatility = _volatilityCalculator.Get(i);

                if (currentVolatility < 0.0001 || volatilityGroup.IsInGroup(currentVolatility) == false)
                {
                    continue;
                }

                if (_deviationFinder.IsDeviated(i, coefficient.Value * currentVolatility))
                {
                    var resultForCurrentCandleAndCoefficient = CalculateForCandle(i);

                    allCandlesResult.Add(resultForCurrentCandleAndCoefficient);
                }
            }

            return allCandlesResult;
           
        }


        //ищем поведение для следующих 10 свечей, здесь мы оказались если отклонение оказалось сильное
        private Dictionary<int, double> CalculateForCandle(int candleIndex)
        {
            var candle = _candles[candleIndex];
            var result = new Dictionary<int, double>();

            for (int i = 1; i < 10; i++)
            {
                var nextCandleIndex = candleIndex + i;
                if (nextCandleIndex >= _candles.Count) break;

                var difference = (_candles[nextCandleIndex].Close - candle.Close) * 100 / candle.Close;

                result.Add(i, difference);
            }

            return result;
        }

        public void InitVolatilityGroups()
        {
            var volatilityList = _volatilityCalculator.GetAll().Select(x => x.Value).OrderBy(x => x).ToList();
            var volatiltityListWithoutAbnormalValues = volatilityList.OrderByDescending(x => x).Skip(volatilityList.Count/200);

            var minValue = volatiltityListWithoutAbnormalValues.Min();
            var maxValue = volatiltityListWithoutAbnormalValues.Max();

            var step = (maxValue - minValue)/VolatilityGroupsCount;

            //int step = volatilityList.Count / (VolatilityGroupsCount);

            for (int i = 0; i < VolatilityGroupsCount; i++)
            {
                var group = new VolatilityGroup
                {
                    Index = i
                };

                if (i == 0)
                {
                    group.MinValue = 0;
                    group.MaxValue = step;
                }
                else if (i == VolatilityGroupsCount - 1)
                {
                    group.MinValue = step * i;
                    group.MaxValue = double.MaxValue;
                }
                else
                {
                    group.MinValue = step * i;
                    group.MaxValue = step * (i + 1);
                }
                group.Count = volatilityList.Count(x => x >= @group.MinValue && x < @group.MaxValue);

                _volatilityGroups.Add(group);
            }

        }

        private void InitVolatilityGroupsOld()
        {
            var volatilityList = _volatilityCalculator.GetAll().Select(x => x.Value).OrderBy(x => x).ToList();

            int step = volatilityList.Count/(VolatilityGroupsCount);

            for (int i = 0; i < VolatilityGroupsCount; i++)
            {
                var group = new VolatilityGroup
                {
                    Index = i
                };

                if (i == 0)
                {
                    group.MinValue = 0;
                    group.MaxValue = volatilityList[step - 1];
                }
                else if (i == VolatilityGroupsCount - 1)
                {
                    group.MinValue = volatilityList[step*i - 1];
                    group.MaxValue = double.MaxValue;
                }
                else
                {
                    group.MinValue = volatilityList[step*i - 1];
                    group.MaxValue = volatilityList[step*(i + 1) - 1];
                }

                _volatilityGroups.Add(group);
            }

        }
    }
}
