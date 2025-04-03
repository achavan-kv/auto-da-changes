/* 
Author:  
Date:  
Description:POST Select Product Service  API 
 */

using STL.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Products;
using Unicomer.Cosacs.Model.Types;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.Implementations
{
    internal class SelectProductRepository : ISelectProductRepository
    {
        private readonly IDbLogggerRepository dbLoggger;

        public SelectProductRepository(IDbLogggerRepository dbLoggger)
        {
            this.dbLoggger = dbLoggger;
        }

        public List<SelectProductResponseModel> Select(ProductServiceModel productServiceModel, IApiRequest apiRequest)
        {
            var apiRequestModel = apiRequest.GetRequest();

            var productService = productServiceModel;
            bool isError = false;
            string message = "";
            var result = new List<SelectProductResponseModel>();

            try
            {
                var productTypes = new List<ProductTableType>();
                MapListTypes(productService, ref productTypes);

                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();

                    SqlParameter[] xparams = {
                                         new SqlParameter("@Product",
                                         Core.DataTableExtensions.ToDataTable(productTypes))
                                         {
                                             SqlDbType=SqlDbType.Structured,
                                             TypeName = "dbo.ProductType"
                                         },
                                          new SqlParameter("@ISOCountryCode", productServiceModel.CountryIsoCode),
                                          new SqlParameter("@message", SqlDbType.VarChar,200) {
                                              Direction = ParameterDirection.Output
                                          },
                                          new SqlParameter("@message", SqlDbType.VarChar,200) {
                                              Direction = ParameterDirection.Output
                                          },
                                          new SqlParameter("@error", SqlDbType.Bit) {
                                              Direction = ParameterDirection.Output
                                          }};

                    SqlCommand command = conn.CreateCommand();
                    command.CommandText = "dbo.SP_BS_PostSelectProductservices";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(xparams);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader[0] != null && reader[0] != DBNull.Value)
                        {
                            result.Add(new SelectProductResponseModel { UPC = reader[0].ToString() });
                        }
                    }

                    if (xparams[2].Value != null && xparams[2].Value != DBNull.Value)
                    {
                        message = xparams[2].Value.ToString();
                    }

                    if (xparams[3].Value != null && xparams[3].Value != DBNull.Value)
                    {
                        isError = (bool)xparams[3].Value;
                    }

                    if (isError)
                    {
                        throw new ValidationException(new ErrorMessage { Id = productServiceModel.CountryIsoCode, Detail = message },
                            new { productServiceModel, Sp = "dbo.SP_BS_PostSelectProductservices" });
                    }

                    return result;
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_PostSelectProductservices", productServiceModel, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_PostSelectProductservices", productServiceModel, ex.Message);
                throw new DBException(ex, new { productServiceModel, Sp = "dbo.SP_BS_PostSelectProductservices" });
            }
        }

        private static void MapListTypes(ProductServiceModel productService, ref List<ProductTableType> productTypes)
        {
            if (productService.Items != null)
            {
                productTypes = productService.Items.Select(t => new ProductTableType
                {
                    StoreId = t.StoreId,
                    SKU = t.Sku,
                    UPC = t.UPC,

                }).ToList();
            }
        }
    }

}
