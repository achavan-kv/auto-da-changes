using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Business.Commands.LoanProcess;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Commands.LoanPayOff
{
    public class LoanPayOffHandler : IRequestHandler<LoanPayOffCommand, LoanResponseModel>
    {
        private readonly IHttpClientService httpClientService;
        private readonly IAccountRepository accountRepository;
        private readonly IMediator mediator;

        public LoanPayOffHandler(
            IAccountRepository accountRepository,
            IHttpClientService httpClientService,
            IMediator mediator)
        {
            this.accountRepository = accountRepository;
            this.httpClientService = httpClientService;
            this.mediator = mediator;
        }

        public async Task<LoanResponseModel> Handle(LoanPayOffCommand request, CancellationToken cancellationToken)
        {
            var mambuAccount = accountRepository.GetMambuAccountId(request.CosacsAccountId, "PAY", request);
            var mambuAccountType = accountRepository.GetMambuAccountType(request.CosacsAccountId, request);
            var payOffAmount = accountRepository.GetPayOffAmount(request.CosacsAccountId, request);
            
            var isLoanPayOff = false;

            if (mambuAccountType.Equals("HP", System.StringComparison.InvariantCultureIgnoreCase))
            {
                isLoanPayOff = await httpClientService.LoanPayOffHP(mambuAccount.MambuAccountId, payOffAmount, mambuAccount.TransactionChannelId, mambuAccount.Notes, request);
            }
            else
            {
                isLoanPayOff = await httpClientService.LoanPayOffCL(mambuAccount.MambuAccountId, payOffAmount, mambuAccount.TransactionChannelId, mambuAccount.Notes, request);
            }

            if (isLoanPayOff)
            {
                var command = new LoanProcessCommand
                {
                    CosacsAccountId = request.CosacsAccountId,
                    MambuAccountId = mambuAccount.MambuAccountId,
                    ProcessRef = ApplicationSettingManager.Payoff
                };

                command.SetApiName(request.GetRequest().RequestName);
                var response = await this.mediator.Send(command);

                return await Task.FromResult(new LoanResponseModel { MambuAccountId = mambuAccount.MambuAccountId, Success = true });
            }

            return new LoanResponseModel { MambuAccountId = mambuAccount.MambuAccountId, Success = isLoanPayOff };
        }
    }
}
