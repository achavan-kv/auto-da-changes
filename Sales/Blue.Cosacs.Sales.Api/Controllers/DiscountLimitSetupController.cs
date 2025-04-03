using Blue.Cosacs.Sales.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Blue.Glaucous.Client.Api;
using Blue.Cosacs.Sales.Models;

namespace Blue.Cosacs.Sales.Api.Controllers
{
    public class DiscountLimitSetupController : ApiController
    {
        private readonly ISaleRepository repository;

        public DiscountLimitSetupController(ISaleRepository repository)
        {
            this.repository = repository;
        }

        public decimal Get(int branchNumber)
        {
            return repository.GetDiscountLimit(branchNumber);
        }

        public IEnumerable<DiscountLimit> Get([FromUri]DiscountLimit searchData)
        {
            return repository.GetDiscountLimitData(searchData);
        }

        public CustomResponseMessage Post([FromBody]DiscountLimit discountLimitDetail)
        {
            return repository.InsertDiscountLimit(discountLimitDetail, this.GetUser().Id);
        }

        public CustomResponseMessage Put([FromBody]DiscountLimit discountLimitDetail)
        {
            return repository.UpdateDiscountLimit(discountLimitDetail, this.GetUser().Id);
        }

        public void Delete([FromUri]DiscountLimit deleteData)
        {
            repository.DeleteDiscountLimit(deleteData);
        }
    }
}