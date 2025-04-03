namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class VendorReturnSearchModel
    {
        public VendorReturnSearchModel()
        {
            Results = new List<VendorReturnSearchResultModel>();
        }     
        public int TotalResults { get; set; }

        public List<VendorReturnSearchResultModel> Results { get; set; }     
    }
}
