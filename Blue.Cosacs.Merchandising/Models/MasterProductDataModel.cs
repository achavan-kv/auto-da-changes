using System.Collections.Generic;

namespace Blue.Cosacs.Merchandising.Models
{
    public class MasterProductDataModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Tag> Hierarchy { get; set; }
    }
}
