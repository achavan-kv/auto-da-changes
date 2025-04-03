/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model.Models.Customers;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Commands.SaveCustomer
{
    public class SaveCustomerHandler : IRequestHandler<SaveCustomerCommand, CustomerResponseViewModel>
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IDbLogggerRepository dbLoggger;

        public SaveCustomerHandler(ICustomerRepository customerRepository, IDbLogggerRepository dbLoggger)
        {
            this.customerRepository = customerRepository;
            this.dbLoggger = dbLoggger;
        }

        public async Task<CustomerResponseViewModel> Handle(SaveCustomerCommand request, CancellationToken cancellationToken)
        {
            System.Exception exception = null;
            CustomerResultModel result = null;

            try
            {
                result = customerRepository.Save(request.Customer, request);
            }
            catch (System.Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                dbLoggger.LogApiRequestResponse(request, result, exception);
            }

            return await Task.FromResult(new CustomerResponseViewModel { Customer = result });
        }
    }
}
