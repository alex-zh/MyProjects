using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OpinionAnalyzer.UnitTests.Classes
{    
    public class TestArticle : IArticle
    {
        [XmlElement("Id")]
        public int Id { get; set; }
        [XmlElement("Title")]
        public string Title { get; set; }
        [XmlElement("Text")]
        public string Text { get; set; }
        [XmlElement("NegativeText")]
        [DefaultValue("")]
        public string NegativeText { get; set; }        
    }

    [Serializable()]
    [XmlRoot("Items")]
    public class TestArticleCollection
    {
        [XmlElement("Item")]
        public TestArticle[] Articles { get; set; }
    }
}
