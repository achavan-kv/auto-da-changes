using System.Collections.Generic;
namespace Unicomer.Cosacs.Model.Models.Payments
{
    public class ReceiptData
    {
        public CreditDetail CreditDetail { get; set; }
        public List<ProductDetail> ProductDetail { get; set; }
    }
}
