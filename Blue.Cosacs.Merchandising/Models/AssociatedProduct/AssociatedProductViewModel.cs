using System.Collections.Generic;

namespace Blue.Cosacs.Merchandising.Models
{
    public class AssociatedProductViewModel
    {
        public Dictionary<string, string> Hierarchy { get; set; }
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> HierarchyOptions { get; set; }
        public Dictionary<string, string> ProductOptions { get; set; }
        public List<AssociatedProduct> AssociatedProducts { get; set; }
    }
}