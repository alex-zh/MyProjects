namespace Common.Classes.General
{
    public class Quote
    {
        public Quote()
        {
            Ask = new QuoteItem();
            Bid = new QuoteItem();
        }

        public QuoteItem Ask { get; set; }
        public QuoteItem Bid { get; set; }
    }
}