using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class StoreCardValidated
    {
        private bool valid;
        public bool Valid 
        {
            get { return valid; }
            set { valid = value; } 
        }
        public string Name { get; set; }

        private string rejectReason = string.Empty;
        public string RejectReason  
        {
            get { return rejectReason; }
            set { rejectReason = value; } 
        }

        public DateTime ExpDate { get; set; }
        public decimal? StoreCardLimit { get; set; }
        public decimal? StoreCardAvailable { get; set; }
        public string Acctno { get; set; }
        public long CardNo { get; set; }
        public bool Suspended { get; set; }
        public bool Blocked { get; set; }
        public decimal? StoreCardBalance { get; set; }
        public double StoreCardInterest { get; set; }
    }
}
