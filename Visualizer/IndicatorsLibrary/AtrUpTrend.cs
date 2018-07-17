using System.Collections.Generic;
using Common;
using Common.Classes;
using Common.Classes.General;

namespace Core
{   
    public class AtrUpTrend
    {
        private int _countOfObligatoryUpSteps;
        private Candle[] _candles;
        private const int atrLength = 16;

        public AtrUpTrend(Candle[] candles, int countOfObligatoryUpSteps)
        {
            _candles = candles;
            _countOfObligatoryUpSteps = countOfObligatoryUpSteps;
            FoundIndices = new SortedList<int, int>();
        }

        /// <summary>
        /// номер тренда/найденные индексы свечей
        /// </summary>
        public SortedList<int, int> FoundIndices
        {
            get;
            private set;
        }

        public void Calculate()
        {
            var result = new List<int>();
            int transitionCount = 0;
            var trendNo = 0;

            for (int i = atrLength; i < _candles.Length - 1; i++)
            {
                transitionCount = 0;

                var currentCandle = _candles[i];

                var atr = new Atr(_candles, atrLength).Calculate(i);

                var rangeChecker = new RangeChecker(currentCandle, atr);

                var indices = new List<int>();
                var baseRangeIndices = new List<int>();

                indices.Add(i);
                
                for (int j = i + 1; j < _candles.Length; j++)
                {
                    var nextCandle = _candles[j];
                    
                    if (rangeChecker.IsInsideTransitionRange(nextCandle))
                    {
                        transitionCount++;
                                                 
                         atr = new Atr(_candles, atrLength).Calculate(j);

                         rangeChecker = new RangeChecker(nextCandle, atr);

                         indices.AddRange(baseRangeIndices);
                         baseRangeIndices.Clear();

                         indices.Add(j);
                    }
                    else if (rangeChecker.IsInsideBaseRange(nextCandle))
                    {
                        baseRangeIndices.Add(j);
                        continue;
                    }
                    else
                    {
                        if (transitionCount >= _countOfObligatoryUpSteps)
                        {
                            foreach (var index in indices)
                            {
                                FoundIndices.Add(index, trendNo);
                            }

                            trendNo++;
                            i = j + 1;
                        }
                        //go to the next candle
                        break;
                    }                   
                }
            }

        }      

    }
}
