namespace Blue.Cosacs.Merchandising.Models
{
    public class StockMovementModel
    {
        public int ProductId { get; set; }
        public int SendingLocationId { get; set; }
        public int ReceivingLocationId { get; set; }
        public int Quantity { get; set; }
    }
}
