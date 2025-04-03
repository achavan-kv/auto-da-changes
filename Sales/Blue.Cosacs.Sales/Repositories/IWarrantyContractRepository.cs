using Blue.Cosacs.Sales.Models;
using System.Collections.Generic;

namespace Blue.Cosacs.Sales.Repositories
{
    public interface IWarrantyContractRepository
    {
        IEnumerable<IEnumerable<DocumentCopy<WarrantyContractDetailsResult>>> GetWarrantyContractDetails(string agreementNo, string[] contractNos, bool multiple);//int? agreementNo
    }
}
