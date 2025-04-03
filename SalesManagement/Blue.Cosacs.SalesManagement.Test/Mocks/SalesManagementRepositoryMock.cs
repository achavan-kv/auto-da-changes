//using Blue.Cosacs.SalesManagement.Repositories;
//using Blue.Events;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Blue.Cosacs.SalesManagement.Test.Mocks
//{
//    internal class SalesManagementRepositoryMock : ISalesManagementRepository
//    {

//        public IEnumerable<CallType> GetCallTypes()
//        {
//            throw new NotImplementedException();
//        }

//        public int SaveCall(Call callToSave)
//        {
//            throw new NotImplementedException();
//        }

//        public void SaveCalls(IList<Call> callsToSave)
//        {
//        }

//        public IEnumerable<Call> GetScheduledCalls(CallSearchFilter searchFilter)
//        {
//            throw new NotImplementedException();
//        }

//        public Call GetCall(int callId)
//        {
//            throw new NotImplementedException();
//        }

//        public int LogCall(Call mainCall, Call callbackCall, int[] pendingCalls, IEventStore audit, CustomerSalesPerson customerSalesPerson)
//        {
//            throw new NotImplementedException();
//        }

//        public HashSet<string> GetCustomerWithCallsFromSource(CallSourceEnum source)
//        {
//            throw new NotImplementedException();
//        }

//        public void InsertCustomersSalesPerson(IList<CustomerSalesPerson> allocatedCustomers)
//        {
//        }

//        public void UpdateCustomersSalesPerson(Dictionary<string, CustomerSalesPerson> allocatedCustomers)
//        {
//        }

//        public IQueryable<CustomerSalesPerson> GetCustomersSalesPerson(bool? doNotCallAgain, IList<string> customers)
//        {
//            return new List<CustomerSalesPerson>().AsQueryable();
//        }

//        public IList<Customer> GetCustomersWithUnscheduledCallsByCSR(int salesPersonId)
//        {
//            throw new NotImplementedException();
//        }

//        public Customer GetCustomerDetails(string customerId)
//        {
//            throw new NotImplementedException();
//        }

//        public IList<FollowUpCall> GetFollowUpCalls()
//        {
//            return new List<FollowUpCall>();
//        }

//        public void SaveFollowUpCall(FollowUpCall followUpCall)
//        {
//            throw new NotImplementedException();
//        }

//        public void DeleteFollowUpCall(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public IList<SalesPersonUnavailableView> GetCsrUnavailable(short userFromBranch, int? csr, DateTime? from, DateTime? to, int? numberOfRecords)
//        {
//            throw new NotImplementedException();
//        }

//        public void SaveCsrUnavailable(CsrUnavailable value)
//        {
//            throw new NotImplementedException();
//        }

//        public void DeleteCsrUnavailable(int id)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<string> GetCustomersWithAutoScheduledCalls(DateTime startDate, DateTime endDate)
//        {
//            throw new NotImplementedException();
//        }

//        public void DeleteCustomersWithAutoScheduledCalls(DateTime startDate, IEnumerable<string> customers = null)
//        {
//            throw new NotImplementedException();
//        }

//        public void UpdateAutoScheduledCalls(DateTime rangeDatesStart, DateTime rangeDatesEnd, Dictionary<string, DateTime> customers)
//        {
//            throw new NotImplementedException();
//        }

//        public IList<Call> GetCallsDetails(string customerId)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<CallType> GetCallTypesForBranchManager()
//        {
//            throw new NotImplementedException();
//        }

//        public ScheduledCalls GetBranchManagerCustomers(CallSearchFilter searchFilter)
//        {
//            throw new NotImplementedException();
//        }

//        public void FlushUnmadeCalls(int daysToFlush, DateTime today, int callCloseReasonId)
//        {

//        }

//        public void ScheduleCall(Call quickDetailsCall)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<CallType> GetCallTypesForCSR()
//        {
//            throw new NotImplementedException();
//        }

//        public ScheduledCalls GetScheduledCallsByCSR(CallSearchFilter searchFilter)
//        {
//            throw new NotImplementedException();
//        }

//        public IQueryable<int> GetUnavailableCSR(short branch)
//        {
//            throw new NotImplementedException();
//        }

//        public void AllocateUnallocatedCalls(int[] selectedCalls, int salesPersonId, IEventStore audit)
//        {
//            throw new NotImplementedException();
//        }

//        public CustomerSalesPerson GetCSRDetails(string customerId)
//        {
//            throw new NotImplementedException();
//        }

//        public List<SalesPersonTargets> GetSalesPersonTargets(int salesPersonId)
//        {
//            throw new NotImplementedException();
//        }

//        public void SaveCSRTargets(SalesPersonTargets salesPersonTargets, IEventStore audit)
//        {
//            throw new NotImplementedException();
//        }

//        public void AllocateCustomersToCSR(IList<CustomerSalesPerson> customers, IList<CustomerSalesPerson> oldCustomers, IEventStore audit)
//        {
//            throw new NotImplementedException();
//        }

//        public IList<PreviousCall> GetPreviousCallsForACustomer(string customerId, int numberOfCalls)
//        {
//            throw new NotImplementedException();
//        }

//        public string GetCallTypeName(byte callTypeId)
//        {
//            throw new NotImplementedException();
//        }

//        public IList<IconTypes> GetIconTypes()
//        {
//            throw new NotImplementedException();
//        }

//        public void SaveIconTypes(IList<IconTypes> iconTypes)
//        {
//            throw new NotImplementedException();
//        }

//        #region ISalesManagementRepository Members


//        public List<short> AllBranchesNumbers()
//        {
//            throw new NotImplementedException();
//        }

//        #endregion


//        public ScheduledCalls GetBranchManagerUnallocatedCalls(CallSearchFilter searchFilter)
//        {
//            throw new NotImplementedException();
//        }


//        public ScheduledCalls GetBranchManagerCalls(CallSearchFilter searchFilter)
//        {
//            throw new NotImplementedException();
//        }


//        public Dictionary<string, int> GetSalesPersonIdByCustomerId(IList<string> customerIds)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
