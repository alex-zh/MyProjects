using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Visualizer.VisualControls.Classes;

namespace JointAssetsViewer
{
    internal class RandomChartPointGenerator
    {
        private Random _random;

        public RandomChartPointGenerator()
        {
            _random = new Random(DateTime.Now.Second);
        }

        public List<BivariateChartPoint> Generate<T>(int count, Func<int, T> dataConstructor)
        {
            var result = new List<BivariateChartPoint>();

            var operation = GetOperation();

            for (int i = 0; i < count; i++)
            {
                var firstValue = _random.Next(4800, 5200);
                var secondValue = _random.Next(60000, 62000); //GetAfterOperation(firstValue, _random.Next(50, 120), operation);
                result.Add(new BivariateChartPoint() {
                    Index = i, 
                    Brush = Brushes.Red, 
                    Radius = 5,
                    FirstValue = firstValue, 
                    SecondValue = secondValue,
                   Data = dataConstructor(i),

                });
            }

            for(int i = result.Count - 20; i<result.Count; i++)
            {
                result[i].Brush = Brushes.LightBlue;
            }

            result.Last().Brush = Brushes.DarkBlue;
            result.Last().Radius = 9;
            return result;
        }

        private int GetAfterOperation(int firstValue, int next, int operation)
        {
            if (operation == 1)
                return firstValue + next;
            if (operation == 2)
                return firstValue - next;
            if (operation == 3)
                return firstValue + 2 * next;
            if (operation == 4)
               return firstValue * 2+next;
            if (operation == 4)
                return firstValue / 2 - next;

            return 0;
        }

        private int GetOperation()
        {
            var v = _random.Next(1, 4);

            return v;
        }
    }
}