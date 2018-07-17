using System.Collections.Generic;

namespace Viewer
{
    public interface IFeatureDetector
    {
        TrendFeature Detect();
        List<TrendFeature> DetectAll();
    }
}