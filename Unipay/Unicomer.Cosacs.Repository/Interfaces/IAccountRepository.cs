/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API 
 */
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.Models.Loans;
using Unicomer.Cosacs.Model.Models.Loans.Schedule;

namespace Unicomer.Cosacs.Repository.Interfaces
{
    public interface IAccountRepository
    {
        MambuAccountModel GetMambuAccountId(string cosacsAccountId, string apiType, IApiRequest apiRequest);
        string GetMambuAccountType(string cosacsAccountId, IApiRequest apiRequest);
        bool GetLoanDueDetail(string cosacsAccountId, LoanScheduleModel loanSchedule, IApiRequest apiRequest);
        bool UpdateAccountDetail(string cosacsAccountId, decimal earlySettlement, decimal rebate, string accountState, string accountType, IApiRequest apiRequest);
        bool LoanAdjust(decimal amount, string transactionId, string loanAccountId, string cosacsAccountId, IApiRequest apiRequest);
        decimal GetPayOffAmount(string cosacsAccountId, IApiRequest apiRequest);
    }
}
