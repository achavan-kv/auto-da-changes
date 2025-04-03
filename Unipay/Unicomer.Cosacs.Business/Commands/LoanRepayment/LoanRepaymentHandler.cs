using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Business.Commands.LoanProcess;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Commands.LoanRepayment
{
    public class LoanRepaymentHandler : IRequestHandler<LoanRepaymentCommand, LoanResponseModel>
    {
        private readonly IRepaymentRepository repaymentRepository;
        private readonly IHttpClientService httpClientService;
        private readonly IMediator mediator;
        private readonly IAccountRepository accountRepository;

        public LoanRepaymentHandler(IRepaymentRepository repaymentRepository,
            IAccountRepository accountRepository,
            IHttpClientService httpClientService,
            IMediator mediator)
        {
            this.accountRepository = accountRepository;
            this.repaymentRepository = repaymentRepository;
            this.httpClientService = httpClientService;
            this.mediator = mediator;
        }

        public async Task<LoanResponseModel> Handle(LoanRepaymentCommand request, CancellationToken cancellationToken)
        {
            var mambuAccount = accountRepository.GetMambuAccountId(request.CosacsAccountId, "PAY", request);
            
            var mambuAccountType = accountRepository.GetMambuAccountType(request.CosacsAccountId, request);
           
            var repaymentTransApiResult = await httpClientService.LoanPayment(mambuAccount.MambuAccountId, request.Amount, mambuAccount.TransactionChannelId, mambuAccount.Notes, mambuAccountType, request);

            repaymentRepository.SaveRepayment(mambuAccount.MambuAccountId, repaymentTransApiResult.Amount, repaymentTransApiResult.EncodedKey, request);

            var command = new LoanProcessCommand { 
                CosacsAccountId = request.CosacsAccountId, 
                MambuAccountId = mambuAccount.MambuAccountId,
                ProcessRef = ApplicationSettingManager.Payment
            };

            command.SetApiName(request.GetRequest().RequestName);
            var response = await this.mediator.Send(command);

            return response;
        }
    }
}
