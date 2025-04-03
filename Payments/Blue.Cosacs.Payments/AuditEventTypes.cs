using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Payments
{
    public class AuditEventTypes
    {
        public const string PaymentMethodStatus = "PaymentMethodStatus";
        public const string InsertExchageRate = "InsertExchageRate";
        public const string UpdateExchangeRate = "UpdateExchangeRate";
        public const string DeleteExchangeRate = "DeleteExchangeRate";
    }

}
