namespace Blue.Cosacs.Merchandising.Models
{
    public class StockCountProductViewModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Sku { get; set; }

        public string LongDescription { get; set; }

        public int StartStockOnHand { get; set; }

        public int Count { get; set; }

        public int SystemAdjustment { get; set; }

        public int Variance 
        {
            get
            {
                return Count - StartStockOnHand + SystemAdjustment;
            }
        }

        public int NetMovement { get; set; }

        public int CurrentStockOnHand { get; set; }

        public string Comments { get; set; }

        public decimal Cost { get; set; }

        public string Currency { get; set; }
    }
}