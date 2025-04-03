namespace Blue.Cosacs.Sales.Models
{
    public class ValidatePaymentsReturn
    {
        public ValidatePaymentsReturn()
        {
            TempPaymentId = 0;
            TenderedAmount = 0;
            ErrorMessage = string.Empty;
        }

        public int TempPaymentId { get; set; }
        public decimal TenderedAmount { get; set; }
        public string ErrorMessage { get; set; }
    }
}
