namespace Unicomer.Cosacs.Model.Models.Loans
{
    public class PreviewPayOffHPAmountResponseModel
    {
        public PreviewPayOffHPAmountModel Data { get; set; }
        public ApiResult Result { get; set; }
    }

    public class PreviewPayOffHPAmountModel
    {
        public decimal TotalSettlement { get; set; }
        public decimal AccruedInterest { get; set; }
        public decimal Adjustment { get; set; }
    }
}
