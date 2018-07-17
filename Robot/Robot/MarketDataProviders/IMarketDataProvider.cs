using System;
using System.Collections.Generic;
using Common.Classes.General;
using Common.Robots.Common;

namespace Robot.Core.MarketDataProviders
{
    public interface IMarketDataProvider
    {
        event EventHandler<EventArgs> TimeChanged;
        event EventHandler<EventArgs> DataChanged;        
        event EventHandler<EventArgs> TradesChanged;
        event EventHandler<EventArgs> OrdersChanged;

        DateTime GetServerTime();

        Quote GetLastQuote(string symbol);
        Candle GetLastCandle(string symbol);

        IEnumerable<Candle> GetCandles(string symbol, int count);
        
        IEnumerable<RobotTrade> GetTrades();
        IEnumerable<RobotTrade> GetTrades(string orderId);

        IEnumerable<LimitOrder> GetLimitOrders();
    }
}