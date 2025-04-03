using System.Collections.Generic;

namespace Blue.Cosacs.Merchandising.Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> HierarchyOptions { get; set; }
        public List<ProductStatus> Statuses { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Suppliers { get; set; }
        public List<string> StoreTypes { get; set; }
        public bool IsMasterInstance { get; set; }

        public Dictionary<int, string> Locations { get; set; }

        public List<Brand> Brands { get; set; }
    }
}