using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinancialIdeasTester.Classes
{
    public class Logger
    {
        public static void Warn(string message)
        {
#if DEBUG
            MessageBox.Show(message);
#endif
      
        }
    }
}
