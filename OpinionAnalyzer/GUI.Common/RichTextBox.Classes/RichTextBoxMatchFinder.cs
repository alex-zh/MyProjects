using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Common.RichTextBox.Classes
{
    public class RichTextBoxMatchFinder
    {
        public static List<HighlightRange> FindMatches(FlowDocument document, Regex regex)
        {
            var result = new List<HighlightRange>();
            
            var start = document.ContentStart;
            var end = document.ContentEnd;
            
            while (start != null && start.CompareTo(end) < 0)
            {                
                if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {                    
                    var text = start.GetTextInRun(LogicalDirection.Forward);

                    var matches = regex.Matches(text);

                    foreach (Match match in matches)
                    {
                        result.Add(new HighlightRange()
                        {
                            StartPointer = start.GetPositionAtOffset(match.Index, LogicalDirection.Forward),
                            EndPointer = start.GetPositionAtOffset(match.Index + match.Length, LogicalDirection.Backward),            
                        });
                    }

                }
                start = start.GetNextContextPosition(LogicalDirection.Forward);
            }

            return result;
        }

        public static void HighlightMatches(IEnumerable<HighlightRange> ranges, Color color, FontWeight weight)
        {
            foreach (var range in ranges)
            {
                var textRange = new TextRange(range.StartPointer, range.EndPointer);
                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));

                textRange.ApplyPropertyValue(TextElement.FontWeightProperty, weight);
                
            }
        }        
    }
}