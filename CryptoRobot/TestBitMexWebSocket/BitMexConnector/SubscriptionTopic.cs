namespace BitMexConnector
{
    public class SubscriptionTopic
    {
        public string Table { get; set; }
        public string Symbol { get; set; }

        public override string ToString()
        {
            return $"{Table}:{Symbol}";
        }
    }
}