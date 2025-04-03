using System;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Deliveries;
using Unicomer.Cosacs.Model.Models.Loans;
using Unicomer.Cosacs.Model.Models.Loans.Repayment;
using Unicomer.Cosacs.Model.Models.Loans.Reverse;
using Unicomer.Cosacs.Model.Models.Loans.Schedule;
using Unicomer.Cosacs.Model.Models.Payments;

namespace Unicomer.Cosacs.Business.Interfaces
{
    public interface IHttpClientService
    {
        Task<LoanScheduleModel> GetLoanSchedule(string mambuAccountId, string processRef, string typeProduct, IApiRequest apiRequest);
        Task<LoanRepaymentModel> LoanPayment(string mambuAccountId, decimal amount, string transactionChannelId, string notes, string typeProduct, IApiRequest apiRequest);
        Task<bool> LoanPayOffCL(string mambuAccountId, decimal amount, string transactionChannelId, string notes, IApiRequest apiRequest);
        Task<bool> LoanPayOffHP(string mambuAccountId, decimal amount, string transactionChannelId, string notes, IApiRequest apiRequest);
        Task<PreviewPayOffCLAmountModel> LoanPreviewPayOffCL(string mambuAccountId, string processRef, DateTime valueDate, IApiRequest apiRequest);
        Task<PreviewPayOffHPAmountModel> LoanPreviewPayOffHP(string mambuAccountId, string processRef, IApiRequest apiRequest);
        Task<LoanReverseAdjustModel> LoanReverse(string loanTransactionId, string notes, string typeProduct, IApiRequest apiRequest);


        Task<DeliveryNotificationResponseModel> DeliveryNotification(DeliveryNotificationModel deliveryNotification, IApiRequest apiRequest);
        
        Task<bool> PaymentRequest(PaymentRequestModel request, string countryIsoCode, IApiRequest apiRequest);
    }
}
