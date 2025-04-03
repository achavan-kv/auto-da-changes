using Blue.Cosacs.Credit.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using Base = Blue.Cosacs.Credit.Model.SanctionStage2;

namespace Blue.Cosacs.Credit.Repositories
{
    public class SanctionStage2Repository : ISanctionStage2Repository
    {
        private int MonthsInCurrentAddress(DateTime? dateInCurrentAddress, IClock clock)
        {
            return dateInCurrentAddress.HasValue ? (clock.Now.Month - dateInCurrentAddress.Value.Month) + (12 * (clock.Now.Year - dateInCurrentAddress.Value.Year)) : 0;
        }

        public Base.SanctionStage2 GetApplicant1Details(int id, IClock clock)
        {
            using (var scope = Context.Read())
            {
                var proposal = new Base.SanctionStage2();
                var applicant1 = scope.Context.Proposal.Find(id);
                if (applicant1 == null)
                {
                    throw new Exception("Proposal not found!");
                }
                proposal.Id = id;
                proposal.Applicant1Name = string.Format("{0} {1}", applicant1.FirstName, applicant1.LastName);
                proposal.MonthsInCurrentAddress = MonthsInCurrentAddress(applicant1.DateInCurrentAddress, clock);
                proposal.CurrentResidentialStatus = applicant1.CurrentResidentialStatus;
                proposal.IsApplicant2 = false;
                proposal.Stage = applicant1.ApplicationStage;
                proposal.ApplicationType = applicant1.ApplicationType;

                var applicant1Previous = scope.Context.Proposal.Where(p => p.CustomerId == applicant1.CustomerId.Value && p.Id != id && applicant1.CustomerId.HasValue);
                // Not populated and has a matching previous proposal.
                if (((int)ProposalStageScreen.Sanction2App1 & applicant1.ApplicationStage) == 0 && applicant1Previous.Any())
                {
                    var a1p = applicant1Previous.OrderByDescending(p => p.UpdatedOn).FirstOrDefault();
                    proposal.Applicant1 = new Base.Applicant1(a1p);
                    proposal.References = scope.Context.ProposalReference
                                          .Where(p => p.ProposalId == a1p.Id && !p.IsApplicant2)
                                          .ToList()
                                          .Select(p => new Base.ProposalReference(p, filled: true)).ToList();
                    proposal.Filled = true;
                }
                else
                {
                    proposal.Applicant1 = new Base.Applicant1(applicant1);
                    proposal.References = scope.Context.ProposalReference
                                              .Where(p => p.ProposalId == id && !p.IsApplicant2)
                                              .ToList()
                                              .Select(p => new Base.ProposalReference(p)).ToList();

                    if (((int)ProposalStageScreen.Sanction2App1 & applicant1.ApplicationStage) == 0 && applicant1.Occupation == ProposalOccupationTypes.SelfEmployed)
                    {
                        // Not Completed and untouched. 
                        var homeAdress = scope.Context.ProposalAddress.Where(p => p.ProposalId == applicant1.Id && p.AddressType == ProposalAddressTypes.Home)
                            .Select(p => new { p.AddressLine1, p.AddressLine2, p.City, p.PostCode }).FirstOrDefault();
                        proposal.Applicant1.EmployerName = string.Format("{0} {1}", applicant1.FirstName, applicant1.LastName);
                        proposal.Applicant1.EmployerAddress = string.Format("{0} {1} {2} {3}", homeAdress.AddressLine1, homeAdress.AddressLine2, homeAdress.City, homeAdress.PostCode);
                        proposal.Applicant1.EmployerWorkplacePhone = applicant1.HomePhone;
                        proposal.FilledSelfEmployed = true;
                    }
                }
                var applicant2 = scope.Context.ProposalApplicant2.Where(p => p.ProposalId == id).Select(p => new { p.FirstName, p.LastName }).FirstOrDefault();
                if (applicant2 != null)
                {
                    proposal.Applicant2Name = string.Format("{0} {1}", applicant2.FirstName, applicant2.LastName);
                }
                return proposal;
            }
        }

        public Base.SanctionStage2 GetApplicant2Details(int id)
        {
            using (var scope = Context.Read())
            {
                var proposal = new Base.SanctionStage2();
                var applicant1 = scope.Context.Proposal.Find(id);
                if (applicant1 == null)
                {
                    throw new Exception("Proposal not found!");
                }
                proposal.Id = id;
                proposal.Applicant1Name = string.Format("{0} {1}", applicant1.FirstName, applicant1.LastName);
                proposal.IsApplicant2 = true;
                proposal.Stage = applicant1.ApplicationStage;
                proposal.ApplicationType = applicant1.ApplicationType;

                var applicant2 = scope.Context.ProposalApplicant2.Where(p => p.ProposalId == id).Select(p => new { p.FirstName, p.LastName, p.CustomerId,p.Id }).FirstOrDefault();
                if (applicant2 == null)
                {
                    throw new Exception("Applicant 2 not found!");
                }
                proposal.Applicant2Name = string.Format("{0} {1}", applicant2.FirstName, applicant2.LastName);

                var customerLookup = from p in scope.Context.Proposal
                                     join pa in scope.Context.ProposalApplicant2 on p.Id equals pa.ProposalId
                                     where p.CustomerId == applicant2.CustomerId.Value && p.Id != applicant2.Id
                                     orderby p.UpdatedOn descending
                                     select p.Id;

                if (((int)ProposalStagesEnum.Sanction2App2References & applicant1.ApplicationStage) == 0 && customerLookup.Any())
                {
                    //Stage 2 Not completed - Lookup old.
                    var applicant1PreviousId = customerLookup.FirstOrDefault();
                    proposal.References = scope.Context.ProposalReference
                                         .Where(p => p.ProposalId == applicant1PreviousId && p.IsApplicant2)
                                         .ToList()
                                         .Select(p => new Base.ProposalReference(p, filled: true)).ToList();
                    proposal.Filled = true;
                }
                else
                {
                    //Stage2 Completed or partially filled.
                    proposal.References = scope.Context.ProposalReference
                                              .Where(p => p.ProposalId == id && p.IsApplicant2)
                                              .ToList()
                                              .Select(p => new Base.ProposalReference(p)).ToList();
                }
                return proposal;
            }
        }

        public void SaveSanctionStage2Applicant1(int id, Base.SanctionStage2 proposal, bool isApplicant2, int updatedBy, DateTime? updatedOn)
        {
            using (var scope = Context.Write())
            {
                var propApp1 = scope.Context.Proposal.Find(id);

                if (propApp1 == null)
                {
                    throw new Exception("Can not find proposal!");
                }
                propApp1.PreviousAddress = proposal.Applicant1.PreviousAddress;
                propApp1.LandlordName = proposal.Applicant1.LandlordName;
                propApp1.LandlordPhone = proposal.Applicant1.LandlordPhone;
                propApp1.EmployerName = proposal.Applicant1.EmployerName;
                propApp1.EmployerAddress = proposal.Applicant1.EmployerAddress;
                propApp1.EmployerDepartment = proposal.Applicant1.EmployerDepartment;
                propApp1.EmployerWorkplacePhone = proposal.Applicant1.EmployerWorkplacePhone;
                propApp1.ApplicationStage = proposal.Stage;
                propApp1.UpdatedBy = updatedBy;
                propApp1.UpdatedOn = updatedOn;

                SaveSanctionStage2References(proposal.Id, isApplicant2, proposal.References);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void SaveSanctionStage2Applicant2(int id, Base.SanctionStage2 proposal, bool isApplicant2, int updatedBy, DateTime? updatedOn)
        {
            using (var scope = Context.Write())
            {
                var propApp1 = scope.Context.Proposal.Find(id);

                if (propApp1 == null)
                {
                    throw new Exception("Can not find proposal!");
                }
                propApp1.ApplicationStage = proposal.Stage;
                propApp1.UpdatedBy = updatedBy;
                propApp1.UpdatedOn = updatedOn;

                SaveSanctionStage2References(proposal.Id, isApplicant2, proposal.References);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void SaveSanctionStage2References(int proposalId, bool isApplicant2, List<Base.ProposalReference> references)
        {
            using (var scope = Context.Write())
            {
                var old = scope.Context.ProposalReference.Where(p => p.ProposalId == proposalId && p.IsApplicant2 == isApplicant2);
                scope.Context.ProposalReference.RemoveRange(old);
                scope.Context.SaveChanges();
                scope.Context.ProposalReference.AddRange(references.Select(r => r.ToTable(proposalId, isApplicant2)));
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }
    }
}
