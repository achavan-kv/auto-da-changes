/* 
Author: suresh-IGT
Date: Feb 15th
Description:JM BlueStart
 */
using STL.DAL;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Deliveries;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.Implementations
{
    internal class ProcessDeliveryRepository : IProcessDeliveryRepository
    {
        private readonly IDbLogggerRepository dbLoggger;

        public ProcessDeliveryRepository(IDbLogggerRepository dbLoggger)
        {
            this.dbLoggger = dbLoggger;
        }

        public DeliveryNotificationModel GetDeliveryNotification(string accountId, string invoiceNumber, IApiRequest apiRequest)
        {
            var apiRequestModel = apiRequest.GetRequest();
            bool isError = false;
            string message = "";
            DeliveryNotificationModel result = new DeliveryNotificationModel
            {
                StageProcess = new DeliveryNotification
                {
                    InvoiceNumber = invoiceNumber,
                    CreditAccountId = accountId,
                }
            };

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
                                        new SqlParameter("@CosacsAccountId", accountId),
                                        new SqlParameter("@InvoiceNumber", invoiceNumber),
                                        new SqlParameter("@CountryISOCode", SqlDbType.VarChar,20) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@CreditCustomerId", SqlDbType.VarChar,50) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@CreditAccountId", SqlDbType.VarChar,50) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@InvoiceDate", SqlDbType.DateTime) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@StatusDelivery", SqlDbType.VarChar,20) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@LineOfCreditId", SqlDbType.VarChar,50) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@Message", SqlDbType.VarChar,100) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@Error", SqlDbType.Bit) {
                                            Direction = ParameterDirection.Output
                                        }
                            };

                            SqlCommand command = conn.CreateCommand();
                            command.Transaction = trans;
                            command.CommandText = "dbo.SP_BS_GetPostProcessDelivery";
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(xparams);
                            command.ExecuteNonQuery();

                            if (xparams[2].Value != null && xparams[2].Value != DBNull.Value)
                            {
                                result.StageProcess.CountryIsoCode = Convert.ToString(xparams[2].Value);
                            }

                            if (xparams[3].Value != null && xparams[3].Value != DBNull.Value)
                            {
                                result.StageProcess.CreditCustomerId = Convert.ToString(xparams[3].Value);
                            }

                            if (xparams[4].Value != null && xparams[4].Value != DBNull.Value)
                            {
                                result.StageProcess.CreditAccountId = Convert.ToString(xparams[4].Value);
                            }

                            if (xparams[5].Value != null && xparams[5].Value != DBNull.Value)
                            {
                                result.StageProcess.InvoiceDate = (Convert.ToDateTime(xparams[5].Value)).ToString("dd-MM-yyyy HH:mm:ss");
                            }

                            if (xparams[6].Value != null && xparams[6].Value != DBNull.Value)
                            {
                                result.StageProcess.Status = Convert.ToString(xparams[6].Value);
                            }

                            if (xparams[7].Value != null && xparams[7].Value != DBNull.Value)
                            {
                                result.StageProcess.LineOfCreditId = Convert.ToString(xparams[7].Value);
                            }

                            if (xparams[8].Value != null && xparams[8].Value != DBNull.Value)
                            {
                                message = xparams[8].Value.ToString();
                            }

                            if (xparams[9].Value != null && xparams[9].Value != DBNull.Value)
                            {
                                isError = (bool)xparams[9].Value;
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
                    throw new ValidationException(new ErrorMessage
                    {
                        Id = accountId,
                        Detail = message
                    },
                    new
                    {
                        accountId,
                        invoiceNumber,
                        Sp = "dbo.SP_BS_GetPostProcessDelivery"
                    });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_GetPostProcessDelivery", new
                {
                    accountId,
                    invoiceNumber
                }, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_GetPostProcessDelivery", new
                {
                    accountId,
                    invoiceNumber
                }, ex.Message);

                throw new DBException(ex, new
                {
                    accountId,
                    invoiceNumber,
                    Sp = "dbo.SP_BS_GetPostProcessDelivery"
                });
            }

            return result;
        }
    }
}
