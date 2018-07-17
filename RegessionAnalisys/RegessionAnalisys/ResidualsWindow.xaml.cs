using System.Windows;

namespace RegessionAnalisys
{    
    public partial class ResidualsWindow
    {
        public ResidualsWindow()
        {
            InitializeComponent();
        }

        public Scatterplot Scatterplot
        {
            get
            {
                return residualsScatterplot; 
                
            }
        }
        
    }
}
