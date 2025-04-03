using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Orders
{
    public class CreditFee
    {
        public List<DetailFee> DetailFee { get; set; }
        public List<DetailFee> DetailTaxFee { get; set; }
    }
}
