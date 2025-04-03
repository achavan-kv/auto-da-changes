using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Business.Interfaces
{
    public interface ICredits
    {
        JResponse GetMaxWithdrawalAmount(string custId);
        JResponse GetPaymentOptionsByAmount(string CustId, string loanAmount);
    }
}
