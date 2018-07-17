using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Common.Finders
{
    public class DeviationsFromMovingFinder
    {
        private readonly Func<DateTime, bool> _isValidDateTime = (date) => (date.Hour > 10 || (date.Hour == 10 && date.Minute > 59));

        private readonly List<DatePrice> _ema = new List<DatePrice>();
        private List<Candle> _candles;

        public DeviationsFromMovingFinder(List<Candle> candles, int period)
        {
            _candles = candles;

            _ema = candles.Ema(period, PriceTypes.Mean).ToList();            
        }
        
        public List<double> FindFor(Direction direction)
        {
            var result = new List<double>();

            if (direction == Direction.Buy)
            {
                for (int i = 0; i < _candles.Count; i++)
                {
                    //if (_isValidDateTime(_candles[i].DateTime) == false) continue;

                    if (_candles[i].High > _ema[i].Value)
                    {
                        result.Add(_candles[i].High - _ema[i].Value);
                    }
                }
            }
            else if (direction == Direction.Sell)
            {
                for (int i = 0; i < _candles.Count; i++)
                {
                    //if (_isValidDateTime(_candles[i].DateTime) == false) continue;

                    if (_candles[i].Low < _ema[i].Value)
                    {
                        result.Add(_ema[i].Value - _candles[i].Low);
                    }
                }
            }
            else
            {
                throw new NotSupportedException(direction.ToString());
            }

            return result;
        }        
    }
}