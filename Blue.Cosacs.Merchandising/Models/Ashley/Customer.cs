using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Blue.Cosacs.Merchandising.Models.Ashley
{
    public class Customer
    {
        public string custid { get; set; }
        public string title { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string alias { get; set; }
        public string accountNo { get; set; }
        public string relationship { get; set; }
        public System.DateTime dob { get; set; }
        public string accountType { get; set; }
        public string maidenName { get; set; }
        public string loyaltyCardNo { get; set; }
        public string countryCode { get; set; }
        public string storeType { get; set; }
        public string otherTabs { get; set; }
        public string maritalStat { get; set; }
        public int dependants { get; set; }
        public string nationality { get; set; }
        public bool resieveSms { get; set; }
        public short branchNo { get; set; }
        public string CreatedBy { get; set; }
        public string Err { get; set; }
        public List<CustomerAddress> RenissanceCustomerAddressesList { get; set; }
        public List<LineItemList> RenissanceLineItemList { get; set; }        
    }

    
    public class Customer1
    {
        
        public string custid { get; set; }       
        public string title { get; set; }       
        public string firstName { get; set; }       
        public string lastName { get; set; }       
        public string alias { get; set; }       
        public string accountNo { get; set; }       
        public string relationship { get; set; }       
        public System.DateTime dob { get; set; }       
        public string accountType { get; set; }       
        public string maidenName { get; set; }       
        public string loyaltyCardNo { get; set; }       
        public string countryCode { get; set; }       
        public string storeType { get; set; }       
        public string otherTabs { get; set; }       
        public string maritalStat { get; set; }       
        public int dependants { get; set; }       
        public string nationality { get; set; }       
        public bool resieveSms { get; set; }       
        public short branchNo { get; set; }       
        public string CreatedBy { get; set; }       
        public string Err { get; set; }
       // public List<CustomerAddress> RenissanceCustomerAddressesList { get; set; }
       // public List<LineItemList> LineItemList { get; set; }
    }
}
