using System.Collections.Generic;

namespace Blue.Cosacs.Merchandising.Models
{
    public class ProductStatusViewModel
    {
        public IEnumerable<ProductStatus> SystemStatuses { get; set; }
        public IEnumerable<ProductStatus> ManualStatuses { get; set; }
    }
}