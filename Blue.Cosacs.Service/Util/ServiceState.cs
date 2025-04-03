using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Service.Utils
{
    public static class ServiceState
    {
        public const string AwaitingAllocation = "Awaiting allocation";
        public const string AwaitingDeposit = "Awaiting deposit";
        public const string AwaitingInstallation = "Awaiting installation";
        public const string AwaitingPayment = "Awaiting payment";
        public const string AwaitingRepair = "Awaiting repair";
        public const string AwaitingSpareParts = "Awaiting spare parts";
        public const string BER = "BER";
        public const string Closed = "Closed";
        public const string New = "New";
        public const string Resolved = "Resolved";
    }
}
