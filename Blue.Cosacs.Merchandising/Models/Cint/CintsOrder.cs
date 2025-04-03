using System;
using messages = Blue.Cosacs.Messages.Merchandising.Cints;
namespace Blue.Cosacs.Merchandising.Model
{
    public class CintOrder
    {
        public string Type { get; set; }
        public string PrimaryReference { get; set; }
        public string SecondaryReference { get; set; }
        public string ReferenceType { get; set; }
        public string SaleType { get; set; }
        public string SaleLocation { get; set; }
        public int SaleLocationId { get; set; }
        public string Sku { get; set; }
        public int WinProductId { get; set; }
        public int MerchProductId { get; set; }
        public string StockLocation { get; set; }
        public string ParentSku { get; set; }
        public int? ParentId { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public int Quantity { get; set; }
        public int QuantityAbs { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public decimal? CashPrice { get; set; }
        public decimal? Tax { get; set; }
        public int? PromotionId { get; set; }
        public int Id { get; set; }
        public int TempId { get; set; }
        public string Error { get; set; }
        public decimal? Awc { get; set; }
        public int StockLocationId { get; set; }
        public Blue.Cosacs.Merchandising.CintOrder ParentOrder { get; set; }
        public int? CintRegularOrderId { get; set; }
        public int? Runno { get; set; }
        public bool HasExistingOrder { get; set; }

        public CintOrder()
        {
        }

        public CintOrder(messages.CintOrder s)
        {
            this.CashPrice = s.CashPrice;
            this.Discount = s.Discount;
            this.ParentId = s.ParentId;
            this.ParentSku = s.ParentSku;
            this.Price = s.Price;
            this.PrimaryReference = s.PrimaryReference;
            this.WinProductId = s.ProductId;
            this.PromotionId = s.PromotionId;
            this.Quantity = s.Quantity;
            this.SaleLocation = s.SaleLocation;
            this.ReferenceType = s.ReferenceType;
            this.Type = s.Type;
            this.SecondaryReference = s.SecondaryReference;
            this.Sku = s.Sku;
            this.TransactionDate = s.TransactionDate;
            this.SaleType = s.SaleType;
            this.StockLocation = s.StockLocation;
            this.Tax = s.Tax;
            this.Runno = s.RunNo;
        }

        public messages.CintOrderSubmit ToMessage(int runno)
        {
            return new messages.CintOrderSubmit()
            {
                CintOrder = new messages.CintOrder
                {
                    CashPrice = this.CashPrice,
                    Discount = this.Discount,
                    ParentId = this.ParentId,
                    ParentIdSpecified = true,
                    ParentSku = this.ParentSku,
                    Price = this.Price,
                    PrimaryReference = this.PrimaryReference,
                    ProductId = this.WinProductId,
                    PromotionId = this.PromotionId,
                    PromotionIdSpecified = true,
                    Quantity = this.Quantity,
                    SaleLocation = this.SaleLocation,
                    ReferenceType = this.ReferenceType,
                    Type = this.Type,
                    SecondaryReference = this.SecondaryReference,
                    Sku = this.Sku,
                    TransactionDate = this.TransactionDate,
                    SaleType = this.SaleType,
                    StockLocation = this.StockLocation,
                    Tax = this.Tax,
                    Error = this.Error,
                    RunNo = runno,
                    RunNoSpecified = true
                }
            };
        }

        public Merchandising.CintOrderTVP ToTVP(int runno, decimal? awc, decimal? repoPrice, decimal lastlanded)
        {
            return new Merchandising.CintOrderTVP
            {
                CashPrice = repoPrice.HasValue ? repoPrice.Value : this.CashPrice,
                RunNo = runno,
                Type = this.Type,
                PrimaryReference = this.PrimaryReference,
                SaleType = this.SaleType,
                SaleLocation = this.SaleLocation,
                Sku = this.Sku,
                ProductId = this.MerchProductId,
                StockLocation = this.StockLocation,
                ParentSku = this.ParentSku,
                ParentId = this.ParentId,
                TransactionDate = this.TransactionDate,
                Quantity = this.Quantity,
                Price = this.Price,
                Tax = Math.Abs(this.Tax.HasValue ? this.Tax.Value : 0M),
                SecondaryReference = this.SecondaryReference,
                ReferenceType = this.ReferenceType,
                Discount = this.Discount,
                PromotionId = this.PromotionId,
                CostPrice = lastlanded,
                TempId = this.TempId
            };
        }
    }
}