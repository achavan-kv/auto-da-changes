using Blue.Cosacs.Credit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Base = Blue.Cosacs.Credit.Model.SanctionStage1;

namespace Blue.Cosacs.Credit.Repositories
{
    public class SanctionStage1Repository : ISanctionStage1Repository
    {
        public void SaveSanctionStage1Applicant1(int id, Base.SanctionStage1 proposal, int updatedBy, DateTime? updatedOn)
        {
            using (var scope = Context.Write())
            {
                var proposalApp1 = scope.Context.Proposal.Find(id);

                if (proposalApp1 == null)
                {
                    throw new Exception("Can not find Proposal!");
                }
                proposalApp1.DateOfBirth = proposal.Applicant.DateOfBirth;
                proposalApp1.Gender = proposal.Applicant.Gender.SafeTrim();
                proposalApp1.EthnicGroup = proposal.Applicant.EthnicGroup.SafeTrim();
                proposalApp1.Nationality = proposal.Applicant.Nationality.SafeTrim();
                proposalApp1.DateInCurrentAddress = proposal.Applicant.DateInCurrentAddress;
                proposalApp1.CurrentResidentialStatus = proposal.Applicant.CurrentResidentialStatus.SafeTrim();
                proposalApp1.PropertyType = proposal.Applicant.PropertyType.SafeTrim();
                proposalApp1.RentAmountPerMonth = proposal.Applicant.RentAmountPerMonth;
                proposalApp1.CurrentEmploymentDate = proposal.Applicant.CurrentEmploymentDate;
                proposalApp1.EmploymentStatus = proposal.Applicant.EmploymentStatus.SafeTrim();
                proposalApp1.Occupation = proposal.Applicant.Occupation.SafeTrim();
                proposalApp1.PayFrequency = proposal.Applicant.PayFrequency.SafeTrim();
                proposalApp1.MonthlyIncome = proposal.Applicant.MonthlyIncome;
                proposalApp1.AdditionalIncomeFrequency = proposal.Applicant.AdditionalIncomeFrequency.SafeTrim();
                proposalApp1.AdditionalIncomeAmount = proposal.Applicant.AdditionalIncomeAmount;
                proposalApp1.UtilitiesFees = proposal.Applicant.UtilitiesFees;
                proposalApp1.LoansOrCreditCardsFees = proposal.Applicant.LoansOrCreditCardsFees;
                proposalApp1.SchoolFees = proposal.Applicant.SchoolFees;
                proposalApp1.OtherFees = proposal.Applicant.OtherFees;
                proposalApp1.HasCreditCard = proposal.Applicant.HasCreditCard;
                proposalApp1.PaymentMethod = proposal.Applicant.PaymentMethod.SafeTrim();
                proposalApp1.BankName = proposal.Applicant.BankName.SafeTrim();
                proposalApp1.AccountType = proposal.Applicant.AccountType.SafeTrim();
                proposalApp1.DateBankAccountOpened = proposal.Applicant.DateBankAccountOpened;
                proposalApp1.AccountNumber = proposal.Applicant.AccountNumber.SafeTrim();
                proposalApp1.AccountBranch = proposal.Applicant.AccountBranch;
                proposalApp1.PayAmount = proposal.Applicant.PayAmount;
                proposalApp1.NoOfDependants = proposal.Applicant.NoOfDependants;
                proposalApp1.ApplicationStage = proposal.Stage;
                proposalApp1.UpdatedBy = updatedBy;
                proposalApp1.UpdatedOn = updatedOn;
                proposalApp1.MaritalStatus = proposal.Applicant.MaritalStatus;
                scope.Context.SaveChanges();

                SaveEmploymentHistory(proposal.EmploymentHistory, id, false);
                scope.Complete();
            }
        }

        public void SaveSanctionStage1Applicant2(int id, Base.SanctionStage1 proposal, int updatedBy, DateTime? updatedOn)
        {
            using (var scope = Context.Write())
            {
                var proposalApp1 = scope.Context.Proposal.Find(id);
                if (proposalApp1 == null)
                {
                    throw new Exception("Can not find proposal");
                }
                proposalApp1.ApplicationStage = proposal.Stage;
                proposalApp1.UpdatedOn = updatedOn;
                proposalApp1.UpdatedBy = updatedBy;

                var proposalApp2 = scope.Context.ProposalApplicant2.Where(p => p.ProposalId == id).FirstOrDefault();
                if (proposalApp1 == null)
                {
                    throw new Exception("Can not find proposal applicant 2");
                }

                proposalApp2.DateOfBirth = proposal.Applicant.DateOfBirth;
                proposalApp2.Gender = proposal.Applicant.Gender.SafeTrim();
                proposalApp2.EthnicGroup = proposal.Applicant.EthnicGroup.SafeTrim();
                proposalApp2.DateInCurrentAddress = proposal.Applicant.DateInCurrentAddress;
                proposalApp2.CurrentResidentialStatus = proposal.Applicant.CurrentResidentialStatus.SafeTrim();
                proposalApp2.PropertyType = proposal.Applicant.PropertyType.SafeTrim();
                proposalApp2.Nationality = proposal.Applicant.Nationality.SafeTrim();

                proposalApp2.RentAmountPerMonth = proposal.Applicant.RentAmountPerMonth;
                proposalApp2.CurrentEmploymentDate = proposal.Applicant.CurrentEmploymentDate;
                proposalApp2.EmploymentStatus = proposal.Applicant.EmploymentStatus.SafeTrim();
                proposalApp2.Occupation = proposal.Applicant.Occupation.SafeTrim();
                proposalApp2.PayFrequency = proposal.Applicant.PayFrequency.SafeTrim();
                proposalApp2.PayAmount = proposal.Applicant.PayAmount;
                proposalApp2.MonthlyIncome = proposal.Applicant.MonthlyIncome;
                proposalApp2.AdditionalIncomeFrequency = proposal.Applicant.AdditionalIncomeFrequency.SafeTrim();
                proposalApp2.AdditionalIncomeAmount = proposal.Applicant.AdditionalIncomeAmount;

                proposalApp2.UtilitiesFees = proposal.Applicant.UtilitiesFees;
                proposalApp2.LoansOrCreditCardsFees = proposal.Applicant.LoansOrCreditCardsFees;
                proposalApp2.SchoolFees = proposal.Applicant.SchoolFees;
                proposalApp2.OtherFees = proposal.Applicant.OtherFees;
                proposalApp2.PaymentMethod = proposal.Applicant.PaymentMethod.SafeTrim();
                proposalApp2.BankName = proposal.Applicant.BankName.SafeTrim();
                proposalApp2.AccountType = proposal.Applicant.AccountType.SafeTrim();
                proposalApp2.DateBankAccountOpened = proposal.Applicant.DateBankAccountOpened;
                proposalApp2.HasCreditCard = proposal.Applicant.HasCreditCard;
                proposalApp2.AccountNumber = proposal.Applicant.AccountNumber.SafeTrim();
                proposalApp2.AccountBranch = proposal.Applicant.AccountBranch;
                proposalApp2.NoOfDependants = proposal.Applicant.NoOfDependants;
                proposalApp2.MaritalStatus = proposal.Applicant.MaritalStatus;
                scope.Context.SaveChanges();

                SaveEmploymentHistory(proposal.EmploymentHistory, id, true);
                scope.Complete();
            }
        }

        private void SaveEmploymentHistory(IEnumerable<Base.EmploymentHistory> employments, int proposalId, bool isApplicant2)
        {
            using (var scope = Context.Write())
            {
                scope.Context.EmploymentHistory.RemoveRange(scope.Context.EmploymentHistory.Where(e => e.ProposalId == proposalId && e.IsApplicant2 == isApplicant2));
                scope.Context.SaveChanges();
                scope.Context.EmploymentHistory.AddRange(employments.Select(e => e.ToTable(proposalId)));
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public Base.SanctionStage1 GetApplicant1Details(int id)
        {
            using (var scope = Context.Read())
            {
                var proposal = new Base.SanctionStage1();
                var applicant1 = scope.Context.Proposal.Find(id);
                if (applicant1 == null)
                {
                    throw new Exception("Can not find proposal");
                }
                proposal.Id = id;
                proposal.Applicant1Name = string.Format("{0} {1}", applicant1.FirstName, applicant1.LastName);
                proposal.IsApplicant2 = false;
                proposal.ApplicationType = applicant1.ApplicationType;
                proposal.Stage = applicant1.ApplicationStage;

                var oldProposal = scope.Context.Proposal.Where(p => p.CustomerId == applicant1.CustomerId.Value && p.Id != id);

                // Stage Completed not completed and has customer.
                if (((int)ProposalStageScreen.Sanction1App1 & applicant1.ApplicationStage) == 0 && applicant1.CustomerId.HasValue && oldProposal.Any())
                {
                    var oldApp1 = oldProposal.OrderByDescending(p => p.UpdatedOn).FirstOrDefault();
                    proposal.Applicant = new Model.SanctionStage1.Applicant(oldApp1);
                    proposal.EmploymentHistory = scope.Context.EmploymentHistory.Where(e => e.ProposalId == oldApp1.Id && !e.IsApplicant2).ToList()
                                                                                .Select(e => new Base.EmploymentHistory(e)).ToList();
                    proposal.Filled = true;
                }
                else
                {
                    proposal.Applicant = new Base.Applicant(applicant1);
                    proposal.EmploymentHistory = scope.Context.EmploymentHistory.Where(e => e.ProposalId == id && !e.IsApplicant2).ToList()
                                                                                .Select(e => new Base.EmploymentHistory(e)).ToList();
                }

                var applicant2 = scope.Context.ProposalApplicant2.Select(p => new { p.ProposalId, p.FirstName, p.LastName })
                                 .FirstOrDefault(p => p.ProposalId == id);
                if (applicant2 != null)
                {
                    proposal.Applicant2Name = string.Format("{0} {1}", applicant2.FirstName, applicant2.LastName);
                }
                return proposal;
            }
        }

        public Base.SanctionStage1 GetApplicant2Details(int id)
        {
            using (var scope = Context.Read())
            {
                var proposal = new Base.SanctionStage1();
                var applicant1 = scope.Context.Proposal.Find(id);
                if (applicant1 == null)
                {
                    throw new Exception("Can not find proposal");
                }
                proposal.Id = id;
                proposal.Applicant1Name = string.Format("{0} {1}", applicant1.FirstName, applicant1.LastName);

                proposal.IsApplicant2 = true;
                proposal.ApplicationType = applicant1.ApplicationType;
                proposal.Stage = applicant1.ApplicationStage;

                var applicant2 = scope.Context.ProposalApplicant2.Where(p => p.ProposalId == id).FirstOrDefault();
                if (applicant1 == null)
                {
                    throw new Exception("Can not find proposal");
                }
                var customerLookup = from p in scope.Context.Proposal
                                     join pa in scope.Context.ProposalApplicant2 on p.Id equals pa.ProposalId
                                     where p.CustomerId == applicant2.CustomerId.Value && p.Id != applicant2.Id
                                     orderby p.UpdatedOn descending
                                     select pa;

                var history = scope.Context.EmploymentHistory.Where(e => e.IsApplicant2);

                // Stage not completed and has a previous customer.
                if (((int)ProposalStageScreen.Sanction1App2 & applicant1.ApplicationStage) == 0 && applicant2.CustomerId.HasValue && customerLookup.Any())
                {
                    var oldProposal = customerLookup.FirstOrDefault();
                    proposal.Applicant = new Model.SanctionStage1.Applicant(oldProposal);
                    proposal.EmploymentHistory = history.Where(e => e.ProposalId == oldProposal.ProposalId).ToList()
                                                        .Select(e => new Base.EmploymentHistory(e)).ToList();
                    proposal.Filled = true;
                }
                else
                {
                    proposal.Applicant = new Model.SanctionStage1.Applicant(applicant2);
                    proposal.EmploymentHistory = history.Where(e => e.ProposalId == id).ToList()
                                                        .Select(e => new Base.EmploymentHistory(e)).ToList();
                }
                proposal.Applicant2Name = string.Format("{0} {1}", applicant2.FirstName, applicant2.LastName);
                return proposal;
            }
        }
    }
}
