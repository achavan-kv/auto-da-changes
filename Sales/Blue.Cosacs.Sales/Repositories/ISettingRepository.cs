using System.Collections.Generic;

namespace Blue.Cosacs.Sales.Repositories
{
    public interface ISettingRepository
    {
        IEnumerable<string> GetRefundExchangeReasons(string path);
        dynamic GetTaxTypeSettings();
    }
}
