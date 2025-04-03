using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Orders;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Repository.Interfaces
{
    public interface ISalesOrderRepository
    {
        SalesOrderResponseModel Save(SalesOrderModel customer, IApiRequest apiRequest);
    }
}
