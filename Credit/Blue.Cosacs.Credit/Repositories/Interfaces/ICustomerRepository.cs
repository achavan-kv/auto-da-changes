using Blue.Cosacs.Credit.Model;
using System;
namespace Blue.Cosacs.Credit.Repositories
{
    public interface ICustomerRepository
    {
        int? CreateNewCustomerFromProposal(int proposalId, int createdBy, DateTime createdOn);
        CustomerFullDetails GetCustomerFullDetails(int customerId);
        int AddContactDetails(string contactType, string contact, int customerId);
        int AddContactDetails(CustomerContact newCustomerContact);
        int AddAddressDetails(CustomerAddress newCustomerAddress);
        void DeleteContactDetails(CustomerContact customerContactToDelete);
        void UpdateCustomerProfilePhoto(int customerId, Guid photoIdentifier);
        int AddCustomerTag(CustomerTag newCustomerTag);
    }
}
