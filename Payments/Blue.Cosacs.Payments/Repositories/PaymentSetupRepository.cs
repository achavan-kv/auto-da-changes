using Blue.Events;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Payments
{
    public class PaymentSetupRepository : IPaymentSetupRepository
    {
        private readonly IClock clock;
        private readonly IEventStore audit;

        public PaymentSetupRepository(IClock clock, IEventStore audit)
        {
            this.clock = clock;
            this.audit = audit;
        }

        public List<PaymentMethod> GetAllPaymentMethods()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.PaymentMethod.ToList();
            }
        }

        public List<PaymentMethod> GetActivePaymentMethods()
        {
            return GetAllPaymentMethods()
                .Where(e => e.Active == true)
                .ToList();
        }

        public PaymentMethod PaymentMethodSetup(PaymentMethod paymentMethod, int currentUserId)
        {
            using (var scope = Context.Write())
            {
                var id = paymentMethod == null ? 0 : paymentMethod.Id;
                var existingData = scope.Context.PaymentMethod.FirstOrDefault(i => i.Id == id);

                if (existingData == null) return existingData;

                existingData.Active = paymentMethod.Active;
                existingData.IsReturnAllowed = paymentMethod.IsReturnAllowed;
                existingData.IsCashReturnAllowed = paymentMethod.IsCashReturnAllowed;

                scope.Context.SaveChanges();
                scope.Complete();

                audit.LogAsync(
                    new
                    {
                        existingData.Id,
                        existingData.Description,
                        existingData.Active,
                        existingData.IsReturnAllowed,
                        UserId = currentUserId,
                        Time = clock.Now
                    },
                    AuditEventTypes.PaymentMethodStatus,
                    AuditCategories.PaymentMethodData);

                return existingData;
            }
        }
    }
}
