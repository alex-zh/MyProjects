using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancialIdeasTester.Classes;

namespace FinancialIdeasTester
{
    class ChangesCalculator
    {
        public void Calculate(LogScaleDetrendedChangesValuator logScaleDetrendedChangesValuator, DataHolder dataHolder)
        {
            dataHolder.Changes = logScaleDetrendedChangesValuator.Valuate(dataHolder.Candles);
        }
    }
}
