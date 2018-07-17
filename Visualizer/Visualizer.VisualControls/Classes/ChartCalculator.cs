using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Visualizer.VisualControls.Classes
{
    public class ChartCalculator : IChartCalculator
    {
        private double _canvasWidth;        
        private readonly double _candleOffset;
        private double _canvasHeight;
        private readonly double _candleHalfWidth;

        private readonly SortedList<DateTime, Candle> _sortedCandles;
        private double _minY = double.MinValue;
        private double _maxY = double.MaxValue;
        private DateTime _viewPortStartDate;
        private DateTime _viewPortEndDate;

        public ChartCalculator(IEnumerable<Candle> candles, double candleHalfWidth, 
                                                            double candleOffSet, 
                                                            double canvasWidth, 
                                                            double canvasHeight)
        {
            _sortedCandles = new SortedList<DateTime, Candle>(candles.ToDictionary(c => c.Date, c => c));
            
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
            _candleOffset = candleOffSet;            
            _candleHalfWidth = candleHalfWidth;
        }

        public void InitMinMaxValues(double viewPortMinValue, double viewPortMaxValue, 
                                     DateTime viewPortStartDate, DateTime viewPortEndDate)
        {
            _minY = viewPortMinValue;
            _maxY = viewPortMaxValue;
            _viewPortStartDate = viewPortStartDate;
            _viewPortEndDate = viewPortEndDate;
        }

        public IEnumerable<Candle> Candles
        {
            get
            {
                return _sortedCandles.Values;
            }
        }

        public int IndexOf(Candle candle)
        {
            return GetIndexByDate(candle.Date);
        }

        public int GetIndexByDate(DateTime date)
        {
            return _sortedCandles.IndexOfKey(date);
        }
        
        public double CanvasWidth
        {
            get { return _canvasWidth; }
            set { _canvasWidth = value; }
        }

        public double CanvasHeight
        {
            get { return _canvasHeight; }
            set { _canvasHeight = value; }
        }

        public double   GetCoordinateByIndex(int index)
        {
            return _candleHalfWidth + index * (2 * _candleHalfWidth + _candleOffset);            
        }

        public double GetCoordinateOfFirstVisibleCandle()
        {
            return GetCoordinateByDate(_viewPortStartDate);
        }

        public double GetCoordinateOfLastVisibleCandle()
        {
            return GetCoordinateByDate(_viewPortEndDate);
        }

        public double GetCoordinateByDate(DateTime candleDate, bool exactMatch = true)
        {
            int candleIndex = 0;

            if (exactMatch)
            {
                candleIndex = _sortedCandles.IndexOfKey(candleDate);
            }
            else
            {
                var nearestCandleDate = _sortedCandles.TakeWhile(x => x.Key <= candleDate).Select(x=>x.Key).LastOrDefault();

                if (nearestCandleDate == default(DateTime))
                {
                    nearestCandleDate = _sortedCandles.TakeWhile(x => x.Key > candleDate).Select(x => x.Key).First();
                }

                candleIndex = _sortedCandles.IndexOfKey(nearestCandleDate);               
            }

            return GetCoordinateByIndex(candleIndex);
        }

        public double GetCoordinateByDateUniformlyDistributed(DateTime candleDate)
        {
            double minValue = GetMinDate().ToOADate();
            double maxValue = GetMaxDate().ToOADate();

            double value = candleDate.ToOADate();

            return (value - minValue) * _canvasWidth / (maxValue - minValue);
        }

        public double GetCoordinateByValue(double value)
        {
            if (_minY == double.MinValue || _maxY == double.MaxValue)
            {
                throw new ArgumentException("MinY and MaxY are not specified");
            }

            return GetCoordinateByValue(value, _minY, _maxY);
        }

        public double GetCoordinateForMinValue()
        {
            if (_minY == double.MinValue)
            {
                throw new ArgumentException("MinY is not specified");
            }

            return GetCoordinateByValue(_minY, _minY, _maxY);
        }

        public double GetCoordinateForMaxValue()
        {
            if (_maxY == double.MaxValue)
            {
                throw new ArgumentException("MaxY is not specified");
            }

            return GetCoordinateByValue(_maxY, _minY, _maxY);
        }

        public double GetCoordinateByValue(double value, double minValue, double maxValue)
        {
            if (minValue > value) return 0;
            if (Math.Abs(minValue - maxValue) < double.Epsilon) return 0;
            
            return _canvasHeight - (value - minValue) * _canvasHeight / (maxValue - minValue);
        }

        public double GetValue(double value)
        {
            if (_minY == double.MinValue || _maxY == double.MaxValue)
            {
                throw new ArgumentException("MinY and MaxY are not specified");
            }

            return (value) * _canvasHeight / (_maxY - _minY);
        }

        public double GetValueByCoordinate(double coordinate)
        {
            if (_minY == double.MinValue || _maxY == double.MaxValue)
            {
                throw new ArgumentException("MinY and MaxY are not specified");
            }

            return GetValueByCoordinate(coordinate, _minY, _maxY);
        }

        public double GetValueByCoordinate(double coordinate, double minValue, double maxValue)
        {

            return minValue + (_canvasHeight - coordinate) * (maxValue - minValue) / _canvasHeight;
        }

        public Candle GetCandleByCoordinate(double candleCoordinate)
        {
            int index = Convert.ToInt32((candleCoordinate - (_candleOffset)) / (2 * _candleHalfWidth + _candleOffset));

            var candles = new List<Candle>();

            if (index > _sortedCandles.Count)
            {
                return _sortedCandles.OrderBy(x => Math.Abs(GetCoordinateByDate(x.Value.Date) - candleCoordinate)).First().Value;
            }

            for (int i = Math.Max(index - 3, 0); i < Math.Min(index + 3, _sortedCandles.Count); i++)
            {
                candles.Add(_sortedCandles.Values[i]);
            }

            return candles.OrderBy(x => Math.Abs(GetCoordinateByDate(x.Date) - candleCoordinate)).First();
        }        

        public double GetRangeMinimum(Candle candle1, Candle candle2)
        {
            return _sortedCandles.Where(x => x.Key >= candle1.Date && x.Key <= candle2.Date).Min(x => x.Value.Low);
        }

        public double GetRangeMaximum(Candle candle1, Candle candle2)
        {
            return _sortedCandles.Where(x => x.Key >= candle1.Date && x.Key <= candle2.Date).Max(x => x.Value.High);
        }

        public double CalculateCanvasWidth()
        {
            return _sortedCandles.Count * (_candleHalfWidth * 2 + _candleOffset);
        }

        public double GetMaxValue()
        {
            return _sortedCandles.Max(x => Math.Max(x.Value.Price, x.Value.High));
        }

        public double GetMinValue()
        {
            return _sortedCandles.Min(x => Math.Min(x.Value.Price, x.Value.Low));
        }

        public DateTime GetMaxDate()
        {
            return _sortedCandles.Keys.Max();
        }

        public DateTime GetMinDate()
        {
            return _sortedCandles.Keys.Min();
        }      
    }
}
