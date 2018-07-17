using System.Net.Http;

namespace BitMexConnector
{
    public class AuthSubscriptionTopic
    {
        public AuthSubscriptionTopic(string apiKey, string privateKey)
        {
            ApiKey = apiKey;
            Expires = new BitmexExpiresCalculator().Calculate(24*60*60); //24 hours
            Signature = new BitMexSignatureCalculator(privateKey).Calculate(HttpMethod.Get, "/realtime", Expires, "");
        }

        public string ApiKey { get; }
        public string Expires { get;  }
        public string Signature { get; }        
    }
}