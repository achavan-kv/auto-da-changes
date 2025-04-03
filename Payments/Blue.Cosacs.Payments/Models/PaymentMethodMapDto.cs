using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Payments.Models
{
   public class PaymentMethodMapDto
    {
        public byte Id { get; set; }
        public string Description { get; set; }
        public int? WinCosacsId { get; set; }
    }
}
