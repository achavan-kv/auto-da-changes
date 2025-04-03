using STL.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.Implementations
{
    public class RepaymentRepository : IRepaymentRepository
    {
        private readonly IDbLogggerRepository dbLoggger;

        public RepaymentRepository(IDbLogggerRepository dbLoggger)
        {
            this.dbLoggger = dbLoggger;
        }

        public bool SaveRepayment(string mambuAccountid, decimal amount, string transactionId, IApiRequest apiRequest)
        {
            var apiRequestModel = apiRequest.GetRequest();
            bool isError = false;
            string message = "";

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
                                                new SqlParameter("@Amount", amount),
                                                new SqlParameter("@MamboTransId", transactionId),
                                                new SqlParameter("@MamboLoanAccountId", mambuAccountid),
                                                new SqlParameter("@Message", SqlDbType.VarChar,200) {
                                                    Direction = ParameterDirection.Output
                                                },
                                                new SqlParameter("@Error", SqlDbType.Bit) {
                                                    Direction = ParameterDirection.Output
                                                }
                                            };

                            SqlCommand command = conn.CreateCommand();
                            command.Transaction = trans;
                            command.CommandText = "dbo.SP_BS_PostMakeRepaymentTransaction";
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(xparams);
                            command.ExecuteNonQuery();

                            if (xparams[3].Value != null && xparams[3].Value != DBNull.Value)
                            {
                                message = xparams[3].Value.ToString();
                            }

                            if (xparams[4].Value != null && xparams[4].Value != DBNull.Value)
                            {
                                isError = (bool)xparams[4].Value;
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
                    throw new ValidationException(new ErrorMessage { Id = mambuAccountid, Detail = message },
                        new { mambuAccountid, amount, transactionId, Sp = "dbo.SP_BS_PostMakeRepaymentTransaction" });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_PostMakeRepaymentTransaction", new { mambuAccountid, amount, transactionId }, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_PostMakeRepaymentTransaction", new { mambuAccountid, amount, transactionId }, ex.Message);
                throw new DBException(ex, new { mambuAccountid, amount, transactionId, Sp = "dbo.SP_BS_PostMakeRepaymentTransaction" });
            }

            return true;
        }
    }
}
