namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class StockAdjustmentReasonViewModel
    {
        public StockAdjustmentReasonViewModel()
        {
            SecondaryReasons = new List<StockAdjustmentSecondaryReasonViewModel>();
        }

        public int Id { get; set; }

        public string PrimaryReason { get; set; }

        public List<StockAdjustmentSecondaryReasonViewModel> SecondaryReasons { get; set; } 
    }
}
