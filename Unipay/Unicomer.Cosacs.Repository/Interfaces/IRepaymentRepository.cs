using Unicomer.Cosacs.Model.Interfaces;

namespace Unicomer.Cosacs.Repository.Interfaces
{
    public interface IRepaymentRepository
    {
        bool SaveRepayment(string mambuAccountId, decimal amount, string transactionId, IApiRequest apiRequest);
    }
}
