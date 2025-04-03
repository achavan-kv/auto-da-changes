/* 
Author: suresh-IGT
Date: Feb 15th
Description:JM BlueStart
 */

using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Payments;

namespace Unicomer.Cosacs.Repository.Interfaces
{
    public interface IPaymentRepository
    {
        PaymentRequestModel GetPaymentRequest(string accountId, string invoiceNumber, IApiRequest apiRequest, out string countryIsoCode);
        bool UpdatePaymentRequest(UpdatePaymentRequest request, IApiRequest apiRequest);
    }
}
