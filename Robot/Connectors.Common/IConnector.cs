using System;
using System.Collections.Generic;
using Common.Classes.General;

namespace Connector.Common
{
    public interface IConnector
    {
        IEnumerable<string> GetSymbols();
        IEnumerable<Candle> GetCandles(string symbol, string timeframe, int limit, DateTime endDate);
        IEnumerable<Candle> GetCandles(string symbol, string timeframe, DateTime start, DateTime end);
    }
}
