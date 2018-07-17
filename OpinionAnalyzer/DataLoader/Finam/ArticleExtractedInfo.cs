using System;

namespace OpinionAnalyzer.DataLoader.Finam
{
    public class ArticleExtractedInfo
    {
        public ArticleExtractedInfo()
        {
            Title = String.Empty;
            Content = String.Empty;            
        }

        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? PublishedDate { get; set; }
    }
}