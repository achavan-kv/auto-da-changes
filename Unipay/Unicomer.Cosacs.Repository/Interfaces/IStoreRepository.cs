/*
Author: 
Date:  
Description:JM BlueStart-Pre-Qualification API 
 */

using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Stores;

namespace Unicomer.Cosacs.Repository.Interfaces
{
    public interface IStoreRepository
    {
        StoreModel GetStore(string countryIsoCode, IApiRequest apiRequest);
    }
}
