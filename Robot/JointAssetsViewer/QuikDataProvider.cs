using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;
using Common.Classes.Robot;
using Robot.Quik2Net;

namespace JointAssetsViewer
{
    public class QuikDataProvider
    {
        private readonly QuikDdeServer _ddeServer;

        public QuikDataProvider()
        {
            _ddeServer = DdeServerKeeper.Get();
        }

        public event EventHandler<EventArgs> TimeChanged
        {
            add { _ddeServer.TimeChanged += value; }
            remove { _ddeServer.TimeChanged -= value; }
        }        

        public event EventHandler<EventArgs> DataChanged
        {
            add { _ddeServer.DataChanged += value; }
            remove { _ddeServer.DataChanged -= value; }
        }

        public DateTime GetServerTime()
        {
            return _ddeServer.GetServerTime();
        }

        public Candle GetLastCandle(Securities security)
        {
            var rawCandles = GetCandles(security, 1);

            return rawCandles.LastOrDefault();
        }

        public IEnumerable<Candle> GetCandles(Securities security, int requestedCount)
        {
            var rawCandles = _ddeServer.GetCandles(JointAssertsSettings.GetSecurityName(security)).ToList();

            var skipCount = Math.Max(0, rawCandles.Count - requestedCount);

            rawCandles = rawCandles.Skip(skipCount).ToList();

            return rawCandles;
        }        
    }
}