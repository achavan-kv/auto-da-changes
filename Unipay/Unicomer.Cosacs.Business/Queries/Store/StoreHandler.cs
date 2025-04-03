/* 
Author:  
Date:  
Description:JM BlueStart-Store Service API 
 */

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model.Models.Stores;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Queries.Store
{
    public class StoreServiceHandler : IRequestHandler<StoreQuery, StoreResponseModel>
    {
        private readonly IStoreRepository StoreRepository;
        private readonly IDbLogggerRepository dbLoggger;

        public StoreServiceHandler(IStoreRepository StoreRepository, IDbLogggerRepository dbLoggger)
        {
            this.StoreRepository = StoreRepository;
            this.dbLoggger = dbLoggger;
        }

        public async Task<StoreResponseModel> Handle(StoreQuery request, CancellationToken cancellationToken)
        {
            System.Exception exception = null;
            StoreModel result = null;

            try
            {
                result = StoreRepository.GetStore(request.CountryIsoCode, request);
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

            return await Task.FromResult(new StoreResponseModel { Store = result });
        }
    }
}
