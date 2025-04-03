using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Model
{
   public class LineitemBooking
    {
        public string AcctNo { get; set; }
        public int  AgreementNo { get; set; }
        public int ItemId { get; set; }
        public short StockLocation { get; set; }
        public short? DelLocation { get; set; }
        public short? Quantity { get; set; }   //#16206
    }
}
