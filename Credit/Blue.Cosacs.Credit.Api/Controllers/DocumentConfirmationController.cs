using Blue.Admin;
using Blue.Cosacs.Credit.EventTypes;
using Blue.Cosacs.Credit.Repositories;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class DocumentConfirmationController : ApiController
    {
        private IEventStore audit;
        private readonly IDocumentConfirmationRepository repository;
        private readonly IClock clock;

        public DocumentConfirmationController(IEventStore audit, IDocumentConfirmationRepository repository, IClock clock)
        {
            this.audit = audit;
            this.repository = repository;
            this.clock = clock;
        }

        public HttpResponseMessage Get(int id)
        {
            if (this.GetUser().HasPermission(PermissionsEnum.EditProposal) && this.GetUser().HasPermission(PermissionsEnum.ViewProposal))
            {
                var documentConfirmationSummary = repository.GetDocumentConfirmationSummary(id);
                var documentConfirmation = repository.GetDocumentConfirmationDetails(id);
                var documentConfirmationFiles = repository.GetDocumentConfirmationFiles(id);
                var fields = repository.GetDocumentConfirmationRules(documentConfirmationSummary.CustomerType);
                var previousProposal = repository.GetPreviousProposal(id);
                var currentProposal = repository.GetCurrentProposal(id);
                if (currentProposal.IsAddressChanged(previousProposal))
                {
                    foreach (var field in fields)
                    {
                        foreach (var item in field.Fields)
                        {
                            if (item.FieldID == "ProofAddress")
                            {
                                item.Required = true;
                            }
                        }
                    }
                }

                if (currentProposal.IsMonthlyIncomeChanged(previousProposal) || currentProposal.IsCurrentEmploymentDateChanged(previousProposal))
                {
                    foreach (var field in fields)
                    {
                        foreach (var item in field.Fields)
                        {
                            if (item.FieldID == "ProofIncome")
                            {
                                item.Required = true;
                            }
                        }
                    }
                }

                return Request.CreateResponse(new { proposalSummary = documentConfirmationSummary, documentConfirmation = documentConfirmation, files = documentConfirmationFiles, fields = fields });
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        [Permission(PermissionsEnum.EditProposal)]
        public HttpResponseMessage Post(Model.DocumentConfirmation dc)
        {
            repository.SaveDocumentConfirmation(dc, this.GetUser().Id, clock.Now);
            audit.Log(dc, EventType.DocumentConfirmationSaved, EventCategory.DocumentConfirmation);
            return Request.CreateResponse();
        }

        [Permission(PermissionsEnum.EditProposal)]
        public HttpResponseMessage Put(int id, Model.File file)
        {
            file.CreatedBy = this.GetUser().Id;
            file.CreatedOn = clock.Now;
            repository.SaveDocumentConfirmationFiles(id, file);
            audit.Log(file, EventType.DocumentConfirmationSaved, EventCategory.DocumentConfirmation);
            return Request.CreateResponse();
        }

        [Permission(PermissionsEnum.EditProposal)]
        public HttpResponseMessage Delete(Guid id)
        {
            repository.DeleteDocumentConfirmationFiles(id);
            return Request.CreateResponse();
        }
    }
}
