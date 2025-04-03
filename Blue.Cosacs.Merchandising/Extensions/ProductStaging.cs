using Newtonsoft.Json;
using System;

namespace Blue.Cosacs.Merchandising
{
    public partial class ProductStaging
    {
        public ProductStaging()
        {
        }

        public ProductStaging(Models.Product product)
        {
            this.SKU = product.SKU;
            this.LongDescription = product.LongDescription;
            this.ProductType = product.ProductType;
            this.Tags = null;
            this.StoreTypes = null;
            this.POSDescription = product.POSDescription;
            this.Attributes = JsonConvert.SerializeObject(product.Attributes);
            this.CreatedDate = product.CreatedDate.HasValue ? product.CreatedDate.Value : DateTime.UtcNow;
            this.LastUpdatedDate = product.LastUpdatedDate.HasValue ? product.LastUpdatedDate.Value : DateTime.UtcNow;
            this.Status = product.Status;
            this.PriceTicket = product.PriceTicket;
            this.SKUStatus = product.SKUStatus;
            this.CorporateUPC = product.CorporateUPC;
            this.VendorUPC = product.VendorUPC;
            this.VendorStyleLong = product.VendorStyleLong;
            this.CountryOfOrigin = product.CountryOfOrigin;
            this.VendorWarranty = product.VendorWarranty;
            this.ReplacingTo = product.ReplacingTo;
            this.Features = JsonConvert.SerializeObject(product.Features);
         //   this.BrandId = product.BrandId;  --- in sp
            this.PrimaryVendorId = product.PrimaryVendorId;
            this.LastStatusChangeDate = DateTime.UtcNow;
            this.OnlineDateAdded = product.OnlineDateAdded;
            this.LabelRequired = product.LabelRequired.HasValue ? product.LabelRequired.Value : false;
            this.BoxCount = product.BoxCount.HasValue ? product.BoxCount.Value : 0;
            this.ProductAction = product.ProductAction;
            this.CreatedById = product.User;
            this.ExternalCreationDate = product.ExternalCreationDate;
            this.MagentoExportType = product.MagentoExportType;
            this.BrandName = product.BrandName;
            this.BrandCode = product.BrandCode;
        }
    }
}
