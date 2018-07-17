using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace OpinionAnalyzer.DataLoader
{
    public class ArticleListItem
    {
        public string Url { get; set; }
        public string Author { get; set; }
        public string Company { get; set; }
    }

    public class LoadedArticle
    {
        [Key]
        public int Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        [MaxLength]
        [Column(TypeName = "Ntext")]        
        public string Content { get; set; }

        public string Author { get; set; }
        
        public string Company { get; set; }

        public DateTime PublishDate { get; set; }
    }
}
