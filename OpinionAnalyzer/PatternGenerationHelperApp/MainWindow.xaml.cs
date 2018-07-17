using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using GUI.Common.RichTextBox.Classes;
using OpinionAnalyzer.Core;
using OpinionAnalyzer.Core.Providers;

namespace PatternGeneratorApp
{
    public partial class MainWindow
    {
        private PatternBuilder _patternBuilder;

        public MainWindow()
        {
            InitializeComponent();
            _patternBuilder = new PatternBuilder(new MacroWordsLoader(Paths.MacroWordsFilePath).MacroWords);
        }

        private void BtnGenerate_OnClick(object sender, RoutedEventArgs e)
        {
            var patternText = tbPattern.Text;
            tbGeneratedPattern.Text = _patternBuilder.Build(patternText);
        }

        private void BtnCheck_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Validate() == false)
                {
                    return;
                }

                rtbText.Document = RichTextBoxHelper.CleanAndReformat(rtbText.Document);

                var regex = tbGeneratedPattern.Text;

                var ranges = RichTextBoxMatchFinder.FindMatches(rtbText.Document, new Regex(regex, RegexOptions.Singleline | RegexOptions.IgnoreCase));

                RichTextBoxMatchFinder.HighlightMatches(ranges, Colors.Red, FontWeights.Bold);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnReset_OnClick(object sender, RoutedEventArgs e)
        {
            rtbText.Document = RichTextBoxHelper.CleanAndReformat(rtbText.Document);
        }


       

        private bool Validate()
        {
            var regexPattern = tbGeneratedPattern.Text;

            if (string.IsNullOrEmpty(regexPattern))
            {
                ShowWarning("Regex field is empty", 1);
                return false;
            }

            return true;
        }

        private void ShowWarning(string text, int displayTime)
        {
            lblWarning.Content = text;
            Task.Run(() =>
            {
                Thread.Sleep(displayTime * 1000);
                lblWarning.Dispatcher.Invoke(() => { lblWarning.Content = String.Empty; });
            });
        }

        private void rtbText_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {

        }

        private void rtbText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }
    }
}
