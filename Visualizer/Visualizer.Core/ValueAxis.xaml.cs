using System.Windows;
using System.Windows.Controls;
using Visualizer.Core.Classes;

namespace Visualizer.Core
{    
    public partial class ValueAxis
    {
        public ValueAxis()
        {
            InitializeComponent();
        }

        public double AxisHeight
        {
            get { return canvasAxis.Height; }
            set { canvasAxis.Height = value; }
        }

        private double MinValue
        {
            get;
            set;
        }

        private double MaxValue
        {
            get;
            set;
        }

        public void Init(double minValue, double maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            
            canvasAxis.Children.Clear();

            DrawLables();            
        }        

        private void DrawLables()
        {
            var ticksCalc = new ValueAxisTicksCalculator(MinValue, MaxValue);
                        
            var x = grid.Width / 2;

            foreach (double dy in ticksCalc.GetTicks())            
            {
                var y = NormalizeY(dy, MaxValue, MinValue);

                var tb = new TextBlock();
                tb.Text = dy.ToString(ticksCalc.GetFormat());
                tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                var size = tb.DesiredSize;
                canvasAxis.Children.Add(tb);
                Canvas.SetLeft(tb, x - size.Width / 2);
                Canvas.SetTop(tb, y - 3 - size.Height / 2);
            }
        }
                
        private double NormalizeY(double y, double maxY, double minY)
        {
            return canvasAxis.Height - (y - minY) * canvasAxis.Height / (maxY - minY);            
        }
    }
}
