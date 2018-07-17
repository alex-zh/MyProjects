using System;
using System.Linq;

namespace Visualizer.VisualControls.Classes
{    
    public class ValueRounder
    {       
       private double _minValue;
       private double _maxValue;
       private double[] _roundValues;

       public ValueRounder(double minValue, double maxValue)
       {
           _minValue = minValue;
           _maxValue = maxValue;
           _roundValues = new[] { 0.001, 0.005, 0.01, 0.05, 0.1, 0.5, 1, 5, 10, 50, 100 };
       }

       public double GetMaxValue()
       {
           return GetMinMaxValue(_maxValue, MinMax.Max);
       }

       public double GetMinValue()
       {
           return GetMinMaxValue(_minValue, MinMax.Min);
       }
           
       private double GetRoundValue()
       {
           var differenceDividedByTen = (_maxValue - _minValue) / 10;
           return _roundValues.OrderBy(x => Math.Abs(x - differenceDividedByTen)).First();
       }

       private double GetMinMaxValue(double value, MinMax minMax)
       {
           //var roundValue = GetRoundValue();
           //var power = (2 * roundValue * 1000).ToString().Length - 4;

           //if (minMax == MinMax.Max)
           //{
           //    return Math.Ceiling(value * (Math.Pow(10, -1 * power))) * Math.Pow(10, power);
           //}
           //else
           //{
           //    return Math.Floor(value * (Math.Pow(10, -1 * power))) * Math.Pow(10, power);
           //}           
           var roundValue = GetRoundValue();
           var power = (2 * roundValue * 1000).ToString().Length - 4;

           if (minMax == MinMax.Max)
           {
               value = Math.Round(value * (Math.Pow(10, -1 * power))) * Math.Pow(10, power);

               if (value < _maxValue) value += roundValue;

               return value;
           }
           else
           {
               value = Math.Round(value * (Math.Pow(10, -1 * power))) * Math.Pow(10, power);

               if (value > _minValue) value -= roundValue;

               return value;
           }
       }
    }
}
