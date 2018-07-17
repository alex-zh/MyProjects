using System;
using System.Linq;
using HtmlAgilityPack;
using OpinionAnalyzer.DataLoader.Classes;

namespace OpinionAnalyzer.DataLoader.Finam
{
    public class ArticleInfoExtractor
    {
        private string _pageContent;
        private ExtractedResultCleanerAndConverter _cleanerAndConverter;

        public ArticleInfoExtractor(string pageContent)
        {
            _pageContent = pageContent;
            _cleanerAndConverter = new ExtractedResultCleanerAndConverter();
        }
        public ArticleExtractedInfo Extract()
        {
            //var wc = new WebClient();
            //wc.Encoding = Encoding.GetEncoding("windows-1251");

            //var page = wc.DownloadString(
            //    "http://www.finam.ru/analysis/marketnews/stoimost-akciiy-na-mirovyx-fondovyx-rynkax-blizka-k-spravedlivoiy-a-razgovory-o-formirovanii-puzyreiy-bezosnovatelny-20141024-1435/");

            var doc = new HtmlDocument();
            
            doc.LoadHtml(_pageContent);

            String titleHtml, contentHtml, dateTimeHtml;

            GetArticleParts(doc, out titleHtml, out contentHtml, out dateTimeHtml);

            var result = new ArticleExtractedInfo
            {
                Title = _cleanerAndConverter.Clean(titleHtml),
                Content = _cleanerAndConverter.Clean(contentHtml),
                PublishedDate = _cleanerAndConverter.GetDateTime(dateTimeHtml)
            };

            return result;
        }

        private void GetArticleParts(HtmlDocument doc, out string titleHtml, out string contentHtml, out string dateTimeHtml)
        {
            titleHtml = string.Empty;
            contentHtml = string.Empty;
            dateTimeHtml = string.Empty;

            var newsitemNode = doc.DocumentNode.Descendants("div").FirstOrDefault(d => d.Id.StartsWith("newsitem"));

            if (newsitemNode != null)
            {
                var h2Node = newsitemNode.Descendants().FirstOrDefault(n => n.OriginalName.ToLower() == "h2");
                if (h2Node != null)
                {
                    titleHtml = h2Node.InnerHtml;
                }
            }

            var contentDiv = doc.DocumentNode.Descendants("div").
                            FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("handmade"));

            if (contentDiv != null)
            {
                contentHtml = contentDiv.InnerHtml;
            }

            if (contentDiv != null)
            {
                var previousDiv = contentDiv.PreviousSiblingOnlyElement();
                if (previousDiv != null && previousDiv.OriginalName.ToLower() == "div")
                {
                    dateTimeHtml = previousDiv.InnerHtml;                    
                }
            }           
        }
    }
}
