using System.Collections.Generic;

namespace Blue.Cosacs.Payments
{
    public interface IPaymentSetupRepository
    {
        List<PaymentMethod> GetAllPaymentMethods();
        List<PaymentMethod> GetActivePaymentMethods();
        PaymentMethod PaymentMethodSetup(PaymentMethod paymentMethod, int currentUserId);
    }
}