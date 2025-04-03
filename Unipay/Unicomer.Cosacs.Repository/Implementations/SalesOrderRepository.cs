using STL.Common.Constants.AccountTypes;
using STL.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Orders;
using Unicomer.Cosacs.Model.Types;
using Unicomer.Cosacs.Repository.DbCommands;
using Unicomer.Cosacs.Repository.DbCommands.Customers;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.Implementations
{
    internal class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly IDbLogggerRepository dbLoggger;

        public SalesOrderRepository(IDbLogggerRepository dbLoggger)
        {
            this.dbLoggger = dbLoggger;
        }

        public SalesOrderResponseModel Save(SalesOrderModel salesorderModel, IApiRequest apiRequest)
        {
            var apiRequestModel = apiRequest.GetRequest();
            var salesorder = salesorderModel;
            bool isError = false;
            string message = "";
            string accountNumber = "";

            var result = new SalesOrderResponseModel
            {
                SalesOrderId = salesorder.SalesOrderId,
                CreditAccountId = salesorder.ReceiptData.CreditDetail.CreditAccountId,
                CreditCustomerId = salesorder.CreditCustomerId,
                LineOfCreditId = salesorder.LineOfCreditId,
                Status = "invoiced"
            };

            try
            {
                DateTime invoiceDate = DateTime.Now;

                if (string.IsNullOrWhiteSpace(salesorderModel.InvoiceDate) || !DateTime.TryParseExact(salesorderModel.InvoiceDate.Trim(), new string[] { "dd-MM-yyyy HH:mm:ss", "dd-MM-yyyy HH:mm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out invoiceDate))
                {
                    throw new ValidationException(new ErrorMessage { Id = salesorder.SalesCustomerId, Detail = "InvoiceDate is not correct format(dd-MM-yyyy HH:mm:ss)" },
                        new { salesorderModel });
                }

                var productDetailTypes = MapListTypes(salesorder);

                short branch = new BranchCommand(salesorder.StoreId).Execute(apiRequest, dbLoggger);
                var country = new CountryCommand().Execute(apiRequest, dbLoggger);

                if (salesorder.ReceiptData.CreditDetail.Fee == null)
                {
                    salesorder.ReceiptData.CreditDetail.Fee = new CreditFee();
                }

                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        try
                        {
                            accountNumber = GenerateAccountNumber(conn, trans, country.Countrycode, branch, AT.ReadyFinance);

                            if (string.IsNullOrWhiteSpace(accountNumber))
                            {
                                throw new ValidationException(new ErrorMessage 
                                { 
                                    Id = salesorder.SalesOrderId, 
                                    Detail = "Unable to generate Account number with account type ReadyFinance for store id-" + salesorder.StoreId 
                                });
                            }

                            SqlParameter[] xparams = {
                                new SqlParameter("@CosacsAccountId", accountNumber),
                                new SqlParameter("@SalesOrderId", salesorder.SalesOrderId),
                                new SqlParameter("@StoreId", salesorder.StoreId),
                                new SqlParameter("@InvoiceDate", invoiceDate),
                                new SqlParameter("@QuantityTotal", salesorder.Quantity),
                                new SqlParameter("@Delivery", salesorder.Delivery??false),
                                new SqlParameter("@SalesCustomerId", salesorder.SalesCustomerId),
                                new SqlParameter("@DocumentNumber", salesorder.DocumentNumber),
                                new SqlParameter("@FullName", salesorder.FullName),
                                new SqlParameter("@Phone", salesorder.Phone),
                                new SqlParameter("@CellPhone", salesorder.CellPhone),
                                new SqlParameter("@CreditCustomerId", salesorder.CreditCustomerId),
                                new SqlParameter("@LineOfCreditId", salesorder.LineOfCreditId),

                                //Credit Detail
                                new SqlParameter("@CreditAccountId", 
                                                salesorder.ReceiptData.CreditDetail.CreditAccountId),
                                new SqlParameter("@TypeCreditProduct",
                                                salesorder.ReceiptData.CreditDetail.TypeCreditProduct),
                                new SqlParameter("@TotalLoan", 
                                                salesorder.ReceiptData.CreditDetail.TotalLoan),
                                new SqlParameter("@NumberInstalments",
                                                salesorder.ReceiptData.CreditDetail.NumberInstallments),
                                new SqlParameter("@InstalmentValue",
                                                salesorder.ReceiptData.CreditDetail.InstallmentValue),
                                new SqlParameter("@LastInstallmentValue",
                                                salesorder.ReceiptData.CreditDetail.LastInstallmentValue),
                                new SqlParameter("@PaymentStartDate", 
                                                salesorder.ReceiptData.CreditDetail.PaymentStartDate),
                                new SqlParameter("@AnnualRate", 
                                                salesorder.ReceiptData.CreditDetail.AnnualRate),

                                //Delivery Address
                                new SqlParameter("@Region",
                                                salesorder.ReceiptData.CreditDetail.DeliveryAddress.Region),
                                new SqlParameter("@State",
                                                salesorder.ReceiptData.CreditDetail.DeliveryAddress.State),
                                new SqlParameter("@City",
                                                salesorder.ReceiptData.CreditDetail.DeliveryAddress.City),
                                new SqlParameter("@DeliveryArea", 
                                                salesorder.ReceiptData.CreditDetail.DeliveryAddress.DeliveryArea),
                                new SqlParameter("@Addressline",
                                                salesorder.ReceiptData.CreditDetail.DeliveryAddress.AddressLine),
                                new SqlParameter("@ZipCode",
                                                salesorder.ReceiptData.CreditDetail.DeliveryAddress.ZipCode),
                                new SqlParameter("@TitleDelivery",
                            salesorder.ReceiptData.CreditDetail.DeliveryAddress.DeliveryContact.TitleDelivery),
                                new SqlParameter("@FirstName",
                                salesorder.ReceiptData.CreditDetail.DeliveryAddress.DeliveryContact.FirstName),
                                new SqlParameter("@LastName",
                                    salesorder.ReceiptData.CreditDetail.DeliveryAddress.DeliveryContact.LastName),
                                new SqlParameter("@CellPhone1", 
                                salesorder.ReceiptData.CreditDetail.DeliveryAddress.DeliveryContact.Cellphone),
                                new SqlParameter("@SalePerson", salesorder.SalePerson),

                                new SqlParameter("@DetailFee",
                                                Core.DataTableExtensions.ToDataTable
                                                (salesorder.ReceiptData.CreditDetail.Fee.DetailFee))
                                                {
                                                    SqlDbType=SqlDbType.Structured,
                                                    TypeName = "dbo.FeeType"
                                                },
                                new SqlParameter("@DetailTaxFee", 
                                                Core.DataTableExtensions.ToDataTable
                                                (salesorder.ReceiptData.CreditDetail.Fee.DetailTaxFee))
                                                {
                                                    SqlDbType=SqlDbType.Structured,
                                                    TypeName = "dbo.FeeType"
                                                },
                                new SqlParameter("@product", 
                                                Core.DataTableExtensions.ToDataTable(productDetailTypes))
                                                {
                                                    SqlDbType=SqlDbType.Structured,
                                                    TypeName = "dbo.ProductDetailType"
                                                },

                                new SqlParameter("@CountryISOCode", SqlDbType.VarChar,20) 
                                {
                                    Direction = ParameterDirection.Output
                                },
                                new SqlParameter("@CreditCustomerIdOut", SqlDbType.VarChar,50)
                                {
                                    Direction = ParameterDirection.Output
                                },
                                new SqlParameter("@InvoiceNumber", SqlDbType.VarChar,20)
                                {
                                    Direction = ParameterDirection.Output
                                },
                                new SqlParameter("@Message", SqlDbType.VarChar,200)
                                {
                                    Direction = ParameterDirection.Output
                                },
                                new SqlParameter("@Error", SqlDbType.Bit)
                                {
                                    Direction = ParameterDirection.Output
                                }
                            };

                            SqlCommand command = conn.CreateCommand();
                            command.Transaction = trans;
                            command.CommandText = "dbo.BS_SP_PostSalesOrder";
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(xparams);
                            command.ExecuteNonQuery();

                            if (xparams[35].Value != null && xparams[35].Value != DBNull.Value)
                            {
                                result.CountryIsoCode = xparams[35].Value.ToString();
                            }

                            if (xparams[37].Value != null && xparams[37].Value != DBNull.Value)
                            {
                                result.InvoiceNumber = xparams[37].Value.ToString();
                            }

                            if (xparams[38].Value != null && xparams[38].Value != DBNull.Value)
                            {
                                message = xparams[38].Value.ToString();
                            }

                            if (xparams[39].Value != null && xparams[39].Value != DBNull.Value)
                            {
                                isError = (bool)xparams[39].Value;
                            }

                            if (!isError)
                            {
                                trans.Commit();
                            }
                            else
                            {
                                trans.Rollback();
                            }

                        }
                        catch (SqlException ex)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }

                if (isError)
                {
                    throw new ValidationException(
                        new ErrorMessage { 
                            Id = salesorder.SalesCustomerId, 
                            Detail = message 
                        },
                        new { 
                            salesorderModel, 
                            Sp = "dbo.BS_SP_PostSalesOrder" 
                        });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.BS_SP_PostSalesOrder", salesorderModel, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.BS_SP_PostSalesOrder", salesorderModel, ex.Message);
                throw new DBException(ex, new { 
                                                salesorderModel, 
                                                Sp = "dbo.BS_SP_PostSalesOrder" 
                                            });
            }
            return result;
        }


        private string GenerateAccountNumber(SqlConnection conn, SqlTransaction trans, string countryCode, short branchNumber, string accountType)
        {
            return new CosacsAccountNumberGenerateCommand().GenerateAccountNumber(conn, trans, countryCode, branchNumber, accountType);
        }

        private static List<ProductDetailType> MapListTypes(SalesOrderModel salesOrder)
        {
            List<ProductDetailType> productDetailTypes = new List<ProductDetailType>();

            if (salesOrder.ReceiptData != null)
            {
                productDetailTypes = salesOrder.ReceiptData.CreditDetail.ProductDetail.Select(t => new ProductDetailType
                {
                    UPC = t.ProductData.UPC,
                    SKU = t.ProductData.SKU,
                    UPCVendor = t.ProductData.UPCVendor,
                    Quantity = t.ProductData.Quantity,

                }).ToList();
            }

            return productDetailTypes;
        }
    }
}
