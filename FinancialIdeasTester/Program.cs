using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Classes.Regression;
using FinancialIdeasTester.Classes;
using FinancialIdeasTester.Rules;
using FinancialIdeasTester.Rules.BrentToUsdRubRatio;

namespace FinancialIdeasTester
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //TestLinearRegression tr = new TestLinearRegression();

            //return;
            var vtbDataFilePath = @"AppData\ICE.BRN_090803_140803.txt";
            var sbrDataFilePath = @"AppData\SBER_100526_101231.txt";

            var vtbDataHolder = new DataHolder();
            var sbrDataHolder = new DataHolder();

            DataLoader.LoadData(vtbDataFilePath, vtbDataHolder);
            DataLoader.LoadData(sbrDataFilePath, sbrDataHolder);

            LinearDetrender.Detrend(vtbDataHolder);
            LinearDetrender.Detrend(sbrDataHolder);

            var dataWriter = new DetrendedValueResultGenerator();
            
            dataWriter.WriteData(sbrDataHolder);
        }
    }

    public class LinearDetrender
    {
        public static void Detrend(DataHolder dataHolder)
        {
            var length = dataHolder.Candles.Count;

            double firstLogValue = Math.Log(dataHolder.Candles[0].Close);
            double lastLogValue = Math.Log(dataHolder.Candles[length - 1].Close);

            double difference = (lastLogValue - firstLogValue)/length;

            for (int i = 0; i < length; i++)
            {
                dataHolder.DetrenedValues.Add(new DetrendedValue()
                {
                    Date = dataHolder.Candles[i].Date,
                    Value = Math.Exp(Math.Log(dataHolder.Candles[i].Close) - i * difference)
                });
            }
        }
    }
}
