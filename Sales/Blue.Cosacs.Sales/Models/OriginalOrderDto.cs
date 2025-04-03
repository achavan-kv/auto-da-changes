using System;

namespace Blue.Cosacs.Sales.Models
{
    [Serializable]
    public class OriginalOrderDto
    {
        public int? ReceiptNumber { get; set; }
        public OrderDto SalesOrder { get; set; }
    }
}