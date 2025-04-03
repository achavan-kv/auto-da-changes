namespace Blue.Cosacs.Web.Areas.Merchandising.Controllers
{
    using System.Web.Mvc;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Cosacs.Web.Common;
    using Blue.Glaucous.Client.Mvc;
    using System.Net.Http;
    using System.Net;

    public class StockCountStartController : Controller
    {
        private readonly IStockCountRepository stockCountRepository;

        public StockCountStartController(IStockCountRepository stockCountRepository)
        {
            this.stockCountRepository = stockCountRepository;
        }

        [Permission(Cosacs.Merchandising.MerchandisingPermissionEnum.StockCountStartPerpetualQuarterly)]
        public ViewResult Start(int id)
        {
            return View(id);
        }

        [Permission(Cosacs.Merchandising.MerchandisingPermissionEnum.StockCountStartPerpetual)]
        public ViewResult StartPerpetual(int id)
        {
            return View(id);
        }

        [Permission(Cosacs.Merchandising.MerchandisingPermissionEnum.StockCountStartPerpetualQuarterly)]
        public JsonResult Create(int model)
        {
            stockCountRepository.CreateStockProducts(model, HttpContext.GetUser().Id);
            return new JSendResult(JSendStatus.Success);
        }

        [Permission(Cosacs.Merchandising.MerchandisingPermissionEnum.StockCountStartPerpetual)]
        public JsonResult CreatePerpetual(int model)
        {
            if (stockCountRepository.IsPerpetualStockCount(model))
            {
                stockCountRepository.CreateStockProducts(model, HttpContext.GetUser().Id);
                return new JSendResult(JSendStatus.Success);
            }
            else
            {
                return new JSendResult(JSendStatus.Error, model, string.Format("StockCount Id {0} does not belong to perpetual type", model));
            }
        }

        [HttpGet]
        [Permission(Cosacs.Merchandising.MerchandisingPermissionEnum.StockCountStartPerpetualQuarterly)]
        public JsonResult Get(int id)
        {
            var result = stockCountRepository.GetStockCountStart(id);
            return new JSendResult(JSendStatus.Success, result);
        }

        [Permission(Cosacs.Merchandising.MerchandisingPermissionEnum.StockCountStartPerpetual)]
        public JsonResult GetPerpetual(int id)
        {
            if (stockCountRepository.IsPerpetualStockCount(id))
            {
                var result = stockCountRepository.GetStockCountStart(id);
                return new JSendResult(JSendStatus.Success, result);
            }
            else
            {
                return new JSendResult(JSendStatus.Error, id, "StockCount is not Perpetual type");
            }
        }
    }
}