using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using Common.Classes;
using Common.Classes.Builders;
using Common.Classes.General;

namespace Common.Finders
{
    /// <summary>
    /// Finds specified structures
    /// For example { 1% up in 5-8 candles, 2% down in next 5-8 candles }
    /// </summary>
    public class RecursiveStructureFinder
    {
        private readonly List<Candle> _allCandles;
        private readonly List<StructureInfo> _structures;
        private readonly bool _useLogScale;

        public RecursiveStructureFinder(List<Candle> allCandles, StructureInfo structureInfo):this(allCandles, structureInfo, false, false)
        {
            
        }

        public RecursiveStructureFinder(List<Candle> allCandles, StructureInfo structureInfo, bool useLogScale, bool varyStructures)
        {
            _allCandles = useLogScale ? allCandles.Log().ToList() : allCandles;

            _structures = varyStructures ? 
                          VaryStructure(structureInfo) : new List<StructureInfo> {structureInfo};

            _useLogScale = useLogScale;
        }

        private List<StructureInfo> VaryStructure(StructureInfo structureInfo)
        {
            var varyCoefficients = new[] { 0.5, 0.75, 1, 1.25, 1.5, 2, 2.5, 3, 3.5, 4 };

            var result = new List<StructureInfo>();

            foreach (var coefficient in varyCoefficients)
            {
                var newStructure = new StructureInfo();

                foreach (var item in structureInfo.Items)
                {
                    var newItem = item.Clone();
                    newItem.Change = newItem.Change * coefficient;
                    newItem.Period = (int)(newItem.Period * coefficient);
                    //newItem.Atr = newItem.Atr * coefficient;

                    newStructure.Items.Add(newItem);
                }

                result.Add(newStructure);
            }

            return result;
        }

        public List<Structure> FindAll()
        {
            var result = new List<Structure>();

            foreach (var structure in _structures)
            {                
                result.AddRange(FindAll(structure));
            }

            if (result.Any() == false) return result;

            var notIntersectedItems = new List<Structure>();

            notIntersectedItems.Add(result[0]);
            for (int i = 1; i < result.Count; i++)
            {
                var item = result[i];

                if (notIntersectedItems.All(x => (item.StartIndex > x.EndIndex || item.EndIndex < x.StartIndex)))
                {
                    notIntersectedItems.Add(item);
                }
            }

            return notIntersectedItems;
        }

        private List<Structure> FindAll(StructureInfo structureInfo)
        {
            var result = new List<Structure>();


            for (int i = 50; i < _allCandles.Count; i++)
            {
                var atr = new SimpleAtr(_allCandles.TakeRange(Math.Max(0, i - 50), i)).Calculate();

                var foundStructure = FindStructure(i, structureInfo, atr);
                
                if(foundStructure.Any())
                    result.Add(foundStructure);
            }

            return result;
        }

        private Structure FindStructure(int startIndex, StructureInfo structureInfo, double atr)
        {            
            var structureItem = structureInfo.Items.First();

            var endRangeStartIndex = startIndex + (int)(structureItem.Period * 0.65);
            var endRangeEndIndex = startIndex + (int)(structureItem.Period * 1.35);

            endRangeEndIndex = Math.Min(_allCandles.Count - 1, endRangeEndIndex);

            for (int endIndex = endRangeStartIndex; endIndex <= endRangeEndIndex; endIndex++)
            {
                var foundStructureItem = FindStructureItem(startIndex, endIndex, structureItem, atr);

                if (foundStructureItem != null)
                {
                    if (structureInfo.Items.Count > 1) // one is already found
                    {
                        var foundItems = FindStructure(endIndex + 1, structureInfo.Pop(), atr);

                        if (foundItems.Any())
                        {
                            foundItems.Insert(0, foundStructureItem);
                            return foundItems;
                        }                                                
                    }
                    else
                    {
                        return new Structure { foundStructureItem };
                    }
                }
            }

            return new Structure();
        }

        private StructureItem FindStructureItem(int startIndex, int endIndex, StructureInfoItem structureInfoItem, double atr)
        {
            var startCandle = _allCandles[startIndex];
            var endCandle = _allCandles[endIndex];

            var change = CalculateChange(startCandle, endCandle, _useLogScale);

            if (IsChangeCorrect(structureInfoItem, change) && 
                IsDeviationCorrect(startIndex, endIndex, structureInfoItem, atr))
            {
                return new StructureItem() {StartIndex =  startIndex, EndIndex = endIndex};
            }

            return null;
        }

        private bool IsDeviationCorrect(int startIndex, int endIndex, StructureInfoItem structureInfoItem, double atr)
        {
            var range = _allCandles.TakeRange(startIndex, endIndex).ToList();
            var stepChange = structureInfoItem.Change / (endIndex - startIndex + 1);

            var deviations = range.Select((candle, index) =>
            {
                var trendValue = ((index+1) * stepChange + 1) * range.First().Open;
                return candle.Close - trendValue;
            }).ToList();

            var maxDeviation =  deviations.Select(Math.Abs).Max();


            if (maxDeviation > 1.5 * atr)
            {
               return false;
            }

            return true;
        }

        private static bool IsChangeCorrect(StructureInfoItem structureInfoItem, double change)
        {
            return change * structureInfoItem.Change >= 0 &&
                   Math.Abs(change) >= Math.Abs(structureInfoItem.Change * 0.8) && Math.Abs(change) <= Math.Abs(structureInfoItem.Change * 1.2);
        }

        public static double CalculateChange(Candle startCandle, Candle endCandle, bool useLogScale)
        {
            if (useLogScale)
            {
                return endCandle.Close - startCandle.Open;
            }

            return (endCandle.Close - startCandle.Open) / startCandle.Open;
        }
    }
}
