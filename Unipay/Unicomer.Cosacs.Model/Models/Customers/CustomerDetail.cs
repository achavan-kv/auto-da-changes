using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Customers
{
    public class CustomerDetail
    {
        public string CreditCustomerId { get; set; }
        public string LineOfCreditId { get; set; }
        public string SalesCustomerId { get; set; }
        public string Nationality { get; set; }
        public string CountryIsoCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public string DateOfBirth { get; set; }
        public string StoreId { get; set; }
        public List<Email> Email { get; set; }
        public List<Identification> Identification { get; set; }
        public List<Phone> Phone { get; set; }
        public List<Work> Work { get; set; }
        public List<PersonalReference> PersonalReference { get; set; }
        public string Gender { get; set; }
        public string CountryId { get; set; }
        public int NumberOfChildren { get; set; }
        public string MaritalStatus { get; set; }
        public decimal IncomesOther { get; set; }
        public List<Address> Address { get; set; }
        public int NumberOfDependents { get; set; }
        public string Salutation { get; set; }
    }
}
