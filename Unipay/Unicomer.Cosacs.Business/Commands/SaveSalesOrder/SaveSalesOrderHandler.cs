using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model.Models.Orders;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Business.Commands.SaveSalesOrder
{
    public class SaveSalesOrderHandler : IRequestHandler<SaveSalesOrderCommand, SalesOrderResponseViewModel>
    {
        private readonly ISalesOrderRepository salesOrderRepository;
        private readonly IDbLogggerRepository dbLoggger;

        public SaveSalesOrderHandler(ISalesOrderRepository salesorderRepository, IDbLogggerRepository dbLoggger)
        {
            this.salesOrderRepository = salesorderRepository;
            this.dbLoggger = dbLoggger;
        }

        public async Task<SalesOrderResponseViewModel> Handle(SaveSalesOrderCommand request, CancellationToken cancellationToken)
        {
            System.Exception exception = null;
            SalesOrderResponseModel result = null;

            try
            {
                result = salesOrderRepository.Save(request.SalesOrder, request);
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

            return await Task.FromResult(new SalesOrderResponseViewModel { SalesOrder = result });
        }
    }
}
