using MediatR;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.Models.Orders;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Business.Commands.SaveSalesOrder
{
    public class SaveSalesOrderCommand : IRequest<SalesOrderResponseViewModel>, IApiRequest
    {
        public ApiRequest GetRequest()
        {
            string requestId = "";
            if (SalesOrder != null)
            {
                requestId = SalesOrder.CreditCustomerId;
            }

            return new ApiRequest { RequestId = requestId, RequestName = "salesorder" };
        }

        public SalesOrderModel SalesOrder { get; set; }
    }
}
