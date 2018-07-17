using System.Xml.Serialization;

namespace OpinionAnalyzer.Core.Classes
{
    [XmlRoot("Items")]
    public class MacroWordsCollection
    {
        [XmlElement("Item")]
        public MacroWord[] MacroWords { get; set; }
    }

    public class MacroWord
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
