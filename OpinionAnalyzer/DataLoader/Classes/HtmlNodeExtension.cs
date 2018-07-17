using HtmlAgilityPack;

namespace OpinionAnalyzer.DataLoader.Classes
{
    public static class HtmlNodeExtension
    {
        public static HtmlNode PreviousSiblingOnlyElement(this HtmlNode node)
        {
            var previousSibling = node.PreviousSibling;
            if (previousSibling != null && previousSibling.NodeType != HtmlNodeType.Element)
            {
                return previousSibling.PreviousSiblingOnlyElement();
            }

            return previousSibling;
        }
    }
}
