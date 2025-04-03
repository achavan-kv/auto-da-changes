using Blue.Cosacs.Sales.Models;
using Blue.Cosacs.Sales.Repositories;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.Sales.Api.Controllers
{
    public class OrdersController : ApiController
    {
        private readonly IOrderRepository repository;

        public OrdersController(IOrderRepository repo)
        {
            repository = repo;
        }

        //CR 2018-13
        public OrderDto Get(string invoiceNo)
        {
            return repository.Get(invoiceNo);
        }

        [Permission(SalesPermissionEnum.CreateOrders)]
        public OrderSaveReturn Post(OrderDto salesOrder)
        {
            var userId = this.GetUser().Id;
            var ret = repository.Save(salesOrder, userId);

            if (ret.Valid && salesOrder.Customer != null && salesOrder.Customer.IsSalesCustomer &&
                !string.IsNullOrEmpty(salesOrder.Customer.FirstName) && !string.IsNullOrEmpty(salesOrder.Customer.LastName))
            {
                repository.IndexNewCustomer(salesOrder.Customer.FirstName, salesOrder.Customer.LastName, userId);
            }

            return ret;
        }

        public HttpResponseMessage Get(int branchNo, System.DateTime dateFrom, System.DateTime dateTo, string invoiceNoMin, string invoiceNoMax, int start, int rows)
        {
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, repository.SearchOrdersForRePrint(branchNo, dateFrom, dateTo, invoiceNoMin, invoiceNoMax, start, rows));
        }
    }
}