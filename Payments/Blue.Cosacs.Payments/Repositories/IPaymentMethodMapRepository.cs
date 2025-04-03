using System.Collections.Generic;
using Blue.Cosacs.Payments.Models;

namespace Blue.Cosacs.Payments.Repositories
{
    public interface IPaymentMethodMapRepository
    {
        List<PaymentMethodMapDto> Get(short branchNo);
        string Save(short branchNo, short posId, short winCosacsId);
    }
}