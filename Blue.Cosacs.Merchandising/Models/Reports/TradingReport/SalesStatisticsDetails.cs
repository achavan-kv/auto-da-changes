namespace Blue.Cosacs.Merchandising.Models
{
    public class SalesStatisticsDetails
    {
        public string Name { get; set; }

        public SalesStatistics Statistics { get; set; }

        public bool IsHeaderRow { get; set; }

        public bool IsGrandTotal { get; set; }
    }
}