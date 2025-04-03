/* 
Author: Swati -IGT
Date: Feb 10 2022
Description:JM BlueStart-Qualification API 
 */
using System;
using System.Data;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.DbCommands.Customers
{
    public class VerifyCustomerExistCommand : Blue.Transactions.Command<ContextBase>
    {
        string salesCustomerId;
        string creditCustomerId;
        string lineOfCreditId;
        public VerifyCustomerExistCommand(string salesCustomerId, string creditCustomerId, string lineOfCreditId) : base("dbo.BS_SP_VerifyCustomerExist")
        {
            this.salesCustomerId = salesCustomerId;
            this.creditCustomerId = creditCustomerId;
            this.lineOfCreditId = lineOfCreditId;

            base.AddInParameter("@custId", DbType.String);
            base.AddInParameter("@CreditCustomerId", DbType.String);
            base.AddInParameter("@LineOfCreditId", DbType.String);
            base.AddOutParameter("@isCustomerExist", DbType.Boolean, 1);

            base[0] = salesCustomerId;
            base[1] = creditCustomerId;
            base[2] = lineOfCreditId;
        }

        public bool Execute(IApiRequest apiRequest, IDbLogggerRepository dbLoggger)
        {
            var apiRequestModel = apiRequest.GetRequest();
            try
            {
                base.ExecuteNonQuery();

                if (base[3] != null && base[3] != DBNull.Value)
                {
                    return (bool)base[3];
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.BS_SP_VerifyCustomerExist", new { this.salesCustomerId, this.creditCustomerId, this.lineOfCreditId }, ex.ErrorMessages);
                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.BS_SP_VerifyCustomerExist", new { this.salesCustomerId, this.creditCustomerId, this.lineOfCreditId }, ex.Message);
                throw new DBException(ex, new { this.salesCustomerId, this.creditCustomerId, this.lineOfCreditId, Sp = "dbo.BS_SP_VerifyCustomerExist" });
            }

            return false;
        }
    }
}

