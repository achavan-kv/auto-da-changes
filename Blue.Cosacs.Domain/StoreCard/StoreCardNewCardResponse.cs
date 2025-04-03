using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class StoreCardNewCardResponse
    {
        public long CardNo { get; set; }
        public string Custid { get; set; }
        public string CardName { get; set; }
        public byte IssueMonth { get; set; }
        public short IssueYear { get; set; }
        public byte ExpirationMonth { get; set; }
        public short ExpirationYear { get; set; }
        public string Acctno { get; set; }
    }
}
