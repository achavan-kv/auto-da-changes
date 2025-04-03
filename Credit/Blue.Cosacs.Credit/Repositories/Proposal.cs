using Blue.Cosacs.Credit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Credit.Repositories
{
    public class ProposalRepository : Blue.Cosacs.Credit.Repositories.IProposalRepository
    {
        public int SaveNewProposal(Model.ProposalApplicant1 proposal)
        {
            var propId = 0;
            using (var scope = Context.Write())
            {
                var newProposal = proposal.Applicant1.ToTable();
                newProposal.ApplicationStage = proposal.Stage;
                newProposal.UpdatedBy = proposal.UpdatedBy;
                newProposal.UpdatedOn = proposal.UpdatedOn;
                scope.Context.Proposal.Add(newProposal);
                scope.Context.SaveChanges();

                propId = newProposal.Id;
                AddAddresses(newProposal.Id, proposal.Addresses);
                scope.Complete();
                return propId;
            }
        }

        public void SaveProposal(int id, Model.ProposalApplicant1 proposal)
        {
            using (var scope = Context.Write())
            {
                var prop = scope.Context.Proposal.Find(id);
                var p = proposal.Applicant1;
                prop.Alias = p.Alias.SafeTrim();
                prop.ApplicationType = p.ApplicationType.SafeTrim();
                prop.Email = p.Email.SafeTrim();
                prop.FirstName = p.FirstName.SafeTrim();
                prop.HomePhone = p.HomePhone.SafeTrim();
                prop.LastName = p.LastName.SafeTrim();
                prop.MobilePhone = p.MobilePhone.SafeTrim();
                prop.Title = p.Title.SafeTrim();
                prop.CustomerId = p.CustomerId;
                prop.ApplicationStage = proposal.Stage;
                prop.UpdatedBy = p.UpdatedBy;
                prop.UpdatedOn = p.UpdatedOn;
                prop.WorkPhone = p.WorkPhone;

                var add = scope.Context.ProposalAddress.Where(a => a.ProposalId == prop.Id).ToList();
                scope.Context.ProposalAddress.RemoveRange(add);

                AddAddresses(prop.Id, proposal.Addresses);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void AddAddresses(int propId, List<Model.Address> addresses)
        {
            using (var scope = Context.Write())
            {
                scope.Context.ProposalAddress.AddRange(
                        addresses.Select(a => a.ToTable(propId)));
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public Model.ProposalApplicant1 GetBasicDetailsApplicant1(int id)
        {
            using (var scope = Context.Read())
            {
                var proposal = new Model.ProposalApplicant1();
                var app1 = scope.Context.Proposal.Find(id);
                if (app1 == null)
                {
                    throw new Exception("Proposal Not Found");
                }
                proposal.Id = app1.Id;
                proposal.Stage = app1.ApplicationStage;
                proposal.Applicant1 = new Model.Applicant1(app1);
                proposal.HasApplicant2 = app1.ApplicationType != "Sole";
                proposal.Addresses = scope.Context.ProposalAddress.Where(p => p.ProposalId == id).ToList().Select(address => new Model.Address(address)).ToList();
                var app2 = scope.Context.ProposalApplicant2.FirstOrDefault(p => p.ProposalId == id);
                if (app2 != null)
                {
                    proposal.Applicant2Name = string.Format("{0} {1}", app2.FirstName, app2.LastName);
                }
                return proposal;
            }
        }

        public Model.ProposalApplicant2 GetBasicDetailsApplicant2(int id)
        {
            using (var scope = Context.Read())
            {
                var app1 = scope.Context.Proposal.Find(id);
                if (app1 == null)
                {
                    throw new Exception("Proposal Not Found");
                }
                var proposal = new Model.ProposalApplicant2()
                {
                    Applicant1Name = string.Format("{0} {1}", app1.FirstName, app1.LastName),
                    Id = app1.Id,
                    Stage = app1.ApplicationStage,
                    ApplicationType = app1.ApplicationType
                };
                var app2 = scope.Context.ProposalApplicant2.FirstOrDefault(p => p.ProposalId == id);
                if (app2 != null)
                {
                    proposal.Applicant2 = new Model.Applicant2(app2);
                }
                return proposal;
            }
        }

        public void SaveNewApplicant2(Model.ProposalApplicant2 proposal, int createdBy, DateTime createdOn)
        {
            using (var scope = Context.Write())
            {
                var app1 = scope.Context.Proposal.Find(proposal.Id);
                if (app1 == null)
                {
                    throw new Exception("Proposal Not Found");
                }
                app1.ApplicationStage = proposal.Stage;

                scope.Context.ProposalApplicant2.Add(proposal.Applicant2.ToTable(proposal.Id));
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void SaveApplicant2(int id, Model.ProposalApplicant2 proposal, int updatedBy, DateTime? updatedOn)
        {
            using (var scope = Context.Write())
            {
                var app1 = scope.Context.Proposal.Find(proposal.Id);
                if (app1 == null)
                {
                    throw new Exception("Proposal Not Found");
                }

                app1.UpdatedBy = updatedBy;
                app1.UpdatedOn = updatedOn;
                app1.ApplicationStage = proposal.Stage;

                var app2 = scope.Context.ProposalApplicant2.FirstOrDefault(p => p.ProposalId == id);
                app2.FirstName = proposal.Applicant2.FirstName.SafeTrim();
                app2.LastName = proposal.Applicant2.LastName.SafeTrim();
                app2.Alias = proposal.Applicant2.Alias.SafeTrim();
                app2.DateOfBirth = proposal.Applicant2.DateOfBirth;
                app2.Gender = proposal.Applicant2.Gender;
                app2.CustomerId = proposal.Applicant2.CustomerId;
                app2.Title = proposal.Applicant2.Title.SafeTrim();
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public List<Model.Proposal> Search(Model.ProposalSearchParams searchParams)
        {
            using (var scope = Context.Read())
            {
                var proposals = from p in scope.Context.Proposal
                                join app2 in scope.Context.ProposalApplicant2 on p.Id equals app2.ProposalId into pj
                                from a in pj.DefaultIfEmpty()
                                join dc in scope.Context.DocumentConfirmation on p.Id equals dc.ProposalId into pdc
                                from docConf in pdc.DefaultIfEmpty()
                                select new { proposal = p, applicant2 = a, documentConfirmation = docConf };

                if (!string.IsNullOrEmpty(searchParams.Branch))
                {
                    var branch = Convert.ToInt16(searchParams.Branch);
                    proposals = proposals.Where(p => p.proposal.Branch == branch);
                }
                if (!string.IsNullOrEmpty(searchParams.FirstName))
                {
                    proposals = proposals.Where(p => p.proposal.FirstName.Contains(searchParams.FirstName));
                }
                if (!string.IsNullOrEmpty(searchParams.LastName))
                {
                    proposals = proposals.Where(p => p.proposal.LastName.Contains(searchParams.LastName));
                }
                if (!string.IsNullOrEmpty(searchParams.Phone))
                {
                    proposals = proposals.Where(p => p.proposal.HomePhone.Contains(searchParams.Phone) ||
                                                     p.proposal.MobilePhone.Contains(searchParams.Phone) ||
                                                     p.proposal.WorkPhone.Contains(searchParams.Phone));
                }
                if (!string.IsNullOrEmpty(searchParams.Email))
                {
                    proposals = proposals.Where(p => p.proposal.Email.Contains(searchParams.Email));
                }
                if (searchParams.SalesPerson.HasValue)
                {
                    proposals = proposals.Where(p => p.proposal.CreatedBy == searchParams.SalesPerson.Value);
                }
                if (searchParams.DateFrom.HasValue)
                {
                    proposals = proposals.Where(p => p.proposal.UpdatedOn >= searchParams.DateFrom.Value);
                }
                if (searchParams.DateTo.HasValue)
                {
                    var dateTo = searchParams.DateTo.Value.AddDays(1);
                    proposals = proposals.Where(p => p.proposal.UpdatedOn < dateTo);
                }
                if (!string.IsNullOrEmpty(searchParams.Source))
                {
                    proposals = proposals.Where(p => p.proposal.Source == searchParams.Source);
                }
                if (!string.IsNullOrEmpty(searchParams.Stage))
                {
                    var stageSearch = ProposalStageScreen.GetStage(searchParams.Stage);
                    proposals = proposals.Where(p => (p.proposal.ApplicationStage & stageSearch.Item1) == stageSearch.Item1 && (p.proposal.ApplicationStage & stageSearch.Item2) == 0);
                }
                if (!string.IsNullOrEmpty(searchParams.CompulsoryId))
                {
                    proposals = proposals.Where(p => p.documentConfirmation.CompulsoryId == searchParams.CompulsoryId.Trim());
                }

                var prop = proposals.OrderByDescending(p => p.proposal.UpdatedOn).Take(50).ToList();
                var ids = prop.Select(p => p.proposal.Id).ToList();
                var addresses = scope.Context.ProposalAddress
                    .Where(a => ids.Contains(a.ProposalId)).ToList();
                var lookup = (from a in addresses
                              select new Model.Address(a)).ToLookup(x => x.ProposalId);
                var ret = new List<Model.Proposal>();
                ret.AddRange(
                        prop.Select(p =>
                            new Model.Proposal()
                            {
                                Id = p.proposal.Id,
                                Applicant1 = new Model.Applicant1(p.proposal),
                                Applicant2 = p.applicant2 == null ? null : new Model.Applicant2(p.applicant2),
                                Addresses = lookup[p.proposal.Id].ToList(),
                                CompulsoryIdType = p.documentConfirmation == null ? string.Empty : p.documentConfirmation.CompulsoryIdType
                            }));
                return ret;
            }
        }
    }
}