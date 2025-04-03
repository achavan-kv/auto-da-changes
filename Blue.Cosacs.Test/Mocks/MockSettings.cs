using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Config;

namespace Blue.Cosacs.Test.Mocks
{
    public class MockSettings : Blue.Config.Repositories.ISettings
    {
        private readonly IDictionary<string, SettingView> cache = null;

        public MockSettings()
        {
            cache = Blue.Cosacs.Test.CvsReader.Reader<SettingView>.Read(@"CsvSources\Config\SettingView.csv")
                .ToDictionary(p => p.CodeName);
        }

        public string Get(string code)
        {
            if (!cache.ContainsKey(code))
                throw new Exception("There is no system setting (country parameter) with code '" + code + "'.");
            return cache[code].Value;
        }

        private decimal GetDecimal(string code)
        {
            return decimal.Parse(Get(code));
        }

        public decimal TaxRate
        {
            get
            {
                return GetDecimal("TaxRate");
            }
        }

        public string TaxType
        {
            get
            {
                return Get("TaxType");
            }
        }
    }
}
