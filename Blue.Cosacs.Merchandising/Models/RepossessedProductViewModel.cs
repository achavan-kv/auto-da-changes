using System.Collections.Generic;

namespace Blue.Cosacs.Merchandising.Models
{
    public class RepossessedProductViewModel : ProductViewModel
    {
        public RepossessionDetailsModel RepossessionDetails { get; set; }
    }
}