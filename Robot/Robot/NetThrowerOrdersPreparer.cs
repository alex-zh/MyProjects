using System;
using System.Collections.Generic;
using Common.Robots.Common;
using Common.StrategiesOptions;
using Robot.Core.MarketDataProviders;

namespace Robot.Core
{
    public class NetThrowerOrdersPreparer
    {
        private readonly IMarketDataProvider _marketDataProvider;
        private readonly NetThrowerOptions _options;
        private readonly string _symbol;
        private readonly IRobotStateGetter _robotState;

        public NetThrowerOrdersPreparer(string symbol, IRobotStateGetter robotState, IMarketDataProvider marketDataProvider, NetThrowerOptions options)
        {
            _symbol = symbol;
            _robotState = robotState;
            _marketDataProvider = marketDataProvider;
            _options = options;
        }

        public List<OrderParams> GetOrders()
        {
            var result = new List<OrderParams>();

            result.AddRange(GetBuyOrders());
            result.AddRange(GetSellOrders());

            return result;
        }

        public List<OrderParams> GetBuyOrders()
        {
            var result = new List<OrderParams>();

            var lastQuote = _marketDataProvider.GetLastQuote(_symbol);

            if (lastQuote == null)
                return result;
            

            var lastPrice = lastQuote.Bid.Price;

            var currentSize = _robotState.Size;
            var maxOrdersSize = Math.Min(_options.MaxPositionSize, Math.Max(0, _options.MaxPositionSize - currentSize));
            //var maxOrdersSize = _options.MaxPositionSize;
            var maxOrdersCount = maxOrdersSize > 0 ? (int)Math.Log(maxOrdersSize/_options.PositionSize + 1, 2) : 0; // double each next order size
                                                                                              // 2^n - 1 = 1 + 2 + 4 + 8 ...

            // var maxNeutralOrdersCount = (int) Math.Log(_options.MaxPositionSize + 1, 2);
            //var maxOrdersSize =  _options.MaxPositionSize - currentSize;
            //var maxOrdersCount = maxOrdersSize > 0 ? (int)Math.Log(maxOrdersSize+1, 2)  : 0; // double each next order size

            //Spread current negative position among first 4 orders
            //var currentPositionOrderAdditionalSize = currentSize < 0 ? (int)(Math.Abs(currentSize) / Math.Pow(2, Math.Min(maxOrdersCount, 4))) : 0; // double each next order size

            // var usedAdditionalSize = 0;

            if (currentSize < 0)
            {
                var order = new OrderParams()
                {
                    Side = OrderSides.Buy,
                    Size = Math.Abs(currentSize),
                    Price = lastPrice * (1 - _options.PercentOrdersOffset),
                };
                result.Add(order);
            }

            var percentOrdersOffset = 4*_options.PercentOrdersOffset;
            for (int i = 0; i < maxOrdersCount; i++)
            {
                //var size = (i == 0) && currentSize < 0
                //    ? Math.Abs(currentSize)
                //    : ((int) Math.Pow(2, i))*_options.PositionSize;

                var size =  ((int)Math.Pow(2, i)) * _options.PositionSize;

                //if (currentPositionOrderAdditionalSize == 0 && currentSize < 0 && Math.Abs(currentSize) - usedAdditionalSize != 0)
                //{
                //    size += Math.Abs(currentSize) - usedAdditionalSize;
                //    usedAdditionalSize += Math.Abs(currentSize) - usedAdditionalSize;
                //}

                //if (i < Math.Min(maxOrdersCount-1, 3) && currentSize < 0)
                //{
                //    usedAdditionalSize+= currentPositionOrderAdditionalSize * (int)Math.Pow(2, i);
                //    size += currentPositionOrderAdditionalSize* (int)Math.Pow(2, i);
                //}
                //else if(i == Math.Min(maxOrdersCount - 1, 3) && currentSize < 0)
                //{
                //    size += Math.Abs(currentSize) - usedAdditionalSize;
                //}

                var order = new OrderParams()
                {
                    Side = OrderSides.Buy,
                    Size = size,
                    Price = lastPrice * (1 - (i + 1) * percentOrdersOffset),
                };
                result.Add(order);
            }

            return result;

        }

        public List<OrderParams> GetSellOrders()
        {
            var result = new List<OrderParams>();

            var lastQuote = _marketDataProvider.GetLastQuote(_symbol);

            if (lastQuote == null)
                return result;


            var lastPrice = lastQuote.Bid.Price;
            var currentSize = _robotState.Size;

            var minOrdersSize = Math.Abs(Math.Max(_options.MinPositionSize, Math.Min(0, _options.MinPositionSize - currentSize)));
            //var minOrdersSize = Math.Abs(_options.MinPositionSize);
            var maxOrdersCount = minOrdersSize > 0 ? (int)Math.Log(minOrdersSize/ _options.PositionSize + 1, 2) : 0; // double each next order size
            // 2^n - 1 = 1 + 2 + 4 + 8 ...

            if (currentSize > 0)
            {
                var order = new OrderParams()
                {
                    Side = OrderSides.Sell,
                    Size = Math.Abs(currentSize),
                    Price = lastPrice * (1 + _options.PercentOrdersOffset),
                };
                result.Add(order);
            }

            var percentOrdersOffset = 4 * _options.PercentOrdersOffset;

            for (int i = 0; i < maxOrdersCount; i++)
            {
                //var size = (i == 0) && currentSize > 0
                //    ? Math.Abs(currentSize)
                //    : ((int)Math.Pow(2, i)) * _options.PositionSize;

                var size = ((int)Math.Pow(2, i)) * _options.PositionSize;

                var order = new OrderParams()
                {
                    Side = OrderSides.Sell,
                    Size = size,
                    Price = lastPrice * (1 + (i + 1) * percentOrdersOffset),
                };
                result.Add(order);
            }

            return result;
        }
    }
}