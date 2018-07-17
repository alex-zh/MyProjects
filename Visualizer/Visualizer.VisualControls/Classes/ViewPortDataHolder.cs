using System;

namespace Visualizer.VisualControls.Classes
{
    public class ViewPortDataHolder
    {
        private double _viewPortX1;
        private double _viewPortX2;                

        private DateTime _viewPortStartDate;
        private DateTime _viewPortEndDate;
        private double _viewPortMaxValue;
        private double _viewPortMinValue;

        public void Init(double viewPortX1, double viewPortX2, ChartCalculator calculator)
        {
            if (calculator == null)
            {
                throw new ArgumentNullException("calculator");
            }

            _viewPortX1 = viewPortX1;
            _viewPortX2 = viewPortX2;            

            var candle1 = calculator.GetCandleByCoordinate(_viewPortX1);
            var candle2 = calculator.GetCandleByCoordinate(_viewPortX2);

            var value1 = calculator.GetRangeMinimum(candle1, candle2);
            var value2 = calculator.GetRangeMaximum(candle1, candle2);

            _viewPortStartDate = candle1.Date;
            _viewPortEndDate = candle2.Date;

            var valueRounder = new ValueRounder(value1, value2);

            _viewPortMaxValue = valueRounder.GetMaxValue();
            _viewPortMinValue = valueRounder.GetMinValue();            
        }

        public double ViewPortMaxValue
        {
            get
            {
                return _viewPortMaxValue;
            }
        }

        public double ViewPortMinValue
        {
            get
            {
                return _viewPortMinValue;
            }
        }

        public DateTime ViewPortStartDate
        {
            get
            {
                return _viewPortStartDate;
            }
        }

        public DateTime ViewPortEndDate
        {
            get
            {
                return _viewPortEndDate;
            }
        }
    }
}
