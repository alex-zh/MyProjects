using System.ComponentModel;
using System.Xml.Serialization;

namespace OpinionAnalyzer.Core.Classes
{
    [XmlRoot("Items")]
    public class OpinionPatternCollection
    {
        [XmlElement("Item")]
        public RawPattern[] RawPatterns { get; set; }
    }
   
    public class RawPattern
    {        
        [XmlIgnore]
        public Trend Trend { get; set; }

        [DefaultValue(0)]         
        public int Weight { get; set; }

        [XmlElement("Regex")]
        public string RegexText { get; set; }

        public int  Priority { get; set; }

        [XmlElement("NegativeCheck")]
        public string NegativeFormCheck { get; set; }
    }
}