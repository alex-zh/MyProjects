using System;
using System.Collections.Generic;
using System.Linq;
using NDde.Server;
using QuikConnector.Common.Classes;
using QuikConnector.Common.XlFormatParser;

namespace QuikConnector.Common
{
    public class QuikDdeServer : DdeServer
    {
        private readonly ILogger _logger;
        private List<Candle> _candles = new List<Candle> ();

        public event EventHandler<EventArgs> DataChanged;

        public QuikDdeServer(string service, ILogger logger) : base(service)
        {
            _logger = logger;            
        }

        public IEnumerable<Candle> Candles
        {
            get { return _candles; }
        }        
        
        protected override PokeResult OnPoke(DdeConversation conversation, string item, byte[] data, int format)
        {   
            var newCandles = new DdeCandlesParser().Parse(data);

            if (newCandles.SequenceEqual(_candles) == false)
            {
                _candles = newCandles;

                if (DataChanged != null)
                {
                    DataChanged(this, EventArgs.Empty);
                }
            }
            
            return PokeResult.Processed;
        }        
    }    
}
