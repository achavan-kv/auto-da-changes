namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockMovementTypeViewModel
    {
        public string ID { get; set; }

        public string Type { get; set; }

        public DateTime Date { get; set; }

        public int Movement { get; set; }

        public int Total { get; set; }

        public bool? IsDirect { get; set; }
    }
}