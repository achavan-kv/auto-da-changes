namespace Blue.Cosacs.Sales.Models
{
    public class OrderSaveReturn
    {
        public OrderSaveReturn()
        {
            InvoiceNo = "";
            //AgreementInvoiceNumber = string.Empty;
            Valid = true;
            Errors = new string[] { };
            FailedPayments = new ValidatePaymentsReturn();
        }

        public string InvoiceNo { get; set; }
        //public string AgreementInvoiceNumber { get; set; }
        public bool Valid { get; set; }
        public string[] Errors { get; set; }
        public ValidatePaymentsReturn FailedPayments { get; set; }
    }
}
