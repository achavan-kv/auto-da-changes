namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class SetViewModel
    {
        public SetModel Set { get; set; }
        public IEnumerable<ProductStatus> Statuses { get; set; }
        public Dictionary<int, string> Locations { get; set; }
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> HierarchyOptions { get; set; }
    }
}
