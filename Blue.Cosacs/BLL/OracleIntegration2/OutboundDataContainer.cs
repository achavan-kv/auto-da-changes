using System;
using System.Collections.Generic;
using STL.BLL.OracleIntegration;
using System.Text;

namespace STL.BLL.OracleIntegration2
{
    public class OutboundDataContainer
    {
        public int RunNo;
        public List<Customer> CustomerList = new List<Customer>();
        public List<Receipt> ReceiptList = new List<Receipt>();
        public List<ARInvoice> ARInvoiceList = new List<ARInvoice>();
        public List<SalesOrder> SalesOrderList = new List<SalesOrder>();
    }
}
