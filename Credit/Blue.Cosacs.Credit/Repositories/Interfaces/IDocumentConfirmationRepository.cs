using System;
using System.Collections.Generic;
namespace Blue.Cosacs.Credit.Repositories
{
    public interface IDocumentConfirmationRepository
    {
        void SaveDocumentConfirmation(Model.DocumentConfirmation documentConfirmation, int updatedBy, DateTime? updatedOn);
        Model.Proposal GetDocumentConfirmationSummary(int id);
        DocumentConfirmation GetDocumentConfirmationDetails(int proposalId);
        void SaveDocumentConfirmationFiles(int id, Model.File file);
        void DeleteDocumentConfirmationFiles(Guid id);
        List<File> GetDocumentConfirmationFiles(int proposalId);
        List<Model.DocumentConfirmationRules> GetDocumentConfirmationRules(string customerType);
        Model.Proposal GetPreviousProposal(int proposalId);
        Model.Proposal GetCurrentProposal(int proposalId);
    }
}
