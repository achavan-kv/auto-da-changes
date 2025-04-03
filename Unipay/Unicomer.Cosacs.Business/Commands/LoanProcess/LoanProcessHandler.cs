/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Commands.LoanProcess
{
    public class LoanProcessHandler : IRequestHandler<LoanProcessCommand, LoanResponseModel>
    {
        private readonly IAccountRepository accountRepository;
        private readonly IHttpClientService httpClientService;

        public LoanProcessHandler(IAccountRepository accountRepository, IHttpClientService httpClientService)
        {
            this.accountRepository = accountRepository;
            this.httpClientService = httpClientService;
        }

        public async Task<LoanResponseModel> Handle(LoanProcessCommand request, CancellationToken cancellationToken)
        {
            decimal earlySettlement = 0;
            decimal rebate = 0;

            if (string.IsNullOrEmpty(request.MambuAccountId))
            {
                request.MambuAccountId = accountRepository.GetMambuAccountId(request.CosacsAccountId, "GET", request).MambuAccountId;
            }

            var mambuAccountType = accountRepository.GetMambuAccountType(request.CosacsAccountId, request);

            var loanScheduleApiResult = await httpClientService.GetLoanSchedule(request.MambuAccountId, request.ProcessRef, mambuAccountType, request);

            if ((!string.IsNullOrWhiteSpace(loanScheduleApiResult.LoanAccountDetails.AccountState))
                && (!loanScheduleApiResult.LoanAccountDetails.AccountState.Trim().Equals("CLOSED", StringComparison.InvariantCultureIgnoreCase))
                && (!loanScheduleApiResult.LoanAccountDetails.AccountState.Trim().Equals("APPROVED", StringComparison.InvariantCultureIgnoreCase))
                && (!loanScheduleApiResult.LoanAccountDetails.AccountState.Trim().Equals("PENDING_APPROVAL", StringComparison.InvariantCultureIgnoreCase))
                )
            {
                try
                {
                    if (mambuAccountType.Equals("HP", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var loanPreviewPayoffHPApiResult = await httpClientService.LoanPreviewPayOffHP(request.MambuAccountId, request.ProcessRef, request);
                        earlySettlement = loanPreviewPayoffHPApiResult.TotalSettlement;
                        rebate = loanPreviewPayoffHPApiResult.Adjustment;
                    }
                    else
                    {
                        var loanPreviewPayoffApiResult = await httpClientService.LoanPreviewPayOffCL(request.MambuAccountId, request.ProcessRef, DateTime.Now, request);
                        earlySettlement = loanPreviewPayoffApiResult.TotalBalance;
                    }
                }
                catch (Exception ex)
                {
                    if (!(!string.IsNullOrWhiteSpace(loanScheduleApiResult.LoanAccountDetails.ProductType) && loanScheduleApiResult.LoanAccountDetails.ProductType.Trim().Equals("Write Off HP", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        throw;
                    }
                }
            }

            accountRepository.GetLoanDueDetail(request.CosacsAccountId, loanScheduleApiResult, request);

            accountRepository.UpdateAccountDetail(request.CosacsAccountId, earlySettlement, rebate, loanScheduleApiResult.LoanAccountDetails.AccountState, mambuAccountType, request);

            return await Task.FromResult(new LoanResponseModel { MambuAccountId = request.MambuAccountId, Success = true });
        }
    }
}
