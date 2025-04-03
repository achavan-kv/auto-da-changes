using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Models
{
    public class UserViewModel
    {
        public List<int> PermissionIds { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EMail { get; set; }
        public string FullName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostCode { get; set; }
        public string Phone { get; set; }
        public string PhoneAlternate { get; set; }
    }
}
