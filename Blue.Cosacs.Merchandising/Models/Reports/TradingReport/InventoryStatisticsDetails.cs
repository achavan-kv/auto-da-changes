namespace Blue.Cosacs.Merchandising.Models
{
    public class InventoryStatisticsDetails
    {
        public string Name { get; set; }

        public InventoryStatistics Statistics { get; set; }

        public bool IsGrandTotal { get; set; }
    }
}