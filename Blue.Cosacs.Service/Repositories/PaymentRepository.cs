using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs.Service.Models;
using Blue.Cosacs.Messages.Service;

namespace Blue.Cosacs.Service.Repositories
{
    public class PaymentRepository
    {
        public PaymentRepository(IClock clock, Chub hub)
        {
            this.clock = clock;
            this.hub = hub;
        }

        private readonly IClock clock;
        private readonly Chub hub;

        public void SavePayment(ServicePayment payment)
        {
            using (var scope = Context.Write())
            {
                scope.Context.Payment.Add(new Payment()
                {
                    Amount = payment.Amount,
                    Bank = payment.Bank,
                    BankAccountNumber = payment.BankAccountNumber,
                    CustomerId = payment.CustomerId,
                    RequestId = payment.ServiceRequestNo,
                    PayMethod = payment.PayMethod,
                    EmpeeNo = payment.EmpeeNo,
                    ChequeNumber = payment.ChequeNumber,
                    ChargeType = payment.ChargeType
                });
                scope.Context.SaveChanges();
                hub.ServicePayment(payment); // inside transaction
                scope.Complete();
            }
        }
    }
}
