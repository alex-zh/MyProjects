using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;

namespace GUI.Common.RichTextBox.Classes
{
    public class RichTextBoxHelper
    {
        public static FlowDocument LoadText(string text)
        {
            var paragraphMatches = Regex.Matches(text, @"\<p\>(?<text>.+?)\</p", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var paragraphs = new List<String>();

            if (paragraphMatches.Count>0)
            {
                foreach (Match match in paragraphMatches)
                {
                    paragraphs.Add(match.Groups["text"].Value);
                }    
            }
            else
            {
                paragraphs.Add(text);
            }

            var flowDocument = new FlowDocument();

            foreach (var p in paragraphs)
            {
                flowDocument.Blocks.Add(new Paragraph(new Run(p.Trim())));
            }
            return flowDocument;
        }

        public static FlowDocument CleanAndReformat(FlowDocument document)
        {
            var collection = document.FindLogicalChildren<Paragraph>();

            var flowDocument = new FlowDocument();

            foreach (var paragraph in collection)
            {
                var range = new TextRange(paragraph.ElementStart, paragraph.ElementEnd);
                var text = range.Text;

                if (String.IsNullOrEmpty(text) == false)
                {
                    flowDocument.Blocks.Add(new Paragraph(new Run(text.Trim())));    
                }
                
            }

            return flowDocument;
        }
        
        public static string GetXamlString(FlowDocument document)
        {
            var start = document.ContentStart;
            var end = document.ContentEnd;
            var range = new TextRange(start, end);

            using (var stream = new MemoryStream())
            {
                range.Save(stream, DataFormats.Xaml);
                stream.Position = 0;

                using (var r = new StreamReader(stream))
                {
                    return r.ReadToEnd();
                }
            }
        }        
    }
}