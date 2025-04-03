using System;
using System.Data;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.DbCommands
{
    public class CountryCommand : Blue.Transactions.Command<ContextBase>
    {
        public CountryCommand() : base("dbo.SP_BS_GetCountryCode")
        {
        }

        public CountryModel Execute(IApiRequest apiRequest, IDbLogggerRepository dbLoggger)
        {
            var apiRequestModel = apiRequest.GetRequest();
            var result = new CountryModel();
            try
            {
                var ds = new DataSet();
                base.Fill(ds);

                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    result.Countrycode = Convert.ToString(ds.Tables[0].Rows[0]["countrycode"]);
                    result.Origbr = Convert.ToInt16(ds.Tables[0].Rows[0]["origbr"]);
                    result.ISOCountryCode = Convert.ToString(ds.Tables[0].Rows[0]["ISOcountryCode"]);
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_GetCountryCode", "", ex.ErrorMessages);
                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_GetCountryCode", "", ex.Message);
                throw new DBException(ex, new { Sp = "dbo.SP_BS_GetCountryCode" });
            }
            return result;
        }
    }
}
