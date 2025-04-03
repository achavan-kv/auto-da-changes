using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement.EventTypes
{
    public sealed class EventType
    {
        public const string InsertLog = "InsertCall";
        public const string UpdateLog = "UpdateCall";
        public const string UpdateBulkLog = "UpdateBulkLog";
        public const string DoNotCallAgain = "DoNotCallAgain";
        public const string AllocateCallsToCSR = "AllocateCallsToCSR";
        public const string SaveSalesPersonTargets = "SaveSalesPersonTargets";
        public const string AllocateCustomerToCSR = "AllocateCustomerToCSR";
        public const string BulkScheduleEmails = "BulkScheduleEmails";
        public const string BulkSendEmails = "BulkSendEmails";
        public const string BulkSendSms = "BulkSendSms";
        public const string ScheduleEmailsToSend = "ScheduleEmailsToSend";
        public const string ScheduleSmsToSend = "ScheduleSmsToSend";
        public const string QuickDetailsCaptureCall = "QuickDetailsCaptureCall";
    }
}
