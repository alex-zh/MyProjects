using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Common;
using Common.Classes;
using Common.Classes.General;
using Common.Classes.Robot;
using Common.Robots.Common;
using Robot.Strategies;

namespace Robot.UnitTests.UsdStrategyAgetStubs
{    
    public class StrategyAgentStub : IStrategyAgent
    {
        private readonly StrategyScript _strategyScript;

        public StrategyAgentStub(string strategyScriptInXmlFormat)
        {
            var xs = new XmlSerializer(typeof(StrategyScript));
            _strategyScript = (StrategyScript)xs.Deserialize(new StringReader(strategyScriptInXmlFormat));
        }

        public int Iteration { get; set; }

        public void Update(IEnumerable<Candle> candles)
        {
            //Do nothing
        }

        public void Update(IEnumerable<Candle> firstSeries, IEnumerable<Candle> secondSeries)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateLongSeries(IEnumerable<Candle> series)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateLongSeries(IEnumerable<Candle> firstSeries, IEnumerable<Candle> secondSeries)
        {
            throw new System.NotImplementedException();
        }

        public DateTime GetLongSeriesLastUpdateTime()
        {
            return DateTime.MaxValue;
        }

        public void UpdateLongTermParameters(DistributionParameters averageUpDeviation, DistributionParameters averageDownDeviation,
            DistributionParameters atrParameters, List<IntersectionSegment> lastIntersectionSegment)
        {
            //
        }

        public void UpdateLongTermParameters(DistributionParameters averageUpDeviation, DistributionParameters averageDownDeviation,
            List<IntersectionSegment> lastIntersectionSegment)
        {
            //
        }

        public void UpdateLongTermParameters(DistributionParameters averageUpDeviation, DistributionParameters averageDownDeviation,
            IntersectionSegment lastIntersectionSegment)
        {
            //Do nothing
        }

        
        public bool ShouldOpen(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams =  _strategyScript.Iterations.Single(x => x.Index == Iteration).ShouldOpen;

            return orderParams != null;
        }

        public bool ShouldClose(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = _strategyScript.Iterations.Single(x => x.Index == Iteration).ShouldClose;

            return orderParams != null;
        }

        public bool ShouldTakeProfit(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = _strategyScript.Iterations.Single(x => x.Index == Iteration).ShouldTakeProfit;

            return orderParams != null;
        }

        public bool ShouldSetStopLoss(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = null;

            return false;
        }

        public bool ShouldAddToPosition(AgentContext context, out AgentActionParams orderParams)
        {
            orderParams = _strategyScript.Iterations.Single(x => x.Index == Iteration).ShouldAddToPosition;

            return orderParams != null;
        }
    }
}