using System.Collections.Generic;

namespace Unicomer.Cosacs.Model
{
    public class SupplierMasterResult
    {
        public List<SupplierMaster> SupplierMaster { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
    public class SupplierMaster
    {
        public string ResourceType { get; set; }
        public string Source { get; set; }
        public bool Active { get; set; }
        public string ExternalVendorID { get; set; }
        public string SupplierName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostalCode { get; set; }
        public string StateorProvince { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Notes { get; set; }
        public string LastUpdatedBy { get; set; }
        public List<string> SupplierType { get; set; }
    }
    public class UpdateSupplier
    {
        public string ResourceType { get; set; }
        public string Source { get; set; }
        public bool Active { get; set; }
        public string ExternalVendorID { get; set; }
        public string SupplierName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostalCode { get; set; }
        public string StateorProvince { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Notes { get; set; }
        public string LastUpdatedBy { get; set; }
        //public List<string> SupplierType { get; set; }
        public string SupplierType { get; set; }
    }

}
