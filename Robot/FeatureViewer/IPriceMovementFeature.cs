using System.Collections.Generic;
using Common.Classes.Analizers;
using Common.Classes.General;

namespace FeatureViewer
{
    public interface IPriceMovementFeature
    {
        bool IsPresent(int index);
    }
}