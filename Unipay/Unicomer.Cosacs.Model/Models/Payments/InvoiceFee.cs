using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Payments
{
    public class InvoiceFee
    {
        public List<InvoiceDetailFee> DetailFee { get; set; }
        public List<InvoiceDetailFee> DetailTaxFee { get; set; }
    }
}
