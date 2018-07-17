using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using NDde.Server;
using Robot.Quik2Net.Classes;
using Robot.Quik2Net.XlFormatParser;

namespace Robot.Quik2Net
{
    public class QuikDdeServer : DdeServer
    {
        private readonly ILogger _logger;
        private readonly string _currentTimeTopicName;
        private readonly string _limitedOrdersTopicName;
        private readonly string[] _candlesTopicNames;
        private DateTime _getCurrentTime;
        private bool _isRegistered = false;
        private readonly Dictionary<string, List<Candle>> _candlesDictionary = new Dictionary<string, List<Candle>>();
        private IEnumerable<QuikTrade> _trades = new List<QuikTrade>();
        private IEnumerable<LimitOrder> _limitedOrders = new List<LimitOrder>();
        private string _tradesTopicName;

        public event EventHandler<EventArgs> TimeChanged;
        public event EventHandler<EventArgs> DataChanged;
        public event EventHandler<EventArgs> TradesChanged;
        public event EventHandler<EventArgs> LimitedOrdersChanged;

        public QuikDdeServer(string serverName,
                             string currentTimeTopicName,
                             string tradesTopicName,
                             string limitedOrdersTopicName,
                             IEnumerable<string> candlesTopicNames,
                             ILogger logger)
            : base(serverName)
        {
            _logger = logger;
            _tradesTopicName = tradesTopicName;
            _currentTimeTopicName = currentTimeTopicName;
            _limitedOrdersTopicName = limitedOrdersTopicName;
            _candlesTopicNames = candlesTopicNames.ToArray();

            foreach (var candlesTopicName in _candlesTopicNames)
            {
                _candlesDictionary[candlesTopicName] = new List<Candle>();
            }

            Register();
        }

        public new void Register()
        {
            try
            {
                if (_isRegistered == false)
                {
                    _isRegistered = true;

                    base.Register();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogMessageTypes.Error);
            }
        }

        public DateTime GetServerTime()
        {
            return _getCurrentTime;
        }

        public IEnumerable<QuikTrade> GetTrades()
        {
            return _trades;
        }

        public IEnumerable<LimitOrder> GetLimitedOrders()
        {
            return _limitedOrders;
        }

        public IEnumerable<Candle> GetCandles(string securityName)
        {
            return _candlesDictionary[securityName];
        }

        /// <summary>
        /// Этот метод будет вызываться очень часто, 
        /// поэтому блокировать _candlesDictionary нехорошо
        /// </summary>
        protected override PokeResult OnPoke(DdeConversation conversation, string item, byte[] data, int format)
        {
            var topicName = conversation.Topic;
            if (topicName == _currentTimeTopicName)
            {
                _getCurrentTime = ParseServerTime(data);

                if (TimeChanged != null)
                {
                    TimeChanged(this, EventArgs.Empty);
                }
            }
            else if (topicName == _tradesTopicName)
            {
                var newTrades = ParseTrades(data);

                if (newTrades.SequenceEqual(_trades) == false)
                {
                    _trades = newTrades;

                    if (TradesChanged != null)
                    {
                        TradesChanged(this, EventArgs.Empty);
                    }
                }


            }
            else if (topicName == _limitedOrdersTopicName)
            {
                var newLimitedOrders = ParseLimitedOrders(data);

                if (newLimitedOrders.SequenceEqual(_limitedOrders) == false)
                {
                    _limitedOrders = newLimitedOrders;

                    if (LimitedOrdersChanged != null)
                    {
                        LimitedOrdersChanged(this, EventArgs.Empty);
                    }
                }

            }
            else if (_candlesTopicNames.Contains(topicName))
            {
                var oldCandles = _candlesDictionary[conversation.Topic];

                var newCandles = ParseCandles(data);

                if (newCandles.SequenceEqual(oldCandles) == false)
                {
                    _candlesDictionary[topicName] = newCandles;

                    if (DataChanged != null)
                    {
                        DataChanged(this, EventArgs.Empty);
                    }
                }

            }

            return PokeResult.Processed;
        }

        private List<LimitOrder> ParseLimitedOrders(byte[] data)
        {
            try
            {
                return new DdeDataParser().ParseLimitedOrders(data);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogMessageTypes.Error);
            }

            return new List<LimitOrder>();
        }

        private List<QuikTrade> ParseTrades(byte[] data)
        {
            try
            {
                return new DdeDataParser().ParseTrades(data);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogMessageTypes.Error);
            }

            return new List<QuikTrade>();

        }

        private DateTime ParseServerTime(byte[] data)
        {
            try
            {
                return new DdeDataParser().ParseServerTime(data);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogMessageTypes.Error);
            }

            return DateTime.MinValue;
        }

        private List<Candle> ParseCandles(byte[] data)
        {
            try
            {
                return new DdeDataParser().ParseCandles(data);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, LogMessageTypes.Error);
            }

            return new List<Candle>();
        }
    }
}
