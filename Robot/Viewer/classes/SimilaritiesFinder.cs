using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Classes.General;

namespace Viewer.classes
{
    public class SimilaritiesFinder
    {
        public static IEnumerable<TrendFeature> FindSimilarFeatures(IEnumerable<TrendFeature> allFeatures, TrendFeature parrentFeature)
        {
            var result = new List<TrendFeature>();

            var featureDistanceList = new List<KeyValuePair<TrendFeature, double>>();

            foreach (var feature in allFeatures)
            {
                var distance = 0.0;

                for (int i = 0; i < feature.GetValues().Count; i++)
                {
                    distance += Math.Pow((feature.GetValues()[i] - parrentFeature.GetValues()[i]), 2);
                }

                distance = Math.Sqrt(distance);

                featureDistanceList.Add(new KeyValuePair<TrendFeature, double>(feature, distance));
            }

            var statistics = new StatisticsCalculator(featureDistanceList.Select(x => x.Value));
            var threshold = statistics.Quantile(20);

            var tempResult = featureDistanceList.Where(x => x.Value < threshold).ToList();

            //foreach (var featureDistanceItem in featureDistanceList)
            //{
            //    if (featureDistanceItem.Value < threshold)
            //        tempResult.Add(featureDistanceItem.Key);
            //}

            //   return tempResult;
            var intersectedResults = new List<KeyValuePair<TrendFeature, double>> { tempResult.First() };

            for (int i = 1; i < tempResult.Count; i++)
            {
                var feature = tempResult[i];

                var hasIntersections = intersectedResults.Any(x => x.Key.EndDate > feature.Key.StartDate);

                if (hasIntersections)
                {
                    intersectedResults.Add(feature);
                }
                else
                {
                    var mostClose = intersectedResults.OrderBy(x => x.Value).First();
                    mostClose.Key.Weight = intersectedResults.Count;

                    result.Add(mostClose.Key);
                    intersectedResults.Clear();
                    intersectedResults.Add(feature);
                }
            }

            return result;

        }
    }
}
