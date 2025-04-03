/* 
Author: Swati -IGT
Date: Feb 10 2022
Description:JM BlueStart-Qualification API 
 */
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Queries.Qualification
{
    public class QualificationHandler : IRequestHandler<QualificationQuery, QualificationResponseModel>
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IDbLogggerRepository dbLoggger;

        public QualificationHandler(ICustomerRepository customerRepository, IDbLogggerRepository dbLoggger)
        {
            this.customerRepository = customerRepository;
            this.dbLoggger = dbLoggger;
        }
        public async Task<QualificationResponseModel> Handle(QualificationQuery request, CancellationToken cancellationToken)
        {
            System.Exception exception = null;
            QualificationResponseModel result = null;

            try
            {
               result = customerRepository.Qualification(request.CountryIsoCode, request.SalesCustomerId, request);
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

            return await Task.FromResult(result);
        }
    }
}
