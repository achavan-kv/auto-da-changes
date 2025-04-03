using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blue.Cosacs.Merchandising.Models
{
    public class SupplierViewModel
    {
        public SupplierModel Supplier { get; set; }
        public List<SupplierStatus> Statuses { get; set; }
    }
}
