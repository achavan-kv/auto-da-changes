namespace Unicomer.Cosacs.Model.Models.Loans
{
    public class PreviewPayOffCLAmountResponseModel
    {
        public PreviewPayOffCLAmountModel Data { get; set; }
        public ApiResult Result { get; set; }
    }

    public class PreviewPayOffCLAmountModel
    {
        public decimal PrincipalBalance { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal InterestBalance { get; set; }
        public decimal PenaltyBalance { get; set; }
        public decimal FeeBalance { get; set; }
    }
}
