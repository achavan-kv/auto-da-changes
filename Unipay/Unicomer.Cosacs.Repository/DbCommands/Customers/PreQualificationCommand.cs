/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API 
 */
using System;
using System.Data;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Customers;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.DbCommands.Customers
{
    internal class PreQualificationCommand : Blue.Transactions.Command<ContextBase>
    {
        string salesCustomerId;
        public PreQualificationCommand(string salesCustomerId) : base("dbo.SP_JMBS_GetCustomerStatus")
        {
            this.salesCustomerId = salesCustomerId;
            base.AddInParameter("@custid", DbType.String);
            base.AddOutParameter("@countryCode", DbType.String, 2);
            base.AddOutParameter("@arrearsCustomer", DbType.String, 5);
            base.AddOutParameter("@message", DbType.String, 100);
            base.AddOutParameter("@error", DbType.Boolean, 1);

            base[0] = salesCustomerId;
        }

        public PreQualificationModel Execute(IApiRequest apiRequest, IDbLogggerRepository dbLoggger)
        {
            var apiRequestModel = apiRequest.GetRequest();

            string message = "";
            bool isError = false;
            var result = new PreQualificationModel { SalesCustomerId = this.salesCustomerId };

            try
            {
                base.ExecuteNonQuery();

                if (base[1] != null && base[1] != DBNull.Value)
                {
                    result.CountryIsoCode = base[1].ToString();
                }

                if (base[2] != null && base[2] != DBNull.Value)
                {
                    result.ArrearsCustomer = base[2].ToString();
                }

                if (base[3] != null && base[3] != DBNull.Value)
                {
                    message = base[3].ToString();
                }

                if (base[4] != null && base[4] != DBNull.Value)
                {
                    isError = (bool)base[4];
                }

                if (isError)
                {
                    throw new ValidationException(new ErrorMessage { Id = this.salesCustomerId, Detail = message },
                        new { this.salesCustomerId, Sp = "dbo.SP_JMBS_GetCustomerStatus" });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_JMBS_GetCustomerStatus", salesCustomerId, ex.ErrorMessages);
                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_JMBS_GetCustomerStatus", salesCustomerId, ex.Message);
                throw new DBException(ex, new { this.salesCustomerId, Sp = "dbo.SP_JMBS_GetCustomerStatus" });
            }

            return result;
        }
    }
}
