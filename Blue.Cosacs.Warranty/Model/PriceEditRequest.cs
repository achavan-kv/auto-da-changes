
namespace Blue.Cosacs.Warranty.Model
{
    public class PriceEditRequest
    {
        public string Operation { get; set; }
        public decimal Amount { get; set; }
        public bool IsPercentage { get; set; }
    }
}
