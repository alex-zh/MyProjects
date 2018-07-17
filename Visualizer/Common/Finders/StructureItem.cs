namespace Common.Finders
{
    public class StructureItem
    {
        public bool IsUp { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public StructureItem Clone()
        {
            return new StructureItem()
            {
                StartIndex = this.StartIndex,
                EndIndex = this.EndIndex
            };
        }
    }
}