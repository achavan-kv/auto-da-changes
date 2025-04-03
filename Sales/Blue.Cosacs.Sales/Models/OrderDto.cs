using System;
using System.Collections.Generic;

namespace Blue.Cosacs.Sales.Models
{
    [Serializable]
    public class OrderDto
    {
        public OrderDto()
        {
            Payments = new List<PaymentDto>();
            Items = new List<ItemDto>();
        }

        public int SoldBy { get; set; }
        public int Id { get; set; }
        public int OriginalOrderId { get; set; }
        public int? ParentId { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public string Comments { get; set; }
        public bool IsTaxFreeSale { get; set; }
        public bool IsDutyFreeSale { get; set; }
        // Temp:
        public short BranchNo { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }

        public string AgreementInvoiceNumber { get; set; }//Invoice CR

        public CustomerDto Customer { get; set; }
        public List<ItemDto> Items { get; set; }
        public List<ItemDto> ReturnedItems { get; set; }
        public List<PaymentDto> Payments { get; set; }

        public OriginalOrderDto OriginalOrder { get; set; }
    }
}
