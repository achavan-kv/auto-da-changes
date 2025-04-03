namespace Blue.Cosacs.Merchandising.Solr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Solr;

    public interface IGoodsReceiptSolrIndexer
    {
        void Index(int[] goodsReceiptIds = null);
    }

    public class GoodsReceiptSolrIndexer : IGoodsReceiptSolrIndexer
    {
        private readonly IStockSolrIndexer stockSolrIndexer;
        private readonly IPurchaseOrderSolrIndexer purchaseOrderSolrIndexer;

        private const string SolrType = "MerchandiseGoodsReceipt";

        public GoodsReceiptSolrIndexer(IStockSolrIndexer stockSolrIndexer, IPurchaseOrderSolrIndexer purchaseOrderSolrIndexer)
        {
            this.stockSolrIndexer = stockSolrIndexer;
            this.purchaseOrderSolrIndexer = purchaseOrderSolrIndexer;
        }

        public void Index(int[] goodsReceiptIds = null)
        {
            var localCurrency = new Settings().LocalCurrency;

            using (var scope = Context.Read())
            {
                var query = scope.Context.ForceGoodsReceiptIndexView.AsNoTracking().AsQueryable();

                if (goodsReceiptIds != null)
                {
                    query = query.Where(r => goodsReceiptIds.Contains(r.ReceiptId));
                }

                var goodsReceipts = query.ToList();

                var documents = goodsReceipts.Select(r => new
                {
                    Id = string.Format("{0}:{1}-{2}", SolrType, r.Type, r.ReceiptId),
                    Status = CalculateStatus(r),
                    TotalCost = (r.Currency == "LOCAL" ? localCurrency : r.Currency) + (r.TotalCost ?? 0).ToCurrency(),
                    Direct = r.Type == "Direct",
                    DateReceived = r.DateReceived.ToSolrDate(),
                    CreatedDate = r.CreatedDate.ToSolrDate(),
                    DateApproved = r.DateApproved.ToSolrDate(),
                    Type = SolrType,
                    ReceivingLocation = r.Location,
                    ReceiptType = r.Type,
                    r.ReceiptId,
                    r.Vendor,
                    r.VendorDeliveryNumber,
                    r.VendorInvoiceNumber,
                    r.CreatedBy,
                    r.ApprovedBy,
                    r.ReceivedBy,
                })
                .ToList();
                new WebClient().Update(documents);

                if (goodsReceiptIds != null)
                {
                    var receiptIds = goodsReceipts.Select(r => r.ReceiptId).ToList();
                    IndexPurchaseOrders(receiptIds);
                }
            }
        }

        private void IndexPurchaseOrders(IEnumerable<int> goodsReceiptIds)
        {
            using (var scope = Context.Read())
            {
                var purchaseOrderIds = scope.Context.GoodsReceiptProductView
                    .Where(grp => goodsReceiptIds.Contains(grp.GoodsReceiptId))
                    .Select(grp => grp.PurchaseOrderId)
                    .Distinct()
                    .ToArray();
                this.purchaseOrderSolrIndexer.Index(purchaseOrderIds);
            }
        }

        private string CalculateStatus(ForceGoodsReceiptIndexView r)
        {
            if (r.Type == "Direct" || r.CostConfirmed.HasValue)
            {
                return r.ApprovedBy == null
                    ? "Awaiting Approval"
                    : "Approved";
            }
            return "Awaiting Costing";
        }
    }
}
