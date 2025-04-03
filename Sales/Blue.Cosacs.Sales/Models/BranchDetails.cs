using System.Collections.Generic;
using System.Globalization;

namespace Blue.Cosacs.Sales.Models
{
    public class BranchDetails
    {
        public int Id { get; set; }
        public string LocationId { get; set; }
        public string SalesId { get; set; }
        public string Name { get; set; }
        public string Fascia { get; set; }
        public bool Warehouse { get; set; }
        public bool VirtualWarehouse { get; set; }
        public bool Active { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string StoreType { get; set; }
        public IEnumerable<StringKeyValue> Contacts { get; set; }

        public short BranchNumber
        {
            get { return short.Parse(SalesId, CultureInfo.InvariantCulture); }
        }
    }
}
