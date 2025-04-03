using Blue.Cosacs.NonStocks.Models;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.NonStocks.Api.Controllers
{
    [RoutePrefix("Api/Price")]
    public class PriceController : ApiController
    {
        private readonly PriceRepository priceRepository;
        private readonly Blue.IClock clock;
        private readonly IEventStore audit;

        public PriceController(PriceRepository priceRepository, Blue.IClock clock, IEventStore audit)
        {
            this.priceRepository = priceRepository;
            this.clock = clock;
            this.audit = audit;
        }

        [Permission(PermissionsEnum.NonStocksPricesView)]
        public HttpResponseMessage Get(int id)
        {
            if (id <= 0)
            {
                return Request.CreateResponse(new { Result = "Error - Not a valid Id." });
            }

            var nonStockPrices = priceRepository.GetPrices(id);

            //audit.Log(@event: nonStockPrices,
            //    category: AuditCategories.NonStocks, type: AuditEventTypes.ViewNonStockPrices);

            return Request.CreateResponse(new { Result = "Done", @NonStockPrice = nonStockPrices });
        }

        [Permission(PermissionsEnum.NonStocksPricesEdit)]
        public HttpResponseMessage Put(NonStockPriceModel locationPrice)
        {
            try
            {
                var price = priceRepository.SavePrice(locationPrice);

                audit.Log(@event: price,
                    category: AuditCategories.NonStocks, type: AuditEventTypes.CreateNonStockPrice);

                return Request.CreateResponse(new { Result = "Done", @NonStockPrice = price });
            }
            catch (OperationCanceledException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
           
        }

        [Permission(PermissionsEnum.NonStocksPricesEdit)]
        public HttpResponseMessage Delete(int id)
        {
            var price = priceRepository.DeletePrice(id);

            audit.Log(@event: price,
                category: AuditCategories.NonStocks, type: AuditEventTypes.DeleteNonStockPrice);

            return Request.CreateResponse(new { Result = "Done", @NonStockPrice = price });

        } 
    }
}
