using System;
using System.Windows.Controls;
using QuikConnector.Common;

namespace QuikConnector.Classes
{
    public class Logger: ILogger
    {
        private readonly TextBox _textBox;

        public Logger(TextBox textBox)
        {
            _textBox = textBox;
        }

        public void Log(string text)
        {
            _textBox.Dispatcher.Invoke(() =>
            {
                _textBox.AppendText(Environment.NewLine);
                var time = DateTime.Now.TimeOfDay;
                _textBox.AppendText(String.Format("{0}:{1}{2}: {3}", time.ToString("hh"), time.ToString("mm"), time.ToString("ss"), text));
            });
        }
    }
}
