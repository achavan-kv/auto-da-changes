using System;
using System.Collections.Generic;
using System.Text;


namespace STL.BLL.OracleIntegration
{
    

    public class ARInvoiceLine
    {

        public string itemno;
        public string lineDescription;
        public string UOM;
        public double Quantity;
        public decimal UnitPrice;
        public decimal LineValue;
        public string TaxFlag;
        public string TaxCode;
        public decimal TaxRate;
        public string RetItemNo;    //jec 29/10/08
        public int LineRef;
        public string AccountCode;


    }
}
