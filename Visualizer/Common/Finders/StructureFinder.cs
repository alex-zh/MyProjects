using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Builders;
using Common.Classes.General;

namespace Common.Finders
{
    /// <summary>
    /// Finds specified structures
    /// For example { 1% up in 5-8 candles, 2% down in next 5-8 candles }
    /// </summary>
    public class StructureFinder
    {
        private readonly List<Candle> _allCandles;
        private readonly List<List<StructureInfoItem>> _structures;
        private readonly bool _useLogScale;

        public StructureFinder(List<Candle> allCandles, List<StructureInfoItem> structure):this(allCandles, structure, false)
        {
            
        }

        public StructureFinder(List<Candle> allCandles, List<StructureInfoItem> structure, bool useLogScale)
        {
            _allCandles = useLogScale ? allCandles.Log().ToList() : allCandles;
            _structures = VaryStructure(structure);
            _useLogScale = useLogScale;
        }

        private List<List<StructureInfoItem>> VaryStructure(List<StructureInfoItem> structure)
        {
            var varyCoefficients = new[] { 0.75, 1, 1.5, 2 };

            var result = new List<List<StructureInfoItem>>();

            foreach (var coefficient in varyCoefficients)
            {
                var newStructure = new List<StructureInfoItem>();

                foreach (var item in structure)
                {
                    var newItem = item.Clone();
                    newItem.Change = newItem.Change * coefficient;
                    newItem.Period = (int)(newItem.Period * coefficient);

                    newStructure.Add(newItem);
                }

                result.Add(newStructure);
            }

            return result;
        }

        public List<TrendPart> FindAll()
        {
            var result = new List<TrendPart>();

            foreach (var structure in _structures)
            {
                result.AddRange(FindAll(structure));
            }

            return result;
        }

        private List<TrendPart> FindAll(List<StructureInfoItem> structure)
        {
            var result = new List<TrendPart>();

            for (int i = 0; i < _allCandles.Count; i++)
            {
                int structureItemIndex = 0;
                int endIndexOffset = 0;
                var startIndex = i - 1;

                var trendPart = new TrendPart { StartIndex = i };
                
                for (structureItemIndex = 0; structureItemIndex < structure.Count; structureItemIndex++)
                {
                    var structureItem = structure[structureItemIndex];

                    startIndex = (startIndex + 1) + endIndexOffset;

                    if (IsStructureItemFound(startIndex, structureItem, out endIndexOffset))
                    {
                        trendPart.Parts.Add(new TrendPart { StartIndex = startIndex - i, EndIndex = startIndex - i + endIndexOffset });
                    }
                    else
                    {
                        break;
                    }
                }

                if (structureItemIndex == structure.Count)
                {
                    trendPart.EndIndex = trendPart.StartIndex + trendPart.Parts.Last().EndIndex;

                    result.Add(trendPart);
                    i = startIndex + endIndexOffset;
                }
            }

            return result;
        }

        private bool IsStructureItemFound(int startLookupIndex, StructureInfoItem structureInfoItem, out int foundStructureEndIndex)
        {
            var rangeStartIndex = startLookupIndex + (int) (structureInfoItem.Period*0.5);
            var rangeEndIndex =  startLookupIndex + (int)(structureInfoItem.Period * 1.5);

            rangeEndIndex = Math.Min(_allCandles.Count-1, rangeEndIndex);

            for (int i = rangeStartIndex; i <= rangeEndIndex  ; i++)
            {
                var change = CalculateChange( _allCandles[startLookupIndex], _allCandles[i], _useLogScale);
                if (change * structureInfoItem.Change >= 0 &&
                    Math.Abs(change) >= Math.Abs(structureInfoItem.Change * 0.9) && Math.Abs(change) <= Math.Abs(structureInfoItem.Change * 1.1))
                {
                    foundStructureEndIndex = i - startLookupIndex;
                    return true;
                }
            }

            foundStructureEndIndex = 0;

            return false;
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
