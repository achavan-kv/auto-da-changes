using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.Web.Models
{
    public class ProcessDeliveryCommand
    {
        public string CosacsAccountId { get; set; }
        public string InvoiceNumber { get; set; }
    }
}