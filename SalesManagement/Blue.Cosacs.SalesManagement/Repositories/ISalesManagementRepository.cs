using Blue.Cosacs.SalesManagement.Hub.Subscribers;
using Blue.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement.Repositories
{
    public interface ISalesManagementRepository
    {
        /*CallType*/
        IEnumerable<CallType> GetCallTypes();
        /*Call*/
        void SaveCalls(IList<Call> callsToSave);
        void SaveCalls(IList<Call> callsToSave, Blue.Cosacs.SalesManagement.Hub.Subscribers.Customer customer);
        IEnumerable<Call> GetScheduledCalls(CallSearchFilter searchFilter);
        Call GetCall(int callId);
        int LogCall(Call mainCall, Call callbackCall, int[] pendingCalls, IEventStore audit, CustomerSalesPerson customerSalesPerson);
        HashSet<string> GetCustomerWithCallsFromSource(CallSourceEnum source);
        void FlushCalls(CallClosedReason.CallClosedReasonEnum callCloseReasonId, int[] callIds, List<SmsToSend> smsToSend, List<MailsToSend> mailsToSend);
        /*CustomerSalesPerson*/
        void InsertCustomersSalesPerson(IList<CustomerSalesPerson> allocatedCustomers);
        void UpdateCustomersSalesPerson(Dictionary<string, CustomerSalesPerson> allocatedCustomers);
        IQueryable<CustomerSalesPerson> GetCustomersSalesPerson(bool? doNotCallAgain, IList<string> customers);
        IList<Customer> GetCustomersWithUnscheduledCallsByCSR(int salesPersonId);
        Customer GetCustomerDetails(string customerId);
        /*FollowUpCall*/
        IList<FollowUpCall> GetFollowUpCalls();
        void SaveFollowUpCall(FollowUpCall followUpCall);
        void DeleteFollowUpCall(int id);
        List<IGrouping<byte?, CallToFlush>> CallsToFlush(DateTime today, int daysToFlush);
        /*CsrUnavailable*/
        IList<SalesPersonUnavailableView> GetCsrUnavailable(short userFromBranch, int? csr, DateTime? from, DateTime? to, int? numberOfRecords);
        void SaveCsrUnavailable(CsrUnavailable value);
        void DeleteCsrUnavailable(int id);
        void ScheduleFollowUpCalls(Blue.Cosacs.SalesManagement.Hub.Subscribers.Customer customer, IList<Call> callsToSave, IList<MailsToSend> mailsToSave, IList<SmsToSend> smsToSave);
        /*--*/
        IEnumerable<string> GetCustomersWithAutoScheduledCalls(DateTime startDate, DateTime endDate);
        void DeleteCustomersWithAutoScheduledCalls(DateTime startDate, IEnumerable<string> customers = null);
        void UpdateAutoScheduledCalls(DateTime rangeDatesStart, DateTime rangeDatesEnd, Dictionary<string, DateTime> customers);

        IList<Call> GetCallsDetails(string customerId);
        IEnumerable<CallType> GetCallTypesForBranchManager();
        ScheduledCalls GetBranchManagerCalls(CallSearchFilter searchFilter);
        void ScheduleCall(Call callToMake);
        IEnumerable<CallType> GetCallTypesForCSR();
        ScheduledCalls GetScheduledCallsByCSR(CallSearchFilter searchFilter);
        IQueryable<int> GetUnavailableCSR(short branch);
        void AllocateUnallocatedCalls(int[] selectedCalls, int salesPersonId, IEventStore audit);
        CustomerSalesPerson GetCSRDetails(string customerId);

        List<short> AllBranchesNumbers();

        /*SalesPersonTargets*/
        List<SalesPersonTargets> GetSalesPersonTargets(int salesPersonId);
        void SaveCSRTargets(SalesPersonTargets salesPersonTargets, IEventStore audit);
        /***************/

        /*Branch Manager Customers Search - Reallocate Customers*/
        void AllocateCustomersToCSR(IList<CustomerSalesPerson> customers, IList<CustomerSalesPerson> oldCustomers, IEventStore audit);
        /***************/

        /*InactiveCustomersInteraction*/
        IList<AdditionalCustomersInteraction> LoadInactiveCustomersInteraction(AdditionalCustomersInteraction.InteractionType interactionType = AdditionalCustomersInteraction.InteractionType.All);
        void UpdateInactiveCustomersInteraction(AdditionalCustomersInteraction value);
        /*************************/

        /*Previous Calls*/
        IList<PreviousCall> GetPreviousCallsForACustomer(string customerId, int numberOfCalls);
        string GetCallTypeName(byte callTypeId);
        /***************/

        /*Icons to inactive customer/account settlement calls*/

        IList<IconTypes> GetIconTypes();
        void SaveIconTypes(IList<IconTypes> iconTypes);
        /***************/

        /*Branch Manager - Unallocated Calls*/
        ScheduledCalls GetBranchManagerUnallocatedCalls(CallSearchFilter searchFilter);
        /***************/

        Dictionary<string, int> GetSalesPersonIdByCustomerId(IList<string> customerIds);

        /*MailsToSend*/
        void InsertMailsToSend(MailsToSend value);
        void InsertMailsToSend(IList<MailsToSend> value);
        IList<MailsToSend> LoadMailsToSend(DateTime? sendingDate);
        void DeleteMailsToSend(IList<int> ids);

        /*SmsToSend*/
        void InsertSmsToSend(IList<SmsToSend> value);
        IList<SmsToSend> LoadSmsToSend(DateTime? sendingDate);
        void DeleteSmsToSend(IList<int> ids);

        /*SummaryTable*/
        void InsertSummaryTable(List<SummaryTable> values);
    }
}
