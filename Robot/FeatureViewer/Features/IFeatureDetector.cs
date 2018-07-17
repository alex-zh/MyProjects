using System.Collections.Generic;

namespace FeatureViewer
{
    public interface IFeatureDetector<T>
    {
         List<T> Detect();
    }
}