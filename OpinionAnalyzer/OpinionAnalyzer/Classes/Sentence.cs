namespace OpinionAnalyzer.Core.Classes
{
    /// <summary>
    /// Represents single article sentence
    /// </summary>
    public class Sentence
    {
        public Sentence(string s)
        {
            Text = s;
        }

        public string Text { get; private set; }
    }
}