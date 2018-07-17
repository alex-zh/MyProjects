using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace UsdRubPredictor.Core
{
    public class Class1
    {

        public static void Test()
        {
            double[,] a =
            {
                {1, 1,1},
                {1, 1,1},
                {1, 1,1}
            };
            double[,] rightSide = { { 100 }, { 100 }, { 100 } };

            var c = Matrix.Solve(a, rightSide, leastSquares: true);

            PrintMatrix(a, "A");
            PrintMatrix(rightSide, "RightSide");
            PrintMatrix(c, "Result");


        }

        private static void PrintMatrix(double[,] c, string name)
        {
            Console.WriteLine("\n");
            Console.WriteLine(name);
            for (int row = 0; row < c.GetLength(0); row++)
            {
                Console.WriteLine("\n");
                for (int column = 0; column < c.GetLength(1); column++)
                    Console.Write(Math.Round(c[row, column], 2) + "  ");
            }
        }
    }
}
