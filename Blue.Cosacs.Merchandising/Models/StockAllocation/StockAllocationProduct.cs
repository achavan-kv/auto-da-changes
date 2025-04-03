namespace Blue.Cosacs.Merchandising
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class StockAllocationProduct
    {
        [NotMapped]
        public int QuantityPending 
        {
            get
            {
                return Quantity - QuantityReceived + QuantityCancelled;
            }
        }
    }
}
