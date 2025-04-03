using System.Collections.Generic;

namespace Blue.Cosacs.Credit.Model
{
    public class CustomerFullDetails
    {
        public Customer Customer { get; set; }

        public List<CustomerAddress> CustomerAddresses { get; set; }

        public List<CustomerContact> CustomerContacts { get; set; }

        public List<CustomerTag> CustomerTags { get; set; }
    }
}
