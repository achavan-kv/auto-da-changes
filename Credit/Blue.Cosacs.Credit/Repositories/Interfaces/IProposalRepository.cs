using System;
namespace Blue.Cosacs.Credit.Repositories
{
    public interface IProposalRepository
    {
        Blue.Cosacs.Credit.Model.ProposalApplicant1 GetBasicDetailsApplicant1(int id);
        Blue.Cosacs.Credit.Model.ProposalApplicant2 GetBasicDetailsApplicant2(int id);
        void SaveApplicant2(int id, Model.ProposalApplicant2 proposal, int updatedBy, DateTime? updatedOn);
        void SaveNewApplicant2(Blue.Cosacs.Credit.Model.ProposalApplicant2 proposal, int createdBy, DateTime createdOn);
        int SaveNewProposal(Blue.Cosacs.Credit.Model.ProposalApplicant1 proposal);
        void SaveProposal(int id, Blue.Cosacs.Credit.Model.ProposalApplicant1 proposal);
        System.Collections.Generic.List<Blue.Cosacs.Credit.Model.Proposal> Search(Blue.Cosacs.Credit.Model.ProposalSearchParams searchParams);
    }
}
