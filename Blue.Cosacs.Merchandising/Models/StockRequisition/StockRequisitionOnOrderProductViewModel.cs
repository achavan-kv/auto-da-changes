namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockRequisitionOnOrderProductViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int Movement { get; set; }

        public int Total { get; set; }
    }
}