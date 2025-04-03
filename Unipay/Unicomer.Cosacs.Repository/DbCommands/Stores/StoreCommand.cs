using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unicomer.Cosacs.Model.Exceptions;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Stores;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.DbCommands.Stores
{
    public class StoreCommand : Blue.Transactions.Command<ContextBase>
    {
        string countryIsoCode;

        public StoreCommand(string countryIsoCode) : base("dbo.SP_BS_GetStoreService")
        {
            this.countryIsoCode = countryIsoCode;

            base.AddInParameter("@CountryCode", DbType.String);
            base.AddOutParameter("@message", DbType.String, 100);
            base.AddOutParameter("@error", DbType.Boolean, 1);

            base[0] = countryIsoCode;
        }

        public StoreModel Execute(IApiRequest apiRequest, IDbLogggerRepository dbLoggger)
        {
            var apiRequestModel = apiRequest.GetRequest();
            string message = "";
            bool isError = false;

            var result = new StoreModel();

            List<StoreDetail> storeDetail = new List<StoreDetail>();
            try
            {
                var ds = new DataSet();
                base.Fill(ds);

                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    storeDetail = ds.Tables[0].Rows.OfType<DataRow>()
                         .Select(p => new StoreDetail()
                         {
                             StoreName = Convert.ToString(p["StoreName"]).Trim(),
                             StoreId = Convert.ToString(p["StoreID"]),

                         }).ToList();
                }

                if (base[1] != null && base[1] != DBNull.Value)
                {
                    message = base[1].ToString();
                }

                if (base[2] != null && base[2] != DBNull.Value)
                {
                    isError = (bool)base[2];
                }

                if (isError)
                {
                    throw new ValidationException(new ErrorMessage { Id = this.countryIsoCode, Detail = message }, 
                        new { countryIsoCode, Sp = "dbo.SP_BS_GetStoreService" });
                }
            }
            catch (ValidationException ex)
            {
                dbLoggger.LogDbValidation(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_GetStoreService", countryIsoCode, ex.ErrorMessages);
                throw;
            }
            catch (Exception ex)
            {
                dbLoggger.LogDbFailure(apiRequestModel.RequestId, apiRequestModel.RequestName, "dbo.SP_BS_GetStoreService", countryIsoCode, ex.Message);
                throw new DBException(ex, new { countryIsoCode, Sp = "dbo.SP_BS_GetStoreService" });
            }

            result.StoreDetail = storeDetail;
            result.CountryIsoCode = countryIsoCode;
            return result;
        }
    }
}
