using System.Collections.Generic;

namespace Viewer
{
    public class TestStructureResult: List<TestStructureResultItem>
    {
        
    }

    public class TestStructureResultItem
    {
        private List<double> _returns = new List<double>();

        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public List<double> Returns
        {
            get { return _returns; }
        }
    }
}