using System;

namespace Blue.Cosacs.Sales.Models
{
    public class WarrantyContractDetailsResult
    {
        public string ItemNo { get; set; }
        public string WarrantyNo { get; set; }
        public short BranchNo { get; set; }
        public string BranchName { get; set; }
        public int EmployeeNo { get; set; }
        public DateTime AgreementDate { get; set; }
        public string ItemDescription { get; set; }
        public byte? WarrantyLength { get; set; }
        public string ItemPrice { get; set; }
        public string WarrantyPrice { get; set; }
        public string WarrantyDescription { get; set; }
        public string EmployeeName { get; set; }
        public string AccountNo { get; set; }
        public string ContractNo { get; set; }
        public string CustomerTitle { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string CustomerAddressLine2 { get; set; }
        public string CustomerAddressLine3 { get; set; }
        public string CustomerPostCode { get; set; }
        public string CustomerMobilePhone { get; set; }
        public string CustomerHomePhone { get; set; }
        //public string Copy { get; set; }
        public int WarrantyCredit { get; set; }
        public bool IsLast { get; set; }
        
        public char GetAccountNoPartial(int index)
        {
            if (string.IsNullOrEmpty(AccountNo) || index > AccountNo.Length || index < 1)
            {
                return '\0'; //null
            }

            index -= 1;

            return AccountNo[index];
        }
    }

    public class DocumentCopy<T>
    {
        public T Document { get; set; }
        public string CopyText { get; set; }
        public string CountryName { get; set; }
    }
}
