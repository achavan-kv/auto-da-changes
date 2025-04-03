using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public partial class FinTrans
    {
        public FinTrans() { }
        public FinTrans(string fromAccount, short branchno, decimal value, int transfreno, string toAccount, int runno)
        {
           this.acctno = fromAccount;
           this.agrmtno = 1;
           this.branchno = branchno;
            this.datetrans = DateTime.Now;
            this.empeeno = -117;
            this.transvalue = value;
            this.runno = 0;
            this.transrefno = transfreno;
            this.chequeno = toAccount;
            this.source = "COSACS";
            this.paymethod = 0;
            this.transtypecode = "BEX";
            this.transprinted = "Y";
            this.transupdated = "Y";
            this.ftnotes = runno.ToString();
        }
    }
}
