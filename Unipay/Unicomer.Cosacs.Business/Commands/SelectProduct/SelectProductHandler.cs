/* 
Author:  
Date:  
Description:POST Select Product Service  API 
 */

using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model.Models.Products;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Commands.SelectProduct
{
    public class SelectProductHandler: IRequestHandler<SelectProductCommand, SelectProductResponsViewModel>
    {

        private readonly ISelectProductRepository selectProductRepository;
        private readonly IDbLogggerRepository dbLoggger;

        public SelectProductHandler(ISelectProductRepository selectProductRepository, IDbLogggerRepository dbLoggger)
        {
            this.selectProductRepository = selectProductRepository;
            this.dbLoggger = dbLoggger;
        }

        public async Task<SelectProductResponsViewModel> Handle(SelectProductCommand request, CancellationToken cancellationToken)
        {
            System.Exception exception = null;
            List<SelectProductResponseModel> result = null;

            try
            {
                result = selectProductRepository.Select(request.ItemsAvailable, request);
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

            return await Task.FromResult(new SelectProductResponsViewModel { itemAvailable = result });
        }
    }
}
