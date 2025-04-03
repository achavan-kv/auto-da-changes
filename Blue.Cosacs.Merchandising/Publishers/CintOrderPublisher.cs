using Blue.Cosacs.Merchandising.Repositories;
using Blue.Cosacs.Messages.Merchandising.CintOrderReceipt;
using Blue.Hub.Client;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Merchandising.Publishers
{
    public interface ICintOrderPublisher
    {
        void PublishDelivered(Model.CintOrder cintOrder, decimal? awc, int runno);
        void PublishCostOfSale(Model.CintOrder cintOrder, decimal? awc, int runno, CintOrder originalDelivery = null);
    }

    public class CintOrderPublisher : ICintOrderPublisher
    {
        private readonly IPublisher publisher;
        private readonly ICostRepository costRepository;
        private readonly IProductRepository productRepository;

        public CintOrderPublisher(IPublisher publisher, ICostRepository costRepository, IProductRepository productRepository)
        {
            this.publisher = publisher;
            this.costRepository = costRepository;
            this.productRepository = productRepository;
        }

        public void PublishDelivered(Model.CintOrder cintOrder, decimal? awc, int runno)
        {
            publisher.Publish<Context, CintOrderReceiptMessage>("Merchandising.CintOrderDelivered", CreateMessage(cintOrder, awc), runno.ToString());
        }

        public void PublishCostOfSale(Model.CintOrder cintOrder, decimal? awc, int runno, CintOrder originalDelivery = null)
        {
            publisher.Publish<Context, CintOrderReceiptMessage>("Merchandising.CintOrderReturned", CreateMessage(cintOrder, awc, originalDelivery), runno.ToString());
        }

        private CintOrderReceiptMessage CreateMessage(Model.CintOrder cintOrder, decimal? awc, CintOrder originalDelivery = null)
        {
            var message = new CintOrderReceiptMessage
            {
                CintOrderId = cintOrder.Id,
                SaleLocationId = cintOrder.SaleLocation,
                StockLocationId = cintOrder.StockLocation,
                Reference = cintOrder.PrimaryReference,
                Description = string.Format("{0}: {1} {2}", cintOrder.PrimaryReference, cintOrder.Type, cintOrder.SecondaryReference),
                ProductId = cintOrder.MerchProductId,
                SaleType = cintOrder.SaleType,
                ReferenceType = cintOrder.ReferenceType
            };
            // Get via sku as productid is an external system Id

            using (var scope = Context.Read())
            {
                var product = productRepository.GetBySku(cintOrder.Sku);

                var productMessage = productRepository.GetProductMessages(new List<int> { product.Id.Value }).FirstOrDefault();
                var fyw = productRepository.GetFirstYearWarranty(product.Id.Value);
                message.ProductId = product.Id.Value;
                message.DepartmentCode = productMessage.DepartmentCode == null ? string.Empty : productMessage.DepartmentCode;
                message.TotalAWC = awc.HasValue ? awc.Value * cintOrder.Quantity : 0;
                message.FirstYearWarranty = fyw;
            }
            return message;
        }
    }
}