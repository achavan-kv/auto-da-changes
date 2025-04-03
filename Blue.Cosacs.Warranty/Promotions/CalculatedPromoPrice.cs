using Blue.Cosacs.Warranty.Model;
using System.Collections.Generic;

namespace Blue.Cosacs.Warranty.Model
{
    public class PromotionCalculatedPrice
    {
        public PromotionAggregate Promotion { get; set; }
        public decimal? Price { get; set; }
        public int LevelMatch { get; set; }
    }
}
