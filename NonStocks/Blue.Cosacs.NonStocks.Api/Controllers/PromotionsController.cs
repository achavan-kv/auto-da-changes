using Blue.Cosacs.NonStocks.Models;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Filter = Blue.Cosacs.NonStocks.Promotions.Filter;

namespace Blue.Cosacs.NonStocks.Api.Controllers
{
    [RoutePrefix("Api/Promotions")]
    public class PromotionsController : ApiController
    {
        private readonly PromotionsRepository promotionsRepository;
        private readonly Blue.IClock clock;
        private readonly IEventStore audit;

        public PromotionsController(PromotionsRepository promotionsRepository, Blue.IClock clock, IEventStore audit)
        {
            this.promotionsRepository = promotionsRepository;
            this.clock = clock;
            this.audit = audit;
        }

        [Permission(PermissionsEnum.NonStocksPromotionsView)]
        public HttpResponseMessage Get([FromUri]Filter filterValues)
        {
            var nonStockPromotions = promotionsRepository.GetPromotions(filterValues);
            //Put the whole result in the log is not good at all
            //audit.Log(@event: nonStockPromotions.Page,
            //    category: AuditCategories.NonStocksPromotions, type: AuditEventTypes.SearchPromotions);

            return Request.CreateResponse(nonStockPromotions);
        }

        [Permission(PermissionsEnum.NonStocksPromotionsView)]
        public HttpResponseMessage Get(int id)
        {
            var promotions = promotionsRepository.GetPromotionsForNonStock(id, clock.UtcNow)
               .Select(p => new
               {
                   p.BranchName,
                   p.BranchNumber,
                   p.Fascia,
                   p.EndDate,
                   p.Id,
                   p.PercentageDiscount,
                   p.RetailPrice,
                   p.StartDate,
                   p.NonStockId,
                   p.NonStockNumber
               })
               .Distinct()//since there is multiple prices for the same nonstock, a Distinct (excluding the NonStockPriceId property) is needed
               .ToList();

            //audit.Log(@event: promotions,
            //    category: AuditCategories.NonStocksPromotions, type: AuditEventTypes.ViewEspecificPromotions);

            return Request.CreateResponse(new { Result = "Done", @NonStockPromotions = promotions });
        }

        [Permission(PermissionsEnum.NonStocksPromotionsEdit)]
        public HttpResponseMessage Put(NonStockPromotionModel promotion)
        {
            try
            {
                var nonStockPromotion = promotionsRepository.SavePromotion(promotion);

                audit.Log(@event: nonStockPromotion,
                    category: AuditCategories.NonStocksPromotions, type: AuditEventTypes.CreatePromotions);

                return Request.CreateResponse(new { Result = "Success", @NonStockPromotion = nonStockPromotion });
            }
            catch (OperationCanceledException ex)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [Permission(PermissionsEnum.NonStocksPromotionsEdit)]
        public HttpResponseMessage Delete(int id)
        {
            var promotionToDelete = promotionsRepository.GetPromotion(id);

            promotionsRepository.DeletePromotion(id);

            if (promotionToDelete != null)
            {
                audit.Log(@event: promotionToDelete,
                category: AuditCategories.NonStocksPromotions, type: AuditEventTypes.DeletePromotion);
            }

            return Request.CreateResponse(new { Result = "Success" });
        } 
    }
}
