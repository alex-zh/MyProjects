using System;
using System.Collections.Generic;
using Common.Classes.General;
using Common.Classes.Robot;

namespace Robot.Core.Providers
{
    public interface ICryptoDataProvider
    {
        event EventHandler<EventArgs> TimeChanged;
        event EventHandler<EventArgs> DataChanged;        
        event EventHandler<EventArgs> LimitedOrdersChanged;

        DateTime GetServerTime();

        Candle GetLastCandle(Securities security);
        IEnumerable<Candle> GetCandles(Securities security, int count);
        
        IEnumerable<RobotTrade> GetTrades();
        IEnumerable<LimitedOrder> GetLimitedOrders();
    }
}