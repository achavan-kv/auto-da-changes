using System;
using System.Data;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.DbCommands
{
    public class BranchCommand : Blue.Transactions.Command<ContextBase>
    {
        string storeId;

        public BranchCommand(string storeId) : base("dbo.BS_SP_GetBranchByLocation")
        {
            this.storeId = storeId;
            base.AddInParameter("@LocationID", DbType.String);
            base.AddOutParameter("@BranchNo", DbType.Int16, 4);
            base[0] = storeId;
        }

        public short Execute(IApiRequest apiRequest, IDbLogggerRepository dbLoggger)
        {
            var apiRequestModel = apiRequest.GetRequest();
            short result = 0;
            try
            {
                base.ExecuteNonQuery();

                if (base[1] != null && base[1] != DBNull.Value)
                {
                    result = Convert.ToInt16(base[1]);
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.BS_SP_GetBranchByLocation", storeId, ex.ErrorMessages);
                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.BS_SP_GetBranchByLocation", storeId, ex.Message);
                throw new DBException(ex, new { storeId, Sp = "dbo.BS_SP_GetBranchByLocation" });
            }
            return result;
        }
    }
}
