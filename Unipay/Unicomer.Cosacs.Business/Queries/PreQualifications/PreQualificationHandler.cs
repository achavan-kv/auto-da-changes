/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API 
 */
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model.Models.Customers;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Queries.PreQualifications
{
    public class PreQualificationHandler : IRequestHandler<PreQualificationQuery, PreQualificationResponseModel>
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IDbLogggerRepository dbLoggger;

        public PreQualificationHandler(ICustomerRepository customerRepository, IDbLogggerRepository dbLoggger)
        {
            this.customerRepository = customerRepository;
            this.dbLoggger = dbLoggger;
        }
        public async Task<PreQualificationResponseModel> Handle(PreQualificationQuery request, CancellationToken cancellationToken)
        {
            System.Exception exception = null;
            PreQualificationModel result = null;

            try
            {
                result = customerRepository.PreQualification(request.SalesCustomerId, request);
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

            return await Task.FromResult(new PreQualificationResponseModel { PreQualificationCustomer = result });
        }
    }
}
