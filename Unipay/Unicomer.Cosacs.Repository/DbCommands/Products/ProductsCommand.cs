/* 
Author: ArunKarthik-IGT
Date: Feb 15th
Description:JM BlueStart-Get Product Service API
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Products;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.DbCommands.Products
{
    public class ProductsCommand : Blue.Transactions.Command<ContextBase>
    {
        string countryIsoCode;
        string searchKey;
        string storeId;
        string productName;
        string brand;
        string sku;
        string upc;

        public ProductsCommand(string countryIsoCode, string storeId, string productName, string brand, string sku, string upc) : base("dbo.SP_BS_GetProductservices")
        {
            this.countryIsoCode = countryIsoCode;
            this.storeId = storeId;
            this.productName = productName;
            this.brand = brand;
            this.sku = sku;
            this.upc = upc;

            if (!string.IsNullOrEmpty(storeId))
            {
                searchKey = storeId;
            }

            base.AddInParameter("@StoreId", DbType.String);
            base[0] = storeId;
            base.AddInParameter("@ProductName", DbType.String);
            base[1] = productName;
            base.AddInParameter("@Brand", DbType.String);
            base[2] = brand;
            base.AddInParameter("@SKU", DbType.String);
            base[3] = sku;
            base.AddInParameter("@UPC", DbType.String);
            base[4] = upc;
            base.AddInParameter("@CountryISOCode", DbType.String);
            base[5] = countryIsoCode;
            base.AddOutParameter("@message", DbType.String, 100);
            base.AddOutParameter("@error", DbType.Boolean, 1);
        }

        public ProductsResponseModel Execute(IApiRequest apiRequest, IDbLogggerRepository dbLoggger)
        {
            var apiRequestModel = apiRequest.GetRequest();
            string message = "";
            bool isError = false;

            var result = new ProductsResponseModel()
            {
                Items = new ProductItem
                {
                    ItemsDetails = new List<ProductItemsDetail>(),
                    countryIsoCode = this.countryIsoCode,
                }
            };

            try
            {
                var ds = new DataSet();
                base.Fill(ds);

                if (base[6] != null && base[6] != DBNull.Value)
                {
                    message = base[6].ToString();
                }

                if (base[7] != null && base[7] != DBNull.Value)
                {
                    isError = (bool)base[7];
                }

                if (isError)
                {
                    throw new ValidationException(new ErrorMessage { Id = this.searchKey, Detail = message }, new { storeId, productName, brand, sku, upc, Sp = "dbo.SP_BS_GetProductservices" });
                }

                if (ds != null && ds.Tables != null && ds.Tables.Count >= 1 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (var row in ds.Tables[0].Rows.OfType<DataRow>())
                    {
                        var itemsDetail = new ProductItemsDetail() { };

                        if (!row.IsNull("StoreName"))
                        {
                            itemsDetail.StoreName = Convert.ToString(row["StoreName"]);
                        }

                        if (!row.IsNull("StoreId"))
                        {
                            itemsDetail.StoreId = Convert.ToString(row["StoreId"]);
                        }

                        if (!row.IsNull("SKU"))
                        {
                            itemsDetail.Sku = Convert.ToString(row["SKU"]);
                        }

                        if (!row.IsNull("DepartmentName"))
                        {
                            itemsDetail.DepartmentName = Convert.ToString(row["DepartmentName"]);
                        }

                        if (!row.IsNull("UPC"))
                        {
                            itemsDetail.UPC = Convert.ToString(row["UPC"]);
                        }

                        if (!row.IsNull("UPCVendor"))
                        {
                            itemsDetail.UPCVendor = Convert.ToString(row["UPCVendor"]);
                        }

                        if (!row.IsNull("Description"))
                        {
                            itemsDetail.Description = Convert.ToString(row["Description"]);
                        }

                        if (!row.IsNull("Brand"))
                        {
                            itemsDetail.Brand = Convert.ToString(row["Brand"]);
                        }

                        if (!row.IsNull("Style"))
                        {
                            itemsDetail.Style = Convert.ToString(row["Style"]);
                        }

                        if (!row.IsNull("Color"))
                        {
                            itemsDetail.Color = Convert.ToString(row["Color"]);
                        }

                        if (!row.IsNull("QuantityInStock"))
                        {
                            itemsDetail.QuantityInStock = Convert.ToInt32(row["QuantityInStock"]);
                        }

                        if (!row.IsNull("Model"))
                        {
                            itemsDetail.Model = Convert.ToString(row["Model"]);
                        }

                        if (!row.IsNull("Price"))
                        {
                            itemsDetail.Price = Convert.ToDecimal(row["Price"]);
                        }

                        if (!row.IsNull("DiscountProduct"))
                        {
                            itemsDetail.DiscountProduct = Convert.ToDecimal(row["DiscountProduct"]);
                        }

                        if (!row.IsNull("ManufacturerWarranty"))
                        {
                            itemsDetail.ManufacturerWarranty = Convert.ToString(row["ManufacturerWarranty"]);
                        }

                        itemsDetail.Promotion = GetPromotions(itemsDetail.Sku, itemsDetail.StoreId, ds);
                        itemsDetail.Warranty = GetWarranties(itemsDetail.Sku, itemsDetail.StoreId, ds);
                        itemsDetail.Gift = GetGifts(itemsDetail.Sku, itemsDetail.StoreId, ds);
                        itemsDetail.OtherService = GetOtherServices(itemsDetail.Sku, itemsDetail.StoreId, ds);

                        result.Items.ItemsDetails.Add(itemsDetail);
                    }
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_GetProductservices", new { storeId, productName, brand, sku, upc }, ex.ErrorMessages);
                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_GetProductservices", new { storeId, productName, brand, sku, upc }, ex.Message);
                throw new DBException(ex, new { storeId, productName, brand, sku, upc, Sp = "dbo.SP_BS_GetProductservices" });
            }
            return result;
        }

        private List<Promotion> GetPromotions(string sku, string storeId, DataSet ds)
        {
            List<Promotion> promotions = new List<Promotion>();

            if (ds != null && ds.Tables != null && ds.Tables.Count >= 2 && ds.Tables[1].Rows.Count > 0)
            {
                var filterRows = ds.Tables[1].AsEnumerable().Where(row => row.Field<String>("SKU").Trim().Equals(sku) && row.Field<String>("Storeid").Trim().Equals(storeId)).ToArray();

                foreach (var row in filterRows)
                {
                    var promotion = new Promotion();

                    if (!row.IsNull("PromotionCode"))
                    {
                        promotion.PromotionCode = Convert.ToString(row["PromotionCode"]);
                    }

                    if (!row.IsNull("PromotionAmount"))
                    {
                        promotion.PromotionAmount = Convert.ToDecimal(row["PromotionAmount"]);
                    }

                    if (!row.IsNull("PromotionType"))
                    {
                        promotion.PromotionType = Convert.ToString(row["PromotionType"]);
                    }

                    promotions.Add(promotion);
                }
            }

            return promotions;
        }

        private List<Warranty> GetWarranties(string sku, string storeId, DataSet ds)
        {
            List<Warranty> warranties = new List<Warranty>();

            if (ds != null && ds.Tables != null && ds.Tables.Count >= 3 && ds.Tables[2].Rows.Count > 0)
            {
                var filterRows = ds.Tables[2].AsEnumerable().Where(row => row.Field<String>("SKU").Trim().Equals(sku) && row.Field<String>("Storeid").Trim().Equals(storeId)).ToArray();

                foreach (var row in filterRows)
                {
                    var warranty = new Warranty();

                    if (!row.IsNull("WarrantyUPC"))
                    {
                        warranty.UPC = Convert.ToString(row["WarrantyUPC"]);
                    }

                    if (!row.IsNull("WarrantySKU"))
                    {
                        warranty.SKU = Convert.ToString(row["WarrantySKU"]);
                    }

                    if (!row.IsNull("Quantity"))
                    {
                        warranty.Quantity = Convert.ToInt32(row["Quantity"]);
                    }

                    if (!row.IsNull("Period"))
                    {
                        warranty.Period = Convert.ToString(row["Period"]);
                    }

                    if (!row.IsNull("Description"))
                    {
                        warranty.Description = Convert.ToString(row["Description"]);
                    }

                    if (!row.IsNull("TotalAmount"))
                    {
                        warranty.TotalAmount = Convert.ToDecimal(row["TotalAmount"]);
                    }

                    warranties.Add(warranty);
                }
            }

            return warranties;
        }

        private List<Gift> GetGifts(string sku, string storeId, DataSet ds)
        {
            List<Gift> gifts = new List<Gift>();

            if (ds != null && ds.Tables != null && ds.Tables.Count >= 4 && ds.Tables[3].Rows.Count > 0)
            {
                var filterRows = ds.Tables[3].AsEnumerable().Where(row => row.Field<String>("SKU").Trim().Equals(sku) && row.Field<String>("Storeid").Trim().Equals(storeId)).ToArray();

                foreach (var row in filterRows)
                {
                    var gift = new Gift();

                    if (!row.IsNull("GiftSKU"))
                    {
                        gift.Sku = Convert.ToString(row["GiftSKU"]);
                    }

                    if (!row.IsNull("Description"))
                    {
                        gift.Description = Convert.ToString(row["Description"]);
                    }

                    gifts.Add(gift);
                }
            }

            return gifts;
        }

        private List<OtherService> GetOtherServices(string sku, string storeId, DataSet ds)
        {
            List<OtherService> otherServices = new List<OtherService>();

            if (ds != null && ds.Tables != null && ds.Tables.Count >= 5 && ds.Tables[4].Rows.Count > 0)
            {
                var filterRows = ds.Tables[4].AsEnumerable().Where(row => row.Field<String>("SKU").Trim().Equals(sku) && row.Field<String>("Storeid").Trim().Equals(storeId)).ToArray();

                foreach (var row in filterRows)
                {
                    var otherService = new OtherService();

                    if (!row.IsNull("NonStockUPC"))
                    {
                        otherService.UPC = Convert.ToString(row["NonStockUPC"]);
                    }

                    if (!row.IsNull("NonStockSKU"))
                    {
                        otherService.SKU = Convert.ToString(row["NonStockSKU"]);
                    }

                    if (!row.IsNull("Description"))
                    {
                        otherService.Description = Convert.ToString(row["Description"]);
                    }

                    if (!row.IsNull("TotalAmount"))
                    {
                        otherService.TotalAmount = Convert.ToDecimal(row["TotalAmount"]);
                    }

                    otherServices.Add(otherService);
                }
            }

            return otherServices;
        }
    }
}


