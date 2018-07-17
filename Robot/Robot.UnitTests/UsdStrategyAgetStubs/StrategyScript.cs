using System.Collections.Generic;
using System.Xml.Serialization;
using Common.Classes.Robot;
using Common.Robots.Common;
using Robot.Strategies;

namespace Robot.UnitTests.UsdStrategyAgetStubs
{
    [XmlRoot("Iterations")]
    public class StrategyScript
    {
        [XmlElement("Iteration")]
        public List<StrategyIteration> Iterations { get; set; }
    }

    [XmlType("Iteration")]
    public class StrategyIteration
    {
        [XmlAttribute("index")]
        public int Index { get; set; }
        public AgentActionParams ShouldOpen { get; set; }
        public AgentActionParams ShouldClose { get; set; }

        public AgentActionParams ShouldTakeProfit { get; set; }
        public AgentActionParams ShouldAddToPosition { get; set; }
    }
}