using System;
using System.Collections.Generic;
using BitMexConnector.Tables;
using BitMexConnector.Tables.Classes;
using Common.Classes.General;

namespace BitMexConnector
{
    public interface IDataProvider
    {
        event EventHandler<EventArgs> TradesChanged;
        event EventHandler<EventArgs> OrdersChanged;

        DateTime GetLastServerDataTime();

        OrderBook GetOrderBook(string symbol);

        IEnumerable<BitMexLimitOrder> GetOrders();

        IEnumerable<BitMexTrade> GetTrades();

        List<Quote> GetQuotes(string symbol, int levelsNumber);
    }
}