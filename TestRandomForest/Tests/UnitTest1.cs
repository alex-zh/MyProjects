using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using dforest = alglib.dforest;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {        
        [TestMethod]
        public void basictest2()
        {
            int pass = 0;
            int passcount = 0;
            double[,] xy = new double[0, 0];
            int npoints = 0;
            int ntrees = 0;
            int i = 0;
            int j = 0;
            double s = 0;
            int info = 0;
            dforest.decisionforest df = new dforest.decisionforest();
            

            double[] x = new double[0];
            double[] y = new double[0];
            dforest.dfreport rep = new dforest.dfreport();
            
            passcount = 1;
            for (pass = 1; pass <= passcount; pass++)
            {

                //
                // select npoints and ntrees
                //
                npoints = 3000;
                ntrees = 50;

                //
                // Prepare task
                //
                xy = new double[npoints - 1 + 1, 1 + 1];
                x = new double[0 + 1];
                y = new double[1 + 1];
                for (i = 0; i <= npoints - 1; i++)
                {
                    xy[i, 0] = 3 * alglib.math.randomreal();
                    if ((double)(xy[i, 0]) <= (double)(1))
                    {
                        xy[i, 1] = 0;
                    }
                    else
                    {
                        if ((double)(xy[i, 0]) <= (double)(2))
                        {
                            if ((double)(alglib.math.randomreal()) < (double)(xy[i, 0] - 1))
                            {
                                xy[i, 1] = 1;
                            }
                            else
                            {
                                xy[i, 1] = 0;
                            }
                        }
                        else
                        {
                            xy[i, 1] = 1;
                        }
                    }
                }

                //
                // Test
                //
                dforest.dfbuildinternal(xy, npoints, 1, 2, ntrees, (int)Math.Round(0.05 * npoints), 1, 0, ref info, df, rep);

                
                bool err = false;
                if (info <= 0)
                {
                    err = true;
                    return;
                }
                x[0] = 0.0;
                while ((double)(x[0]) <= (double)(3.0))
                {
                    dforest.dfprocess(df, x, ref y);

                    //
                    // Test for basic properties
                    //
                    s = 0;
                    for (j = 0; j <= 1; j++)
                    {
                        if ((double)(y[j]) < (double)(0))
                        {
                            err = true;
                            return;
                        }
                        s = s + y[j];
                    }
                    if ((double)(Math.Abs(s - 1)) > (double)(1000 * alglib.math.machineepsilon))
                    {
                        err = true;
                        return;
                    }

                    //
                    // test for good correlation with results
                    //
                    if ((double)(x[0]) < (double)(1))
                    {
                        err = err || (double)(y[0]) < (double)(0.8);
                    }
                    if ((double)(x[0]) >= (double)(1) && (double)(x[0]) <= (double)(2))
                    {
                        err = err || (double)(Math.Abs(y[1] - (x[0] - 1))) > (double)(0.5);
                    }
                    if ((double)(x[0]) > (double)(2))
                    {
                        err = err || (double)(y[1]) < (double)(0.8);
                    }
                    x[0] = x[0] + 0.01;
                }
            }
        }
    }
}
