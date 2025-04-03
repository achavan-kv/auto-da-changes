/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API 
 */
using STL.Common.Constants.AccountTypes;
using STL.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Customers;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.DbCommands;
using Unicomer.Cosacs.Repository.DbCommands.Customers;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.Implementations
{
    internal class CustomerRepository : ICustomerRepository
    {
        private readonly IDbLogggerRepository dbLoggger;

        public CustomerRepository(IDbLogggerRepository dbLoggger)
        {
            this.dbLoggger = dbLoggger;
        }

        public PreQualificationModel PreQualification(string salesCustomerId, IApiRequest apiRequest)
        {
            return new PreQualificationCommand(salesCustomerId).Execute(apiRequest, dbLoggger);
        }

        public CustomerResultModel Save(CustomerModel customerModel, IApiRequest apiRequest)
        {
            var apiRequestModel = apiRequest.GetRequest();
            var customer = customerModel.CustomerDetail;
            bool isError = false;
            string message = "";

            var result = new CustomerResultModel { SalesCustomerId = customer.CreditCustomerId };

            var country = new CountryCommand().Execute(apiRequest, dbLoggger);

            var isCustomerExist = IsCustomerExist(customer.SalesCustomerId, customer.CreditCustomerId, customer.LineOfCreditId, apiRequest);

            string accountNumber = "";

            try
            {
                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        try
                        {
                            short branch = new BranchCommand(customer.StoreId).Execute(apiRequest, dbLoggger);

                            if (branch <= 0)
                            {
                                throw new ValidationException(new ErrorMessage
                                {
                                    Id = customer.CreditCustomerId,
                                    Detail = "Branch not found for respective storeId-" + customer.StoreId
                                });
                            }

                            if (!isCustomerExist)
                            {
                                accountNumber = GenerateAccountNumber(conn, trans, country.Countrycode, branch, AT.ReadyFinance);

                                if (string.IsNullOrWhiteSpace(accountNumber))
                                {
                                    throw new ValidationException(new ErrorMessage
                                    {
                                        Id = customer.CreditCustomerId,
                                        Detail = "Unable to generate Account number with account type ReadyFinance for store id-" + customer.StoreId
                                    });
                                }
                            }

                            SqlParameter[] xparams = {
                                        new SqlParameter("@CosacsAcctNo", accountNumber),
                                        new SqlParameter("@Branchno", branch),
                                        new SqlParameter("@CreditCustomerId", customer.CreditCustomerId),
                                        new SqlParameter("@LineOfCreditId", customer.LineOfCreditId),
                                        new SqlParameter("@SalesCustomerId", customer.SalesCustomerId),
                                        new SqlParameter("@Nationality", customer.Nationality),
                                        new SqlParameter("@ISOcountryCode", customer.CountryIsoCode),
                                        new SqlParameter("@FirstName", customer.FirstName),
                                        new SqlParameter("@LastName", customer.LastName),
                                        new SqlParameter("@SecondLastName", customer.SecondLastName),
                                        new SqlParameter("@DateOfBirth", customer.DateOfBirth),
                                        new SqlParameter("@StoreId", customer.StoreId),
                                        new SqlParameter("@Gender", customer.Gender),
                                        new SqlParameter("@CountryId", customer.CountryId),
                                        new SqlParameter("@NumberOfChildren", customer.NumberOfChildren),
                                        new SqlParameter("@MaritalStatus", customer.MaritalStatus),
                                        new SqlParameter("@IncomesOther", customer.IncomesOther),
                                        new SqlParameter("@NumberOfDependents", customer.NumberOfDependents),
                                        new SqlParameter("@Salutation", customer.Salutation),

                                        new SqlParameter("@Email",
                                        Core.DataTableExtensions.ToDataTable
                                        (customer.Email,"EmailType"))
                                        {
                                            SqlDbType=SqlDbType.Structured,TypeName = "dbo.EmailType"
                                        },
                                        new SqlParameter("@Identification",
                                        Core.DataTableExtensions.ToDataTable
                                        (customer.Identification,"IdentificationType"))
                                        {
                                            SqlDbType=SqlDbType.Structured,
                                            TypeName = "dbo.IdentificationType"
                                        },
                                        new SqlParameter("@Phone",
                                        Core.DataTableExtensions.ToDataTable
                                        (customer.Phone, "PhoneType"))
                                        {
                                            SqlDbType=SqlDbType.Structured,
                                            TypeName = "dbo.PhoneType"
                                        },

                                        new SqlParameter("@Work",
                                        Core.DataTableExtensions.ToDataTable
                                        (customer.Work, "WorkType"))
                                        {
                                            SqlDbType=SqlDbType.Structured,
                                            TypeName = "dbo.WorkType"
                                        },
                                        new SqlParameter("@PersonalReference",
                                        Core.DataTableExtensions.ToDataTable
                                        (customer.PersonalReference, "PersonalReferenceType"))
                                        {
                                            SqlDbType=SqlDbType.Structured,
                                            TypeName = "dbo.PersonalReferenceType"
                                        },
                                        new SqlParameter("@Address",
                                        Core.DataTableExtensions.ToDataTable
                                        (customer.Address,"AddressType"))
                                        {
                                            SqlDbType=SqlDbType.Structured,
                                            TypeName = "dbo.AddressType"
                                        },
                                        new SqlParameter("@message", SqlDbType.VarChar,200)
                                        {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@error", SqlDbType.Bit)
                                        {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@ISOcustomerId", SqlDbType.VarChar,23)
                                        {
                                            Direction = ParameterDirection.Output
                                        },
                                        new SqlParameter("@SalesCustId", SqlDbType.VarChar,20)
                                        {
                                            Direction = ParameterDirection.Output
                                        }
                                   };

                            SqlCommand command = conn.CreateCommand();
                            command.Transaction = trans;
                            command.CommandText = "dbo.SP_BS_InsertUpdateCustomerDetail";
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddRange(xparams);
                            command.ExecuteNonQuery();

                            if (xparams[25].Value != null && xparams[25].Value != DBNull.Value)
                            {
                                message = xparams[25].Value.ToString();
                            }

                            if (xparams[26].Value != null && xparams[26].Value != DBNull.Value)
                            {
                                isError = (bool)xparams[26].Value;
                            }

                            if (xparams[27].Value != null && xparams[27].Value != DBNull.Value)
                            {
                                result.Id = xparams[27].Value.ToString();
                            }

                            if (xparams[28].Value != null && xparams[28].Value != DBNull.Value)
                            {
                                result.SalesCustomerId = xparams[28].Value.ToString();
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
                        Id = customer.CreditCustomerId,
                        Detail = message
                    },
                    new
                    {
                        customerModel,
                        Sp = "dbo.SP_BS_InsertUpdateCustomerDetail"
                    });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_InsertUpdateCustomerDetail", customerModel, ex.ErrorMessages);

                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_InsertUpdateCustomerDetail", customerModel, ex.Message);
                throw new DBException(ex, new { customerModel, Sp = "dbo.SP_BS_InsertUpdateCustomerDetail" });
            }

            return result;
        }

        public QualificationResponseModel Qualification(string countryIsoCode, string salesCustomerId, IApiRequest apiRequest)
        {
            return new QualificationCommand(countryIsoCode, salesCustomerId).Execute(apiRequest, dbLoggger);
        }

        private string GenerateAccountNumber(SqlConnection conn, SqlTransaction trans, string countryCode, short branchNumber, string accountType)
        {
            return new CosacsAccountNumberGenerateCommand().GenerateAccountNumber(conn, trans, countryCode, branchNumber, accountType);
        }
        private bool IsCustomerExist(string salesCustomerId, string creditCustomerId, string lineOfCreditId, IApiRequest apiRequest)
        {
            return new VerifyCustomerExistCommand(salesCustomerId, creditCustomerId, lineOfCreditId).Execute(apiRequest, dbLoggger);
        }
    }
}
