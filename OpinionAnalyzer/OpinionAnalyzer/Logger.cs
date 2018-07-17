using System.Windows.Forms;

namespace OpinionAnalyzer
{
    public class Logger
    {
        public static void LogWarning(string text)
        {
            MessageBox.Show(text, "Logger", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}