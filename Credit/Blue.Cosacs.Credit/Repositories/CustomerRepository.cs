using Blue.Cosacs.Credit.EventTypes;
using Blue.Cosacs.Credit.Extensions;
using Blue.Cosacs.Credit.Model;
using Blue.Events;
using Blue.Networking;
using System;
using System.Linq;

namespace Blue.Cosacs.Credit.Repositories
{
    public class CustomerRepository : Blue.Cosacs.Credit.Repositories.ICustomerRepository
    {
        private readonly IEventStore audit;
        private readonly IHttpClientJson httpClientJson;

        public CustomerRepository(IEventStore audit, IHttpClientJson httpClientJson)
        {
            this.audit = audit;
            this.httpClientJson = httpClientJson;
        }

        public int? CreateNewCustomerFromProposal(int proposalId, int createdBy, DateTime createdOn)
        {
            int? customerId = null;

            using (var scope = Context.Write())
            {
                var applicant1 = scope.Context.Proposal.Find(proposalId);

                if (!applicant1.CustomerId.HasValue)
                {
                    var newCustomer = new Customer()
                    {
                        Alias = applicant1.Alias.SafeTrim(),
                        Branch = applicant1.Branch,
                        DateOfBirth = applicant1.DateOfBirth.Value.Date,
                        FirstName = applicant1.FirstName.SafeTrim(),
                        Gender = applicant1.Gender.SafeTrim(),
                        LastName = applicant1.LastName.SafeTrim(),
                        Title = applicant1.Title.SafeTrim(),    
                        CreatedBy = createdBy,
                        CreatedOn = createdOn
                    };
                    scope.Context.Customer.Add(newCustomer);
                    scope.Context.SaveChanges();

                    customerId = newCustomer.Id;

                    scope.Context.CustomerAddress.AddRange(
                        scope.Context.ProposalAddress.Where(pa => pa.ProposalId == proposalId).ToList()
                        .Select(pa => new CustomerAddress()
                        {
                            AddressLine1 = pa.AddressLine1.SafeTrim(),
                            AddressLine2 = pa.AddressLine2.SafeTrim(),
                            AddressType = pa.AddressType.SafeTrim(),
                            City = pa.City.SafeTrim(),
                            DeliveryArea = pa.DeliveryArea.SafeTrim(),
                            PostCode = pa.PostCode.SafeTrim(),
                            CustomerId = newCustomer.Id
                        }));

                    AddContactDetails("HomePhone", applicant1.HomePhone, newCustomer.Id);
                    AddContactDetails("Email", applicant1.Email, newCustomer.Id);
                    AddContactDetails("MobilePhone", applicant1.MobilePhone, newCustomer.Id);
                    AddContactDetails("WorkPhone", applicant1.EmployerWorkplacePhone, newCustomer.Id);
                    audit.Log(newCustomer, EventType.CreateCustomerDocumentConfirmation, EventCategory.Customer);
                    scope.Context.SaveChanges();
                }
                scope.Complete();
                return customerId;
            }
        }

        public CustomerFullDetails GetCustomerFullDetails(int customerId)
        {
            CustomerFullDetails customerFullDetails = new CustomerFullDetails();

            using (var scope = Context.Read())
            {
                var customer = (from customerDetails in scope.Context.Customer
                                    where customerDetails.Id == customerId
                                    select customerDetails).Single<Customer>();

                var customerAddresses = from customerAddress in scope.Context.CustomerAddress
                                        where customerAddress.CustomerId == customerId
                                        select customerAddress;

                var customerContacts = from customerContact in scope.Context.CustomerContact
                                       where customerContact.CustomerId == customerId
                                       select customerContact;

                var customerTags = from customerTag in scope.Context.CustomerTag
                                   where customerTag.CustomerId == customerId
                                   select customerTag;

                return new CustomerFullDetails()
                {
                    Customer = customer,
                    CustomerAddresses = customerAddresses.ToList<CustomerAddress>(),
                    CustomerContacts = customerContacts.ToList<CustomerContact>(),
                    CustomerTags = customerTags.ToList<CustomerTag>()
                };
            }
        }

        public int AddContactDetails(CustomerContact newCustomerContact)
        {
            using (var scope = Context.Write())
            {
                scope.Context.CustomerContact.Add(newCustomerContact);
                scope.Context.SaveChanges();
                scope.Complete();
            }

            new CustomerSolrIndex(httpClientJson).Reindex(new[] { newCustomerContact.CustomerId });

            return newCustomerContact.Id;
        }
        
        public int AddContactDetails(string contactType, string contact, int customerId)
        {
            CustomerContact customerContact = new CustomerContact()
            {
                ContactType = contactType.SafeTrim(),
                Contact = contact.SafeTrim(),
                CustomerId = customerId
            };

            return AddContactDetails(customerContact);
        }

        public void DeleteContactDetails(CustomerContact customerContactToDelete)
        {
            using (var scope = Context.Write())
            {
                scope.Context.CustomerContact.Attach(customerContactToDelete);
                scope.Context.CustomerContact.Remove(customerContactToDelete);
                scope.Context.SaveChanges();
                scope.Complete();
            }

            new CustomerSolrIndex(httpClientJson).Reindex(new[] { customerContactToDelete.CustomerId });
        }

        public int AddAddressDetails(CustomerAddress newCustomerAddress)
        {
            using (var scope = Context.Write())
            {
                scope.Context.CustomerAddress.Add(newCustomerAddress);
                scope.Context.SaveChanges();
                scope.Complete();
            }

            new CustomerSolrIndex(httpClientJson).Reindex(new[] { newCustomerAddress.CustomerId });

            return newCustomerAddress.Id;
        }

        public void UpdateCustomerProfilePhoto(int customerId, Guid photoIdentifier)
        {
            using (var scope = Context.Write())
            {
                var customerEntity = scope.Context.Customer.Find(customerId);
                customerEntity.ProfilePhoto = photoIdentifier;
                scope.Context.SaveChanges();
                scope.Complete();
            }

            new CustomerSolrIndex(httpClientJson).Reindex(new[] { customerId });
        }

        public int AddCustomerTag(CustomerTag newCustomerTag)
        {
            using (var scope = Context.Write())
            {
                scope.Context.CustomerTag.Add(newCustomerTag);
                scope.Context.SaveChanges();
                scope.Complete();
            }

            new CustomerSolrIndex(httpClientJson).Reindex(new[] { newCustomerTag.CustomerId });

            return newCustomerTag.Id;
        }
    }
}
