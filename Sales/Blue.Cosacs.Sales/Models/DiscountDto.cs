namespace Blue.Cosacs.Sales.Models
{
   public class DiscountDto
    {
       public int Code { get; set; }
       public string ItemNo { get; set; }
       public string Description { get; set; }
       public string PosDescription { get; set; }
       public decimal? Amount { get; set; }
       public string Percentage { get; set; }
       public int Id { get; set; }
       public bool? Returned { get; set; }
       public short? ReturnQuantity { get; set; }
       public int OriginalId { get; set; }
       public string ParentItemNo { get; set; }
       public decimal TaxRate { get; set; }
       public bool IsFixedDiscount { get; set; }
       public decimal? TaxAmount { get; set; }
    }
}
