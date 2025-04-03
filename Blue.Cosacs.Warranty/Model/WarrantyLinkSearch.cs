using System;
using Blue.Cosacs.Stock;
using System.Collections.Generic;
using System.Linq;
using Blue.Data;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantyLinkSearch : PagedSearch
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public DateTime? EffectiveStartFrom { get; set; }
        public DateTime? EffectiveStartTo { get; set; }
    }
}