using BitMexConnector.Parsers;
using BitMexConnector.Parsers.OrderBookL2;
using Newtonsoft.Json.Linq;

namespace BitMexConnector
{
    public class BitMexTableData
    {
        public ActionNames ActionName { get; set; }
        public JArray Data { get; set; }
        public string ErrorText { get; set; }
        public bool HasErrors { get; set; }
    }
}