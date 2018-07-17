using System.Collections.Generic;
using OpinionAnalyzer.Core.Classes;

namespace OpinionAnalyzer.Core.Providers
{
    public interface IPatternsProvider
    {
        IEnumerable<Pattern> Patterns { get; }
    }
}