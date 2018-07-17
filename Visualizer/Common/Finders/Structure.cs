using System.Collections.Generic;
using System.Linq;

namespace Common.Finders
{
    public class Structure : List<StructureItem>
    {
        public Structure()
        {
        }

        public Structure(IEnumerable<StructureItem> list)
        {
            this.AddRange(list);
        }

        public int StartIndex
        {
            get
            {
                if(this.Any())
                    return this.First().StartIndex;

                return 0;
            } 
            
        }

        public int EndIndex
        {
            get
            {
                if (this.Any())
                    return this.Last().EndIndex;

                return 0;
            }
        }
    }
}