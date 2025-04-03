using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Business.Commands.LoanProcess;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Commands.LoanAdjust
{
    public class LoanAdjustHandler : IRequestHandler<LoanAdjustCommand, LoanResponseModel>
    {
        private readonly IAccountRepository accountRepository;
        private readonly IHttpClientService httpClientService;
        private readonly IMediator mediator;

        public LoanAdjustHandler(IAccountRepository accountRepository, IHttpClientService httpClientService, IMediator mediator)
        {
            this.accountRepository = accountRepository;
            this.httpClientService = httpClientService;
            this.mediator = mediator;
        }

        public async Task<LoanResponseModel> Handle(LoanAdjustCommand request, CancellationToken cancellationToken)
        {
            var mambuAccount = accountRepository.GetMambuAccountId(request.CosacsAccountId, "ADJUST", request);
            var mambuAccountType = accountRepository.GetMambuAccountType(request.CosacsAccountId, request);
            
            var loanAdjust = await httpClientService.LoanReverse(request.LoanTransactionId, mambuAccount.Notes, mambuAccountType, request);

            accountRepository.LoanAdjust(loanAdjust.Amount, loanAdjust.EncodedKey, mambuAccount.MambuAccountId, request.CosacsAccountId, request);

            var command = new LoanProcessCommand
            {
                CosacsAccountId = request.CosacsAccountId,
                MambuAccountId = mambuAccount.MambuAccountId,
                ProcessRef = ApplicationSettingManager.Reverse
            };

            command.SetApiName(request.GetRequest().RequestName);
            var response = await this.mediator.Send(command);


            return await Task.FromResult(new LoanResponseModel { MambuAccountId = mambuAccount.MambuAccountId, Success = response.Success });
        }
    }
}
