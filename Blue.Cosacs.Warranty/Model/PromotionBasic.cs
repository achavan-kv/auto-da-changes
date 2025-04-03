using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warranty.Model
{
   public class PromotionBasic
    {
        public string WarrantyNumber { get; set; }
        public int? Branch { get; set; }
        public decimal? PromoPrice { get; set; }
        public decimal? PromoPrecent { get; set; }
    }
}
