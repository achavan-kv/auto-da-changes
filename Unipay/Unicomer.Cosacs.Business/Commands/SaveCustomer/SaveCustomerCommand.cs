/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.Models.Customers;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Commands.SaveCustomer
{
    public class SaveCustomerCommand : IRequest<CustomerResponseViewModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            string requestId = "";
            if (Customer != null && Customer.CustomerDetail != null)
            {
                requestId= Customer.CustomerDetail.CreditCustomerId;
            }

            return new ApiRequest { RequestId = requestId, RequestName = "customer" };
        }
        
        public CustomerModel Customer { get; set; }
    }
}
