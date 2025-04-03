/* 
Author: 
Date:  
Description:JM BlueStart-Get Store Service API 
 */

using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Stores
{
    public class StoreModel
    {
        public string CountryIsoCode { get; set; }
        public List<StoreDetail> StoreDetail { get; set; }
    }
}


