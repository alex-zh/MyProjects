using System.Collections.Generic;
using System.Linq;

namespace Common.Finders
{
    public class StructureInfo
    {
        private List<StructureInfoItem> _items = new List<StructureInfoItem>();

        public StructureInfo()
        {
        }

        public StructureInfo(IEnumerable<StructureInfoItem> list)
        {
            _items.AddRange(list);
        }

        public string Id
        {
            get
            {
                var id = "";
                foreach (var item in _items)
                {
                    id += (item.Change * 100).ToString("##0.000;(##0.000)") + "-";
                }

                id = id.Trim().Trim('-');

                return id;
            }
        }

        public List<StructureInfoItem> Items
        {
            get
            {
                return _items;
            }
        }
        
        public StructureInfo Pop()
        {
            return new StructureInfo(this._items.Skip(1));
        }
    }
}