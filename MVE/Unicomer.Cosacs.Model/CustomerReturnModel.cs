using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class CustomerReturnModel
    {
        public string ResourceType { get; set; }
        public string Source { get; set; }
        public string AccountNo { get; set; }
        public string Name { get; set; }
        public string CustomerId { get; set; }
        public string NOTES { get; set; }
        public double TotalCreditValuetoacct { get; set; }
        public string CheckOutId { get; set; }
        public List<CustomerReturnList> CustomerReturnList { get; set; }
    }

    public class CustomerReturnList
    {
        public string OrderId { get; set; }
        public string ReturnType { get; set; }
        public string CollectionReason { get; set; }       
        public string ItemNo { get; set; }
        public double Quantity { get; set; }
        public Int32 Stocklocn { get; set; }
        public Int32 ReturnStocklocn { get; set; }
        public double TotalValue { get; set; }     
    }
}
