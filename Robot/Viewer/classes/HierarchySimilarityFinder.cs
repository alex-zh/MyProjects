using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.General;

namespace Viewer.classes
{
    public class HierarchySimilarityFinder
    {
        public static IEnumerable<TrendFeature> Find(IEnumerable<Dictionary<int, TrendFeature>> allFeatures, Dictionary<int, TrendFeature>  patternFeature)
        {
            var levelsCount = patternFeature.Keys.Max() + 1;

            var patterns = allFeatures.ToList();

            for (int levelIndex = 0; levelIndex < levelsCount; levelIndex++)
            {
                patterns = FindSimilar(patterns, patternFeature, levelIndex);
            }

            return patterns.Select(x => x[0]).ToList();
        }

        private static List<Dictionary<int, TrendFeature>> FindSimilar(List<Dictionary<int, TrendFeature>> patterns, Dictionary<int, TrendFeature> patternFeature, int levelIndex)
        {
            var featureDistanceList = new List<KeyValuePair<Dictionary<int, TrendFeature>, double>>();

            foreach (var pattern in patterns)
            {
                var feature = pattern[levelIndex];

                var distance = 0.0;

                for (int i = 0; i < feature.GetValues().Count; i++)
                {
                    distance += Math.Pow((feature.GetValues()[i] - patternFeature[levelIndex].GetValues()[i]), 2);
                }

                distance = Math.Sqrt(distance);

                featureDistanceList.Add(new KeyValuePair<Dictionary<int, TrendFeature>, double>(pattern, distance));
            }

            var statistics = new StatisticsCalculator(featureDistanceList.Select(x => x.Value));
            var threshold = statistics.Quantile(5);

            return featureDistanceList.Where(x => x.Value < threshold).Select(x => x.Key).ToList();
        }
    }
}
