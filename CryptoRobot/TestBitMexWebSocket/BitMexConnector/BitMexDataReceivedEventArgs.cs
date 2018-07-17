using Newtonsoft.Json.Linq;

namespace BitMexConnector
{
    public class BitMexDataReceivedEventArgs
    {
        public JObject Data { get; set; }
    }
}