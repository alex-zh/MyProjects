using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using dforest = alglib.dforest;

namespace TestRandomForest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var xy = new double[1000, 3];
            var numberOfPoints = 1000;

            InitPoints(xy, numberOfPoints);

            Draw(xy);
            //return;
            var numberOfVar = 2;
            var numberOfClasses = 3;
            var r = 0.25;
            int info = 0;

            dforest.dfreport report = new dforest.dfreport();
            dforest.decisionforest df = new dforest.decisionforest();

            //  dforest.dfbuildinternal(xy, numberOfPoints, 1, 2, ntrees, (int)Math.Round(0.05 * npoints), 1, 0, ref info, df, rep);

            dforest.dfbuildrandomdecisionforest(xy,
                                                numberOfPoints,
                                                numberOfVar,
                                                numberOfClasses,
                                                75,
                                                r,
                                                ref info,
                                                df,
                                                report);

            DrawPredictedPoints(df);

        }

        private void DrawPredictedPoints(dforest.decisionforest df)
        {
            var x = new double[2];
            var y = new double[3];

            for (int i = 0; i < 120; i++)
            {
                for (int j = 0; j < 120; j++)
                {
                    x[0] = i;
                    x[1] = j;

                    dforest.dfprocess(df, x, ref y);

                    if (y[0] >= y[1] && y[0] >= y[2])
                    {
                        DrawPoint(i, j, 0);
                    }
                    else if (y[1] >= y[0] && y[1] >= y[2])
                    {
                        DrawPoint(i, j, 1);
                    }
                    else if (y[2] >= y[0] && y[2] >= y[1])
                    {
                        DrawPoint(i, j, 2);
                    }


                }
            }

        }

        private void DrawPoint(int x, int y, int classId)
        {
            var ellipse = new Ellipse
            {
                Width = 4,
                Height = 4
            };
            if (classId == 0)
            {
                ellipse.Fill = Brushes.LightBlue;
            }
            else if (classId == 1)
            {
                ellipse.Fill = Brushes.Red;
            }
            else if (classId == 2)
            {
                ellipse.Fill = Brushes.Green;
            }

            Canvas.SetLeft(ellipse, x * 4);
            Canvas.SetTop(ellipse, resultCanvas.Height - y * 4);

            resultCanvas.Children.Add(ellipse);
        }


        private void Draw(double[,] xy)
        {
            for (int i = 0; i < xy.GetLength(0); i++)
            {
                var ellipse = new Ellipse
                {
                    Width = 4,
                    Height = 4
                };
                if (Math.Abs(xy[i, 2]) <= 0)
                {
                    ellipse.Fill = Brushes.LightBlue;
                }
                else if (Math.Abs(xy[i, 2]) <= 1)
                {
                    ellipse.Fill = Brushes.Red;
                }
                else if (Math.Abs(xy[i, 2]) <= 2)
                {
                    ellipse.Fill = Brushes.Green;
                }

                Canvas.SetLeft(ellipse, xy[i, 0] * 4);
                Canvas.SetTop(ellipse, startCanvas.Height - xy[i, 1] * 4);

                startCanvas.Children.Add(ellipse);
            }

        }

        private void InitPoints(double[,] xy, int numberOfPoints)
        {
            var randomGenerator = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < numberOfPoints; i++)
            {
                var x = randomGenerator.Next(0, 119);
                var y = randomGenerator.Next(0, 119);

                xy[i, 0] = x;
                xy[i, 1] = y;
                xy[i, 2] = 1;

                if (((x - 60) * (x - 60) + (y - 60) * (y - 60)) <= 900)
                {
                    xy[i, 2] = 1;
                }
                else if ( x>=10 && x<=20)
                {
                    xy[i, 2] = 2;
                }
                else
                {
                    xy[i, 2] = 0;
                }

            }
        }
    }
}
