using System;
using System.Collections.Generic;
using Common.Classes.General;
using Common.Classes.Robot;
using Common.Robots.Common;

namespace Robot.Strategies
{
    public interface IStrategyAgent
    {
        void Update(IEnumerable<Candle> series);
        void Update(IEnumerable<Candle> firstSeries, IEnumerable<Candle> secondSeries);

        void UpdateLongSeries(IEnumerable<Candle> series);
        void UpdateLongSeries(IEnumerable<Candle> firstSeries, IEnumerable<Candle> secondSeries);

        bool ShouldOpen(AgentContext context, out AgentActionParams orderParams);

        bool ShouldClose(AgentContext context, out AgentActionParams orderParams);

        bool ShouldTakeProfit(AgentContext context, out AgentActionParams orderParams);

        bool ShouldSetStopLoss(AgentContext context, out AgentActionParams orderParams);

        bool ShouldAddToPosition(AgentContext context, out AgentActionParams orderParams);
    }
}