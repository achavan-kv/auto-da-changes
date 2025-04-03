using Blue.Cosacs.Credit.Constants;
using Blue.Cosacs.Credit.Extensions;
using Blue.Networking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Credit.Repositories
{
    public class DocumentConfirmationRepository : IDocumentConfirmationRepository
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IHttpClientJson httpClientJson;

        public DocumentConfirmationRepository(ICustomerRepository customerRepostory, IAccountRepository accountRepository, IHttpClientJson httpClientJson)
        {
            this.customerRepository = customerRepostory;
            this.accountRepository = accountRepository;
            this.httpClientJson = httpClientJson;
        }

        public void SaveDocumentConfirmation(Model.DocumentConfirmation documentConfirmation, int updatedBy, DateTime? updatedOn)
        {
            using (var scope = Context.Write())
            {
                var propApp1 = scope.Context.Proposal.Find(documentConfirmation.ProposalId);

                if (propApp1 == null)
                {
                    throw new Exception("Can not find proposal!");
                }

                propApp1.UpdatedBy = updatedBy;
                propApp1.UpdatedOn = updatedOn;

                var dc = scope.Context.DocumentConfirmation.Where(p => p.ProposalId == documentConfirmation.ProposalId).FirstOrDefault();

                if (dc == null)
                {
                    scope.Context.DocumentConfirmation.Add(documentConfirmation.ToTable());
                }
                else
                {
                    dc.GovernmentEmployees = documentConfirmation.GovernmentEmployees.SafeTrim();
                    dc.LandlordDetails = documentConfirmation.LandlordDetails.SafeTrim();
                    dc.OptionalId = documentConfirmation.OptionalId.SafeTrim();
                    dc.OptionalIdType = documentConfirmation.OptionalIdType.SafeTrim();
                    dc.OverseasPerson = documentConfirmation.OverseasPerson.SafeTrim();
                    dc.ProofAddress = documentConfirmation.ProofAddress.SafeTrim();
                    dc.ProofAddressNotes = documentConfirmation.ProofAddressNotes.SafeTrim();
                    dc.CompulsoryId = documentConfirmation.CompulsoryId.SafeTrim();
                    dc.CompulsoryIdType = documentConfirmation.CompulsoryIdType.SafeTrim();
                    dc.CompulsoryIdNotes = documentConfirmation.CompulsoryIdNotes.SafeTrim();
                    dc.ProofIncomeNotes = documentConfirmation.ProofIncomeNotes.SafeTrim();
                    dc.ProofIncomeOther = documentConfirmation.ProofIncomeOther.SafeTrim();
                    dc.ProofIncomeOtherNotes = documentConfirmation.ProofIncomeOtherNotes.SafeTrim();
                    dc.ProposalId = documentConfirmation.ProposalId;
                    dc.References = documentConfirmation.References.SafeTrim();
                    dc.SpouseDetails = documentConfirmation.SpouseDetails.SafeTrim();
                    dc.Comments = documentConfirmation.Comments;
                    dc.ProofIncome = documentConfirmation.ProofIncome;
                }

                var proposal = scope.Context.Proposal.Find(documentConfirmation.ProposalId);
                if (documentConfirmation.Completed)
                {
                    proposal.ApplicationStage = proposal.ApplicationStage | (int)ProposalStagesEnum.DocumentConfirmation;
                }
                if (!proposal.CustomerId.HasValue)
                {
                    documentConfirmation.CreatedBy = updatedBy;
                    documentConfirmation.CreatedOn = updatedOn;
                    proposal.CustomerId = customerRepository.CreateNewCustomerFromProposal(documentConfirmation.ProposalId, documentConfirmation.CreatedBy.Value, documentConfirmation.CreatedOn.Value);

                    if (proposal.CustomerId.HasValue)
                    {
                        new CustomerSolrIndex(httpClientJson).Reindex(new[] { proposal.CustomerId.Value });
                    }
                }
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public Model.Proposal GetDocumentConfirmationSummary(int id)
        {
            var proposal = new Model.Proposal();
            using (var scope = Context.Read())
            {
                proposal.Addresses = scope.Context.ProposalAddress.Where(p => p.ProposalId == id).ToList().Select(address => new Model.Address(address)).ToList();
                var applicant1 = scope.Context.Proposal.Find(id);

                if (applicant1 == null)
                {
                    throw new Exception("Proposal Not Found");
                }
                else
                {
                    proposal.MonthlyIncome = applicant1.MonthlyIncome.HasValue ? applicant1.MonthlyIncome.Value : 0;
                    proposal.Occupation = applicant1.Occupation;
                    proposal.Nationality = applicant1.Nationality;
                    proposal.ApplicationType = applicant1.ApplicationType;
                    proposal.CurrentResidentialStatus = applicant1.CurrentResidentialStatus;
                    proposal.FirstName = applicant1.FirstName;
                    proposal.LastName = applicant1.LastName;

                    proposal.LandlordDetails = new Model.LandlordDetails()
                    {
                        LandlordName = applicant1.LandlordName,
                        LandlordPhone = applicant1.LandlordPhone
                    };
                    proposal.References = scope.Context.ProposalReference.Where(p => p.ProposalId == id &&
                                                                                p.IsApplicant2 == false).ToList();

                    if (applicant1.CustomerId.HasValue)
                    {
                        var date = DateTime.Now.AddMonths(-15);
                        var active = scope.Context.Account.Any(a => a.CustomerId == applicant1.CustomerId.Value && a.SettledOn >= date);
                        proposal.CustomerType = active ? CustomerTypes.Active : CustomerTypes.Old;
                    }
                    else
                    {
                        proposal.CustomerType = CustomerTypes.New;
                    }
                }
            }
            return proposal;
        }

        public DocumentConfirmation GetDocumentConfirmationDetails(int proposalId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.DocumentConfirmation.Where(p => p.ProposalId == proposalId).FirstOrDefault();
            }
        }

        public List<File> GetDocumentConfirmationFiles(int proposalId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.File.Where(p => p.ProposalId == proposalId).ToList();
            }
        }

        public void SaveDocumentConfirmationFiles(int id, Model.File file)
        {
            using (var scope = Context.Write())
            {
                scope.Context.File.Add(new File()
                {
                    FileName = file.FileName,
                    DocumentType = file.UploadName,
                    Guid = file.Guid,
                    ProposalId = id,
                    CreatedBy = file.CreatedBy,
                    CreatedOn = file.CreatedOn
                });
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void DeleteDocumentConfirmationFiles(Guid id)
        {
            using (var scope = Context.Write())
            {
                scope.Context.File.Remove(scope.Context.File.Where(f => f.Guid == id).FirstOrDefault());
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public List<Model.DocumentConfirmationRules> GetDocumentConfirmationRules(string customerType)
        {
            var fieldsList = new List<Model.DocumentConfirmationRules>();

            using (var scope = Context.Read())
            {
                var docConfirmationFields = new List<Model.DocumentConfirmationField>();
                var docConfirmationRules = new Model.DocumentConfirmationRules();

                docConfirmationRules.CustomerType = customerType;

                var customerFields = scope.Context.DocumentConfirmationRules.Where(p => p.CustomerType == customerType).ToList();

                foreach (var field in customerFields)
                {
                    var docConfirmationObject = new Model.DocumentConfirmationField()
                    {
                        FieldID = field.FieldID,
                        FieldType = field.FieldType,
                        FieldDescription = field.FieldDescription,
                        Required = field.Required,
                        Upload = field.Upload
                    };

                    docConfirmationFields.Add(docConfirmationObject);
                }
                docConfirmationRules.Fields = docConfirmationFields;
                fieldsList.Add(docConfirmationRules);
                return fieldsList;
            }
        }

        public Model.Proposal GetPreviousProposal(int proposalId)
        {
            using (var scope = Context.Read())
            {
                var customerId = scope.Context.Proposal.Where(p => p.Id == proposalId).Select(p => p.CustomerId).FirstOrDefault();

                var previousProposal = scope.Context.Proposal.Where(p => p.CustomerId == customerId && p.UpdatedOn != null)
                                             .OrderBy(p => p.UpdatedOn)
                                             .Select(p => p).FirstOrDefault();

                var addresses = scope.Context.ProposalAddress.Where(p => p.ProposalId == previousProposal.Id).ToList();

                var addressesList = new List<Model.Address>();
                foreach (var item in addresses)
                {
                    addressesList.Add(new Model.Address(item));
                }

                var prop = new Model.Proposal()
                {
                    MonthlyIncome = previousProposal.MonthlyIncome,
                    CurrentEmploymentDate = previousProposal.CurrentEmploymentDate,
                    Addresses = addressesList
                };

                return prop;
            }
        }

        public Model.Proposal GetCurrentProposal(int proposalId)
        {
            using (var scope = Context.Read())
            {
                var currentProposal = scope.Context.Proposal.Where(p => p.Id == proposalId).FirstOrDefault();

                var addresses = scope.Context.ProposalAddress.Where(p => p.ProposalId == currentProposal.Id).ToList();

                var addressesList = new List<Model.Address>();
                foreach (var item in addresses)
                {
                    addressesList.Add(new Model.Address(item));
                }

                var prop = new Model.Proposal()
                {
                    MonthlyIncome = currentProposal.MonthlyIncome.Value,
                    CurrentEmploymentDate = currentProposal.CurrentEmploymentDate,
                    Addresses = addressesList
                };

                return prop;
            }
        }
    }
}
