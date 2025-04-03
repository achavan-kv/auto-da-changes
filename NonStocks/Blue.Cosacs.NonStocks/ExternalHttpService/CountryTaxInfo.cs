using Blue.Networking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.NonStocks.ExternalHttpService
{
    public class CountryTaxInfo
    {
        private readonly string currentUserId;
        public readonly string TaxType = null;
        public readonly decimal? TaxRate = 0;

        public CountryTaxInfo(string currentUserId, IClock clock)
        {
            this.currentUserId = currentUserId;
            TaxType = ReadTaxTypeSetting(currentUserId, clock);
            TaxRate = ReadTaxRateSetting(currentUserId, clock) * 100;
        }

        private decimal ReadTaxRateSetting(string currentUserId, IClock clock)
        {
            var client = new HttpClientJsonAuth(new Blue.Networking.HttpClient(), clock, currentUserId);
            var request = RequestJson<byte[]>.Create("/Cosacs/Merchandising/Tax/GetTaxSettings", "GET");

            var clientRequest = client.Do<byte[], TaxRateRequest>(request);
            if (clientRequest.Body != null && clientRequest.Body.data != null)
            {
                return decimal.Parse(clientRequest.Body.data.currentTaxRate);
            }
            else
            {
                throw (new ApplicationException("Tax Type not set on the Merchandising module!")); // fail loudly
            }
        }

        private string ReadTaxTypeSetting(string currentUserId, IClock clock)
        {
            var client = new HttpClientJsonAuth(new Blue.Networking.HttpClient(), clock, currentUserId);
            var request = RequestJson<byte[]>.Create("/Cosacs/Settings/GetSetting?moduleNamespace=Blue.Cosacs.Merchandising", "GET");
            var clientRequest = client.Do<byte[], List<TaxTypeSetting>>(request);

            if (clientRequest.Body != null)
            {
                var taxInclusiveSetting = clientRequest.Body
                    .Where(e => e.meta != null && e.module != null &&
                        e.module.Namespace == "Blue.Cosacs.Merchandising" &&
                        e.meta.Id == "TaxInclusive")
                    .FirstOrDefault();

                if (bool.Parse(taxInclusiveSetting.value))
                {
                    return "I";
                }
                else
                {
                    return "E";
                }
            }
            else
            {
                throw (new ApplicationException("Tax rate not set on the Merchandising module!")); // fail loudly
            }
        }

        private class TaxRateRequest
        {
            public TaxRateSetting data { get; set; }
        }

        private class TaxRateSetting
        {
            public string taxSetting { get; set; }
            public string currentTaxRate { get; set; }
        }

        private class TaxTypeSetting
        {
            public TaxTypeSettingModule module { get; set; }
            public TaxTypeSettingMetadata meta { get; set; }
            public string value { get; set; }
        }

        private class TaxTypeSettingModule
        {
            public string Namespace { get; set; }
        }

        private class TaxTypeSettingMetadata
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
