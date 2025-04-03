/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API 
 */
using STL.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Model.Models.Loans;
using Unicomer.Cosacs.Model.Models.Loans.Schedule;
using Unicomer.Cosacs.Model.Types;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.Implementations
{
    internal class AccountRepository : IAccountRepository
    {
        private readonly IDbLogggerRepository dbLoggger;

        public AccountRepository(IDbLogggerRepository dbLoggger)
        {
            this.dbLoggger = dbLoggger;
        }

        public MambuAccountModel GetMambuAccountId(string cosacsAccountId, string apiType, IApiRequest apiRequest)
        {
            const string StoreProcedureName = "dbo.SP_BS_GetAccountMapping";
            var apiRequestModel = apiRequest.GetRequest();
            bool isError = false;
            string message = "";
            var mambuAcount = new MambuAccountModel();

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
                                        new SqlParameter("@CosacsAcctNo", cosacsAccountId),
                                        new SqlParameter("@APIType", apiType),

                                        new SqlParameter("@LoanAcctNo", SqlDbType.VarChar,50) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@TransactionChannelId", SqlDbType.VarChar,50) {
                                            Direction = ParameterDirection.Output
                                        },
                                         new SqlParameter("@Notes", SqlDbType.VarChar,100) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@Message", SqlDbType.VarChar,100) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@Error", SqlDbType.Bit) {
                                            Direction = ParameterDirection.Output
                                        }};

                            SqlCommand command = conn.CreateCommand();
                            command.Transaction = trans;
                            command.CommandText = StoreProcedureName;
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(xparams);
                            command.ExecuteNonQuery();

                            if (xparams[2].Value != null && xparams[2].Value != DBNull.Value)
                            {
                                mambuAcount.MambuAccountId = xparams[2].Value.ToString();
                            }

                            if (xparams[3].Value != null && xparams[3].Value != DBNull.Value)
                            {
                                mambuAcount.TransactionChannelId = xparams[3].Value.ToString();
                            }

                            if (xparams[4].Value != null && xparams[4].Value != DBNull.Value)
                            {
                                mambuAcount.Notes = xparams[4].Value.ToString();
                            }

                            if (xparams[5].Value != null && xparams[5].Value != DBNull.Value)
                            {
                                message = xparams[5].Value.ToString();
                            }

                            if (xparams[6].Value != null && xparams[6].Value != DBNull.Value)
                            {
                                isError = (bool)xparams[6].Value;
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
                    throw new ValidationException(new ErrorMessage { Id = cosacsAccountId, Detail = message }, new { cosacsAccountId, Sp = StoreProcedureName });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, StoreProcedureName, cosacsAccountId, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, StoreProcedureName, cosacsAccountId, ex.Message);

                throw new DBException(ex, new { cosacsAccountId, Sp = StoreProcedureName });
            }

            return mambuAcount;
        }

        public decimal GetPayOffAmount(string cosacsAccountId, IApiRequest apiRequest)
        {
            const string StoreProcedureName = "dbo.SP_BS_GetPayOffAmount";

            var apiRequestModel = apiRequest.GetRequest();
            bool isError = false;
            string message = "";
            decimal amount = 0;

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
                                        new SqlParameter("@CosacsAcctNo", cosacsAccountId),
                                        new SqlParameter("@Amount", SqlDbType.Money) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@Message", SqlDbType.VarChar,200) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@Error", SqlDbType.Bit) {
                                            Direction = ParameterDirection.Output
                                        }};

                            SqlCommand command = conn.CreateCommand();
                            command.Transaction = trans;
                            command.CommandText = StoreProcedureName;
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(xparams);
                            command.ExecuteNonQuery();

                            if (xparams[1].Value != null && xparams[1].Value != DBNull.Value)
                            {
                                amount = (decimal)xparams[1].Value;
                            }

                            if (xparams[2].Value != null && xparams[2].Value != DBNull.Value)
                            {
                                message = xparams[2].Value.ToString();
                            }

                            if (xparams[3].Value != null && xparams[3].Value != DBNull.Value)
                            {
                                isError = (bool)xparams[3].Value;
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
                    throw new ValidationException(new ErrorMessage { Id = cosacsAccountId, Detail = message }, new { cosacsAccountId, Sp = StoreProcedureName });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, StoreProcedureName, cosacsAccountId, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, StoreProcedureName, cosacsAccountId, ex.Message);

                throw new DBException(ex, new { cosacsAccountId, Sp = StoreProcedureName });
            }

            return amount;
        }

        public string GetMambuAccountType(string cosacsAccountId, IApiRequest apiRequest)
        {
            const string StoreProcedureName = "dbo.SP_BS_GetAccountType";
            var apiRequestModel = apiRequest.GetRequest();
            bool isError = false;
            string message = "";
            string result = "";

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
                                        new SqlParameter("@CosacsAccountId", cosacsAccountId),
                                        new SqlParameter("@HPOrCashLoan", SqlDbType.VarChar,50) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@Message", SqlDbType.VarChar,200) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@Error", SqlDbType.Bit) {
                                            Direction = ParameterDirection.Output
                                        }};

                            SqlCommand command = conn.CreateCommand();
                            command.Transaction = trans;
                            command.CommandText = StoreProcedureName;
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(xparams);
                            command.ExecuteNonQuery();

                            if (xparams[1].Value != null && xparams[1].Value != DBNull.Value)
                            {
                                result = xparams[1].Value.ToString();
                            }

                            if (xparams[2].Value != null && xparams[2].Value != DBNull.Value)
                            {
                                message = xparams[2].Value.ToString();
                            }

                            if (xparams[3].Value != null && xparams[3].Value != DBNull.Value)
                            {
                                isError = (bool)xparams[3].Value;
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
                    throw new ValidationException(new ErrorMessage { Id = cosacsAccountId, Detail = message }, new { cosacsAccountId, Sp = StoreProcedureName });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, StoreProcedureName, cosacsAccountId, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, StoreProcedureName, cosacsAccountId, ex.Message);

                throw new DBException(ex, new { cosacsAccountId, Sp = StoreProcedureName });
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                return "";
            }
            else if (result.Equals("HP", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return ApplicationSettingManager.XtypeProductHP;
            }
            else
            {
                return ApplicationSettingManager.XtypeProductCL;
            }
        }

        public bool LoanAdjust(decimal amount, string transactionId, string loanAccountId, string cosacsAccountId, IApiRequest apiRequest)
        {
            const string StoreProcedureName = "dbo.SP_BS_PostReversePayment";
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
                                        new SqlParameter("@MamboLoanAccountId", loanAccountId),
                                        new SqlParameter("@Message", SqlDbType.VarChar,100) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@Error", SqlDbType.Bit) {
                                            Direction = ParameterDirection.Output
                                        }
                                    };

                            SqlCommand command = conn.CreateCommand();
                            command.Transaction = trans;
                            command.CommandText = StoreProcedureName;
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
                    throw new ValidationException(new ErrorMessage { Id = cosacsAccountId, Detail = message },
                        new { amount, transactionId, loanAccountId, cosacsAccountId, Sp = StoreProcedureName });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, StoreProcedureName, new { amount, transactionId, loanAccountId, cosacsAccountId }, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, StoreProcedureName, new { amount, transactionId, loanAccountId, cosacsAccountId }, ex.Message);
                throw new DBException(ex, new { amount, transactionId, loanAccountId, cosacsAccountId, Sp = StoreProcedureName });
            }

            return !isError;
        }

        public bool GetLoanDueDetail(string cosacsAccountId, LoanScheduleModel loanSchedule, IApiRequest apiRequest)
        {
            const string StoreProcedureName = "dbo.SP_BS_GetLoanSchedule";
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
                                        new SqlParameter("@CosacsAccountId", cosacsAccountId),
                                         new SqlParameter("@AccountDueDetails",
                                         Core.DataTableExtensions.ToDataTable(MapListTypes(loanSchedule)))
                                        {
                                             SqlDbType=SqlDbType.Structured,
                                             TypeName = "dbo.AccountDueDetails"
                                         },
                                        new SqlParameter("@Message", SqlDbType.VarChar,100) {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@Error", SqlDbType.Bit) {
                                            Direction = ParameterDirection.Output
                                        }};

                            SqlCommand command = conn.CreateCommand();
                            command.Transaction = trans;
                            command.CommandText = StoreProcedureName;
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(xparams);
                            command.ExecuteNonQuery();

                            if (xparams[2].Value != null && xparams[2].Value != DBNull.Value)
                            {
                                message = xparams[2].Value.ToString();
                            }

                            if (xparams[3].Value != null && xparams[3].Value != DBNull.Value)
                            {
                                isError = (bool)(xparams[3]).Value;
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
                    throw new ValidationException(new ErrorMessage { Id = cosacsAccountId, Detail = message },
                        new { cosacsAccountId, loanSchedule, Sp = StoreProcedureName });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, StoreProcedureName, new { cosacsAccountId, loanSchedule }, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, StoreProcedureName, new { cosacsAccountId, loanSchedule }, ex.Message);
                throw new DBException(ex, new { cosacsAccountId, loanSchedule, Sp = StoreProcedureName });
            }

            return !isError;
        }

        public bool UpdateAccountDetail(string cosacsAccountId, decimal earlySettlement, decimal rebate, string accountState, string accountType, IApiRequest apiRequest)
        {
            const string StoreProcedureName = "dbo.SP_BS_UpdateAccountDetail";
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
                                new SqlParameter("@AccountId", cosacsAccountId),
                                new SqlParameter("@EarlySettlement",earlySettlement),
                                new SqlParameter("@Rebate", rebate),
                                new SqlParameter("@AccountState", accountState),
                                new SqlParameter("@AccountType", accountType),
                                new SqlParameter("@Message", SqlDbType.VarChar,100) {
                                    Direction = ParameterDirection.Output
                                },
                                new SqlParameter("@Error", SqlDbType.Bit) {
                                    Direction = ParameterDirection.Output
                                }};

                            SqlCommand command = conn.CreateCommand();
                            command.Transaction = trans;
                            command.CommandText = StoreProcedureName;
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(xparams);
                            command.ExecuteNonQuery();

                            if (xparams[5].Value != null && xparams[5].Value != DBNull.Value)
                            {
                                message = xparams[5].Value.ToString();
                            }

                            if (xparams[6].Value != null && xparams[6].Value != DBNull.Value)
                            {
                                isError = (bool)xparams[6].Value;
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
                    throw new ValidationException(new ErrorMessage { Id = cosacsAccountId, Detail = message },
                        new { cosacsAccountId, earlySettlement, rebate, accountState, accountType, Sp = StoreProcedureName });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, StoreProcedureName, new { cosacsAccountId, earlySettlement, rebate, accountState, accountType }, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, StoreProcedureName, new { cosacsAccountId, earlySettlement, rebate, accountState, accountType }, ex.Message);
                throw new DBException(ex, new { cosacsAccountId, earlySettlement, rebate, accountState, accountType, Sp = StoreProcedureName });
            }

            return !isError;
        }

        private List<AccountDueDetailType> MapListTypes(LoanScheduleModel loanSchedule)
        {
            var result = new List<AccountDueDetailType>();

            if (loanSchedule != null && loanSchedule.Installments != null)
            {
                foreach (var installment in loanSchedule.Installments)
                {
                    if (installment != null)
                    {
                        decimal taxDue = 0;
                        decimal taxExpected = 0;
                        decimal taxPaid = 0;

                        var accountDueDetailType = new AccountDueDetailType
                        {
                            Duedate = installment.DueDate,
                            State = installment.State
                        };

                        if (installment.Fee != null && installment.Fee.Amount != null)
                        {
                            accountDueDetailType.FeesDue = installment.Fee.Amount.Due;
                            accountDueDetailType.FeesExpected = installment.Fee.Amount.Expected;
                            accountDueDetailType.FeesPaid = installment.Fee.Amount.Paid;
                        }

                        if (installment.Interest != null && installment.Interest.Amount != null)
                        {
                            accountDueDetailType.InterestDue = installment.Interest.Amount.Due;
                            accountDueDetailType.InterestExpected = installment.Interest.Amount.Expected;
                            accountDueDetailType.InterestPaid = installment.Interest.Amount.Paid;
                        }

                        if (installment.Penalty != null && installment.Penalty.Amount != null)
                        {
                            accountDueDetailType.PenaltyDue = installment.Penalty.Amount.Due;
                            accountDueDetailType.PenaltyExpected = installment.Penalty.Amount.Expected;
                            accountDueDetailType.PenaltyPaid = installment.Penalty.Amount.Paid;
                        }

                        if (installment.Principal != null && installment.Principal.Amount != null)
                        {
                            accountDueDetailType.PrincipalDue = installment.Principal.Amount.Due;
                            accountDueDetailType.PrincipalExpected = installment.Principal.Amount.Expected;
                            accountDueDetailType.PrincipalPaid = installment.Principal.Amount.Paid;
                        }

                        if (installment.Fee != null && installment.Fee.Tax != null)
                        {
                            taxDue += installment.Fee.Tax.Due;
                            taxExpected += installment.Fee.Tax.Expected;
                            taxPaid += installment.Fee.Tax.Paid;
                        }

                        if (installment.Interest != null && installment.Interest.Tax != null)
                        {
                            taxDue += installment.Interest.Tax.Due;
                            taxExpected += installment.Interest.Tax.Expected;
                            taxPaid += installment.Interest.Tax.Paid;
                        }

                        if (installment.Penalty != null && installment.Penalty.Tax != null)
                        {
                            taxDue += installment.Penalty.Tax.Due;
                            taxExpected += installment.Penalty.Tax.Expected;
                            taxPaid += installment.Penalty.Tax.Paid;
                        }

                        accountDueDetailType.TaxDue = taxDue;
                        accountDueDetailType.TaxExpected = taxExpected;
                        accountDueDetailType.TaxPaid = taxPaid;
                        result.Add(accountDueDetailType);
                    }
                }
            }

            return result;
        }
    }
}
