using System;
using System.Collections.Generic;
using System.Linq;
using BitMexConnector;
using Common.Classes.General;
using Common.Robots.Common;
using Robot.Core.MarketDataProviders;

namespace BitMex.TradeApplication.Classes
{
    class MarketDataProvider : IMarketDataProvider
    {
        private readonly Connector _connector;

        public event EventHandler<EventArgs> TimeChanged;
        public event EventHandler<EventArgs> DataChanged;
        public event EventHandler<EventArgs> TradesChanged;
        public event EventHandler<EventArgs> OrdersChanged;

        public MarketDataProvider()
        {
            _connector = ConnectorKeeper.GetConnector();
            
            _connector.DataProvider.TradesChanged += DataProvider_TradesChanged; ;
            _connector.DataProvider.OrdersChanged += DataProviderLimitOrdersChanged;
        }

        private void DataProvider_TradesChanged(object sender, EventArgs e)
        {
            TradesChanged?.Invoke(sender, e);
        }

        private void DataProviderLimitOrdersChanged(object sender, EventArgs e)
        {
            OrdersChanged?.Invoke(sender, e);
        }

        public DateTime GetServerTime()
        {
            return _connector.DataProvider.GetLastServerDataTime();
        }

        public Quote GetLastQuote(string symbol)
        {
            var quotes = _connector.DataProvider.GetQuotes(symbol, 1);

            if (quotes.Any())
            {
                return quotes[0];
            }

            return new Quote();
        }

        public Candle GetLastCandle(string symbol)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Candle> GetCandles(string symbol, int count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RobotTrade> GetTrades()
        {
            return _connector.DataProvider.GetTrades().Select(x => x.ToRobotTrade());
        }

        public IEnumerable<RobotTrade> GetTrades(string orderId)
        {
            return _connector.DataProvider.GetTrades()
                                          .Where(x => x.OrderId == orderId)
                                          .Select(x => x.ToRobotTrade());
        }

        public IEnumerable<LimitOrder> GetLimitOrders()
        {
            return _connector.DataProvider.GetOrders().Select(x => x.ToLimitOrder());
        }
    }
}
