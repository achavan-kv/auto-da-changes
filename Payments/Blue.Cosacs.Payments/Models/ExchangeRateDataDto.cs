using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Payments.Models
{
    public class ExchangeRateDataDto
    {
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Rate { get; set; }
        public decimal RateChanged { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateFromChanged { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }      
    }
}
