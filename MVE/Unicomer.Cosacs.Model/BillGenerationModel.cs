using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Unicomer.Cosacs.Model
{
    public class BillGenerationHeader
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerId { get; set; }
        public string AccountNo { get; set; }
        public string FinanaceCharges { get; set; }
        public string CheckOutId { get; set; }
        public string GST { get; set; }
        public string TotalPrice { get; set; }
        [DataType(DataType.Date)]
        public string ReceiptDate { get; set; }
        public List<BillGenerationLists> BillGenerationList { get; set; }
        public string AccountType { get; set; }
        public string AddressType { get; set; }
        public string BillType { get; set; }
        public string EmployeeNo { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string CoSaCSUserID { get; set; }
    }

    public class BillGenerationLists
    {
        public string OrderId { get; set; }
        public string itemno { get; set; }
        public string ItemName { get; set; }
        public string Quantity { get; set; }
        public string stocklocn { get; set; }
        public string AddressType { get; set; }
        public string Price { get; set; }
        public string GST { get; set; }
        public string Discount { get; set; }
    }
}
