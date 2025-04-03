using Blue.Cosacs.Payments.Models.WinCosacs;
using Blue.Networking;
using StructureMap;
using System;
using System.Collections.Generic;

namespace Blue.Cosacs.Payments.ExternalHttpService
{
    public class CountryTaxInfo
    {
        private readonly string currentUserId;
        private readonly string _TaxType = null;

        public CountryTaxInfo(string currentUserId)
        {
            this.currentUserId = currentUserId;
            _TaxType = "I"; // TODO remove this after settings read fix - only works on tax inclusive countries...
        }

        public System.String TaxType
        {
            get
            {
                if (_TaxType == null)
                {
                    //var httpClientJson = new Blue.Networking.HttpClientJsonAuth(
                    //    new HttpClient(),
                    //    ObjectFactory.GetInstance<IClock>(),
                    //    currentUserId);

                    //var test_countryTaxInfoFromTheSalesModule = ExternalHttpSources
                    //    .Get<List<BranchInfo>>("/Sales/api/Settings", httpClientJson);

                    //if (_TaxType != "I" && _TaxType != "E")
                    //{
                    //    throw new Exception("Tax Type must be I or E.");
                    //}
                }

                return _TaxType;
            }
        }

    }
}
