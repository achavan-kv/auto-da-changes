using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Models.Ashley
{
    public class PaymentDetails
    {
        public string accountNo { get; set; } // No Need 
        public bool sundryCredit { get; set; } // False 
        public short paymentMethod { get; set; } // 1
        public string chequeNo { get; set; } // ""
        public string bankCode { get; set; } // ""
        public string bankAcctNo { get; set; } // ""
        public short branchNo { get; set; } 
        public DataSet payments { get; set; }
        public decimal localTender { get; set; }
        public decimal localChange { get; set; }
        public int authorisedBy { get; set; }
        public DateTime chequeClearance { get; set; }
        public int receiptNo { get; set; }
        public int commissionRef { get; set; }
        public int paymentRef { get; set; }
        public int rebateRef { get; set; }
        public decimal rebateSum { get; set; }
        public string countryCode { get; set; }
        public string voucherReference { get; set; }
        public bool courtsVoucher { get; set; }
        public int voucherAuthorisedBy { get; set; }
        public string accountNoCompany { get; set; }
        public int returnedChequeAuthorisedBy { get; set; }
        public int agrmtno { get; set; }
        public string storeCardAcctno { get; set; }
        public long storeCardNo { get; set; }
    }
}
