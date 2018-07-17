using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GUI.Common.RichTextBox.Classes;
using OpinionAnalyzer.Core;
using OpinionAnalyzer.Core.Classes;
using OpinionAnalyzer.DataLoader;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;

namespace OpinionAnalyzer.Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEnumerable<LoadedArticle> _articlesList;

        public MainWindow()
        {
            InitializeComponent();
            CalendarFirstDate.SelectedDate = DateTime.Now;

            BeforeOpeningPatternsKeeper.LoadPatterns();
        }

        public LoadedArticle SelectedArticle
        {
            get;
            set;
        }


        private void Load_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private async void Load()
        {
            if (Validate() == false)
            {
                return;
            }

            grLoading.Visibility = Visibility.Visible;
            grMain.IsEnabled = false;

            var selectedDate = CalendarFirstDate.SelectedDate.Value;
            var loader = new ArticleFromFileLoader(tbPath.Text);

            ctrAnimation.AnimationText = "Loading ...";
            await Task.Run(() =>
             {
                 _articlesList = loader.LoadForDate(selectedDate);
                 _articlesList = _articlesList.OrderBy(x => x.PublishDate).Where(x => x.PublishDate.Hour < 12);
             });


            lvLoadedArticles.ItemsSource = _articlesList;

            grLoading.Visibility = Visibility.Hidden;
            grMain.IsEnabled = true;

            if (_articlesList.Any())
            {
                lvLoadedArticles.SelectedIndex = 0;
                LoadArticleContent(_articlesList.First());
            }
            else
            {
                ResetArticleContent();
            }
        }

        private void ResetArticleContent()
        {
            rtbArticleContent.Document = RichTextBoxHelper.LoadText(String.Empty);
        }


        private bool Validate()
        {
            if (CalendarFirstDate.SelectedDate.HasValue == false)
            {
                MessageBox.Show("First date is not selected!");
                return false;
            }

            return true;
        }
        private void CalendarFirstDate_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            if (Mouse.Captured is Calendar || Mouse.Captured is System.Windows.Controls.Primitives.CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;

            this.Width = workingRectangle.Width * 0.75;
            this.Height = workingRectangle.Height * 0.75;
        }

        private void LvLoadedArticles_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as LoadedArticle;
            if (item != null)
            {
                LoadArticleContent(item);
            }
        }

        private async void LoadArticleContent(LoadedArticle item)
        {
            grLoading.Visibility = Visibility.Visible;
            grMain.IsEnabled = false;

            SelectedArticle = item;
            SetDirection(Trend.Unknown);
            SetWeight(int.MinValue);
            rtbArticleContent.Document = RichTextBoxHelper.LoadText(item.Content);
            var needFindMatch = cbCheckAutomatically.IsChecked.HasValue && cbCheckAutomatically.IsChecked.Value;

            Pattern pattern = null;

            if (needFindMatch)
            {
                ctrAnimation.AnimationText = "Searching for patterns ...";
                await Task.Run(() =>
                {
                    pattern = FindMatch();
                });
            }

            if (pattern != null)
            {
                HighlightMatch(pattern.Regex, pattern.Trend);
                tbRegexPattern.Text = pattern.RawRegexText;
                SetDirection(pattern.Trend);
                SetWeight(pattern.Weight);                
            }

            if (cbFindMarket.IsChecked.Value)
            {
                HighlightMatch(new Regex(BeforeOpeningPatternsKeeper.Market), Colors.Blue, true);                    
            }

            if (cbFindExpect.IsChecked.Value)
            {
                HighlightMatch(new Regex(BeforeOpeningPatternsKeeper.Expect), Colors.DarkOrange, true);
            }

            grLoading.Visibility = Visibility.Hidden;
            grMain.IsEnabled = true;
        }

        private void SetWeight(int weight)
        {
            tbWeight.Text = weight == int.MinValue ? "Unknown" : weight.ToString();
        }

        private void SetDirection(Trend trend)
        {
            switch (trend)
            {
                case Trend.Up:
                    imgDirection.Source = new BitmapImage(new Uri("Icons/Raise.png", UriKind.Relative));
                    break;
                case Trend.Down:
                    imgDirection.Source = new BitmapImage(new Uri("Icons/Fall.png", UriKind.Relative));
                    break;
                case Trend.Neutral:
                    imgDirection.Source = new BitmapImage(new Uri("Icons/Forward.png", UriKind.Relative));
                    break;
                default:
                    imgDirection.Source = new BitmapImage(new Uri("Icons/Question.png", UriKind.Relative));
                    break;
            }
        }

        private void btnFindMatch_Click(object sender, RoutedEventArgs e)
        {
           // FindMatch();
        }

        private Pattern FindMatch()
        {
            foreach (var pattern in BeforeOpeningPatternsKeeper.Patterns)
            {
                if (pattern.Regex.IsMatch(SelectedArticle.Content))
                {
                    return pattern;
                }
            }

            return null;
        }
        

        private void HighlightMatch(Regex regex, Trend trend)
        {
            var ranges = RichTextBoxMatchFinder.FindMatches(rtbArticleContent.Document, regex);

            var color = GetColorForTrend(trend);

            RichTextBoxMatchFinder.HighlightMatches(ranges, color, FontWeights.ExtraBold);
        }

        private static Color GetColorForTrend(Trend trend)
        {
            var color = Colors.Red;
            if (trend == Trend.Up)
            {
                color = Colors.Green;
            }
            else if (trend == Trend.Neutral)
            {
                color = Colors.Blue;
            }
            return color;
        }

        private void HighlightMatch(Regex regex, Color color, bool isBold)
        {
            var ranges = RichTextBoxMatchFinder.FindMatches(rtbArticleContent.Document, regex);

            RichTextBoxMatchFinder.HighlightMatches(ranges, color, isBold? FontWeights.Bold : FontWeights.Normal);            
        }

        private void CalendarFirstDate_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Load();
        }
    }
}
