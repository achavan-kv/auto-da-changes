/* 
Author: suresh-IGT
Date: Feb 15th
Description:JM BlueStart
 */
using STL.BLL;
using STL.Common.Static;
using STL.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Payments;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.Implementations
{
    internal class PaymentRepository : IPaymentRepository
    {
        private readonly IDbLogggerRepository dbLoggger;

        public PaymentRepository(IDbLogggerRepository dbLoggger)
        {
            this.dbLoggger = dbLoggger;
        }

        public PaymentRequestModel GetPaymentRequest(string accountId, string invoiceNumber, IApiRequest apiRequest, out string countryIsoCode)
        {
            var apiRequestModel = apiRequest.GetRequest();
            var paymentRequest = new PaymentRequestModel
            {
                PaymentRequest = new PaymentRequest
                {
                    CellPhone = "",
                    CreditCustomerId = "",
                    Delivery = "",
                    FullName = "",
                    InvoiceDate = "",
                    LineOfCreditId = "",
                    Phone = "",
                    ReferenceinvoiceId = "",
                    SalePerson = "",
                    SalesCustomerId = "",
                    StoreId = "",
                    ReceiptData = new ReceiptData
                    {
                        CreditDetail = new CreditDetail
                        {
                            TypeCreditProduct = "",
                            PurchaseAmount = 0.00M,
                            PurchaseAmountTax = 0.00M,
                            TotalDiscount = 0.00M,
                            DepositAmount = 0.00M,
                            Fee = new System.Collections.Generic.List<Fee>(),
                        },
                        ProductDetail = new System.Collections.Generic.List<ProductDetail>()
                    }
                }
            };
            countryIsoCode = "";
            bool isError = false;
            string message = "";

            try
            {
                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();

                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        DataSet ds = new DataSet();
                        try
                        {
                            SqlParameter[] xparams = {
                                        new SqlParameter("@CosacsAcctNo", accountId),
                                        new SqlParameter("@InvoiceNo", invoiceNumber),
                                        new SqlParameter("@CountryIsoCode", SqlDbType.VarChar,2) {
                                            Direction = ParameterDirection.Output
                                        },
                                         new SqlParameter("@Message", SqlDbType.VarChar,200) {
                                             Direction = ParameterDirection.Output
                                         },
                                        new SqlParameter("@Error", SqlDbType.Bit) {
                                            Direction = ParameterDirection.Output
                                        },
                                    };

                            SqlCommand command = conn.CreateCommand();
                            command.Transaction = trans;
                            command.CommandText = "dbo.SP_BS_GetPaymentRequest";
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(xparams);

                            SqlDataAdapter adapter = new SqlDataAdapter(command);

                            adapter.Fill(ds);

                            if (xparams[2].Value != null && xparams[2].Value != DBNull.Value)
                            {
                                countryIsoCode = xparams[2].Value.ToString();
                            }

                            if (xparams[3].Value != null && xparams[3].Value != DBNull.Value)
                            {
                                message = xparams[3].Value.ToString();
                            }

                            if (xparams[4].Value != null && xparams[4].Value != DBNull.Value)
                            {
                                isError = (bool)xparams[4].Value;
                            }

                            if (isError)
                            {
                                throw new ValidationException(new ErrorMessage { Id = accountId, Detail = message },
                                    new { accountId, invoiceNumber, Sp = "dbo.SP_BS_GetPaymentRequest" });
                            }

                            if (!isError)
                            {
                                trans.Commit();
                                if (ds != null && ds.Tables != null && ds.Tables.Count >= 3)
                                {
                                    PopulateCustomerInvoiceInfo(paymentRequest, ds);
                                    PopulateCreditDetail(paymentRequest, ds);
                                    PopulateProductDetail(paymentRequest, ds);
                                }
                            }
                            else
                            {
                                ds = null;
                                trans.Rollback();
                            }
                        }
                        catch (SqlException ex)
                        {
                            ds = null;
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_GetPaymentRequest", new { accountId, invoiceNumber }, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_GetPaymentRequest", new { accountId, invoiceNumber }, ex.Message);
                throw new DBException(ex, new { accountId, invoiceNumber, Sp = "dbo.SP_BS_GetPaymentRequest" });
            }

            return paymentRequest;
        }

        private static void PopulateCustomerInvoiceInfo(PaymentRequestModel paymentRequest, DataSet ds)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                var rows = ds.Tables[0].AsEnumerable();
                var obj = paymentRequest.PaymentRequest;

                foreach (var row in rows)
                {
                    if (!row.IsNull("CreditCustomerId"))
                    {
                        obj.CreditCustomerId = Convert.ToString(row["CreditCustomerId"]);
                    }

                    if (!row.IsNull("CellPhone"))
                    {
                        obj.CellPhone = Convert.ToString(row["CellPhone"]);
                    }

                    if (!row.IsNull("Delivery"))
                    {
                        obj.Delivery = Convert.ToString(row["Delivery"]);
                    }

                    if (!row.IsNull("FullName"))
                    {
                        obj.FullName = Convert.ToString(row["FullName"]);
                    }

                    if (!row.IsNull("InvoiceDate"))
                    {
                        obj.InvoiceDate = Convert.ToString(row["InvoiceDate"]);
                    }

                    if (!row.IsNull("LineOfCreditId"))
                    {
                        obj.LineOfCreditId = Convert.ToString(row["LineOfCreditId"]);
                    }

                    if (!row.IsNull("Phone"))
                    {
                        obj.Phone = Convert.ToString(row["Phone"]);
                    }

                    if (!row.IsNull("ReferenceinvoiceId"))
                    {
                        obj.ReferenceinvoiceId = Convert.ToString(row["ReferenceinvoiceId"]);
                    }

                    if (!row.IsNull("SalePerson"))
                    {
                        obj.SalePerson = Convert.ToString(row["SalePerson"]);
                    }

                    if (!row.IsNull("SalesCustomerId"))
                    {
                        obj.SalesCustomerId = Convert.ToString(row["SalesCustomerId"]);
                    }

                    if (!row.IsNull("StoreId"))
                    {
                        obj.StoreId = Convert.ToString(row["StoreId"]);
                    }

                    break;
                }
            }
        }

        private static void PopulateCreditDetail(PaymentRequestModel paymentRequest, DataSet ds)
        {
            if (ds.Tables[1].Rows.Count > 0)
            {
                var rows = ds.Tables[1].AsEnumerable();
                var obj = paymentRequest.PaymentRequest.ReceiptData.CreditDetail;

                foreach (var row in rows)
                {
                    if (!row.IsNull("TypeCreditProduct"))
                    {
                        obj.TypeCreditProduct = Convert.ToString(row["TypeCreditProduct"]);
                    }

                    if (!row.IsNull("PurchaseAmount"))
                    {
                        obj.PurchaseAmount = Convert.ToDecimal(row["PurchaseAmount"]);
                    }

                    if (!row.IsNull("PurchaseAmountTax"))
                    {
                        obj.PurchaseAmountTax = Convert.ToDecimal(row["PurchaseAmountTax"]);
                    }

                    if (!row.IsNull("TotalDiscount"))
                    {
                        obj.TotalDiscount = Convert.ToDecimal(row["TotalDiscount"]);
                    }

                    if (!row.IsNull("DepositAmount"))
                    {
                        obj.DepositAmount = Convert.ToDecimal(row["DepositAmount"]);
                    }

                    break;
                }

                foreach (var row in rows)
                {
                    var feeobj = new Fee
                    {
                        DetailFee = new DetailFee
                        {
                            CodeFee = "",
                            DetailFeeTax = new DetailFeeTax
                            {
                                CodeFee = "",
                            }
                        }
                    };

                    if (!row.IsNull("DetailFeeCpi"))
                    {
                        feeobj.DetailFee.CodeFee = Convert.ToString(row["DetailFeeCpi"]);
                    }

                    if (!row.IsNull("DetailFeeTaxCgtCpi"))
                    {
                        feeobj.DetailFee.DetailFeeTax.CodeFee = Convert.ToString(row["DetailFeeTaxCgtCpi"]);
                    }

                    paymentRequest.PaymentRequest.ReceiptData.CreditDetail.Fee.Add(feeobj);

                    break;
                }
            }
        }

        private static void PopulateProductDetail(PaymentRequestModel paymentRequest, DataSet ds)
        {
            if (ds.Tables[2].Rows.Count > 0)
            {
                var rows = ds.Tables[2].AsEnumerable();

                foreach (var row in rows)
                {
                    var obj = new ProductDetail
                    {
                        ProductData = new ProductData
                        {
                            Description = "",
                            UnitDiscount = 0.0M,
                            UnitPrice = 0.0M,
                            PriceTax = 0.0M,
                            Quantity = 0,
                            Sku = "",
                            UPC = ""
                        }
                    };

                    if (!row.IsNull("Description"))
                    {
                        obj.ProductData.Description = Convert.ToString(row["Description"]);
                    }

                    if (!row.IsNull("Discount"))
                    {
                        obj.ProductData.UnitDiscount = Convert.ToDecimal(row["Discount"]);
                    }

                    if (!row.IsNull("Price"))
                    {
                        obj.ProductData.UnitPrice = Convert.ToDecimal(row["Price"]);
                    }

                    if (!row.IsNull("PriceTax"))
                    {
                        obj.ProductData.PriceTax = Convert.ToDecimal(row["PriceTax"]);
                    }

                    if (!row.IsNull("Quantity"))
                    {
                        obj.ProductData.Quantity = Convert.ToInt32(row["Quantity"]);
                    }

                    if (!row.IsNull("Sku"))
                    {
                        obj.ProductData.Sku = Convert.ToString(row["Sku"]);
                    }

                    if (!row.IsNull("UPC"))
                    {
                        obj.ProductData.UPC = Convert.ToString(row["UPC"]);
                    }

                    paymentRequest.PaymentRequest.ReceiptData.ProductDetail.Add(obj);
                }
            }
        }

        public bool UpdatePaymentRequest(UpdatePaymentRequest request, IApiRequest apiRequest)
        {
            var apiRequestModel = apiRequest.GetRequest();
            bool isError = false;
            bool isAutoDA = false;
            string message = "";
            string cosacsAcctNo = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        try
                        {
                            SqlParameter[] xparams = {
                                        new SqlParameter("@countryIsoCode", request.CountryIsoCode),
                                        new SqlParameter("@InvoiceNumber", request.InvoiceNumber),
                                        new SqlParameter("@CreditCustomerId", request.CreditCustomerId),
                                        new SqlParameter("@LineOfCreditId", request.LineOfCreditId),
                                        new SqlParameter("@CreditAccountId", request.CreditAccountId),
                                        new SqlParameter("@SalesCustomerId", request.SalesCustomerId),
                                        new SqlParameter("@TotalLoan", request.TotalLoan),
                                        new SqlParameter("@NumberInstallments", request.NumberInstallments),
                                        new SqlParameter("@InstallmentValue", request.InstallmentValue),
                                        new SqlParameter("@LastInstallmentValue", request.LastInstallmentValue),
                                        new SqlParameter("@PaymentStartDate", request.PaymentStartDate),
                                        new SqlParameter("@AnnualRate", request.AnnualRate),
                                        new SqlParameter("@DetailFee",
                                                Core.DataTableExtensions.ToDataTable
                                                (request.Fee.DetailFee))
                                                {
                                                    SqlDbType=SqlDbType.Structured,
                                                    TypeName = "dbo.FeeType"
                                                },
                                        new SqlParameter("@DetailTaxFee",
                                                Core.DataTableExtensions.ToDataTable
                                                (request.Fee.DetailTaxFee))
                                                {
                                                    SqlDbType=SqlDbType.Structured,
                                                    TypeName = "dbo.FeeType"
                                                },
                                        new SqlParameter("@CosacsAcctNo", SqlDbType.VarChar,12)
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
                            command.CommandText = "dbo.SP_BS_UpdatePaymentRequest";
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(xparams);
                            command.ExecuteNonQuery();

                            if (xparams[14].Value != null && xparams[14].Value != DBNull.Value)
                            {
                                cosacsAcctNo = xparams[14].Value.ToString();
                            }

                            if (xparams[15].Value != null && xparams[15].Value != DBNull.Value)
                            {
                                message = xparams[15].Value.ToString();
                            }

                            if (xparams[16].Value != null && xparams[16].Value != DBNull.Value)
                            {
                                isError = (bool)xparams[16].Value;
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
                    if (!isError)
                    {
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            try
                            {
                                SqlParameter[] xparams = {
                                        new SqlParameter("@acctno", cosacsAcctNo),
                                        new SqlParameter("@IsAutoDA", SqlDbType.Bit)
                                        {
                                            Direction = ParameterDirection.Output
                                        }
                                };

                                SqlCommand command = conn.CreateCommand();
                                command.Transaction = trans;
                                command.CommandText = "dbo.CheckAccountForAutoDA";
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddRange(xparams);
                                command.ExecuteNonQuery();

                                if (xparams[1].Value != null && xparams[1].Value != DBNull.Value)
                                {
                                    isAutoDA = (bool)xparams[1].Value;
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }

                        if (isAutoDA)
                        {
                            using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                            {
                                try
                                {
                                    BAgreement agreement = new BAgreement();
                                    agreement.User = Credential.UserId;
                                    agreement.ClearProposal(conn, trans, cosacsAcctNo, "AUTO");
                                    trans.Commit();
                                }
                                catch (SqlException ex)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                }

                if (isError)
                {
                    throw new ValidationException(new ErrorMessage
                    {
                        Id = request.CreditCustomerId,
                        Detail = message
                    },
                    new
                    {
                        request,
                        Sp = "dbo.SP_BS_UpdatePaymentRequest"
                    });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_UpdatePaymentRequest", request, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_UpdatePaymentRequest", request, ex.Message);
                throw new DBException(ex, new { request, Sp = "dbo.SP_BS_UpdatePaymentRequest" });
            }

            return !isError;
        }
    }
}
