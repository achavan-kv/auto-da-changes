namespace Blue.Cosacs.Merchandising.Models
{
    using FileHelpers;

    [DelimitedRecord(",")]
    public class PromoExportViewModel
    {
        public string ProductCode { get; set; }

        public string WarehouseNumber { get; set; }

        public string HPPrice1 { get; set; }

        public string HPDateFrom1 { get; set; }

        public string HPDateTo1 { get; set; }

        public string HPPrice2 { get; set; }

        public string HPDateFrom2 { get; set; }

        public string HPDateTo2 { get; set; }
        public string HPPrice3 { get; set; }

        public string HPDateFrom3 { get; set; }

        public string HPDateTo3 { get; set; }
        public string CashPrice1 { get; set; }

        public string CashDateFrom1 { get; set; }

        public string CashDateTo1 { get; set; }
        public string CashPrice2 { get; set; }

        public string CashDateFrom2 { get; set; }

        public string CashDateTo2 { get; set; }
        public string CashPrice3 { get; set; }

        public string CashDateFrom3 { get; set; }

        public string CashDateTo3 { get; set; }

        public string PromotionId { get; set; }
    }
}