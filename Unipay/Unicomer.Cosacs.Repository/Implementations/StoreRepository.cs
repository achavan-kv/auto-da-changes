/* 
Author: 
Date:  
Description:JM BlueStart-get Store Service API 
 */

using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Stores;
using Unicomer.Cosacs.Repository.DbCommands.Stores;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.Implementations
{
    internal class StoreRepository : IStoreRepository
    {
        private readonly IDbLogggerRepository dbLoggger;

        public StoreRepository(IDbLogggerRepository dbLoggger)
        {
            this.dbLoggger = dbLoggger;
        }

        public StoreModel GetStore(string countryIsoCode, IApiRequest apiRequest)
        {
            return new StoreCommand(countryIsoCode).Execute(apiRequest, dbLoggger);
        }
    }
}
