namespace Blue.Cosacs.Merchandising
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class StockTransferProduct
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
