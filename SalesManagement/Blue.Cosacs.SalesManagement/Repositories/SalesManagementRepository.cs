using Blue.Cosacs.SalesManagement.EventTypes;
using Blue.Cosacs.SalesManagement.Hub.Subscribers;
using Blue.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.Entity;
using Blue.Transactions;
//using System.Data.Objects.SqlClient;

namespace Blue.Cosacs.SalesManagement.Repositories
{
    public class SalesManagementRepository : ISalesManagementRepository
    {
        private readonly IEventStore audit;
        private readonly IClock clock;
        private readonly ICallSummarySubscriber callSummarySubscriber;

        public SalesManagementRepository(IEventStore audit, IClock clock, ICallSummarySubscriber callSummarySubscriber)
        {
            this.audit = audit;
            this.clock = clock;
            this.callSummarySubscriber = callSummarySubscriber;
        }

        public IEnumerable<CallType> GetCallTypes()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.CallType
                    .Where(p => p.IsSystemType == false)
                    .Select(p => p)
                    .ToList();
            }
        }

        public IEnumerable<Call> GetScheduledCalls(CallSearchFilter searchFilter)
        {
            using (var scope = Context.Read())
            {
                var scheduledCalls = from c in scope.Context.Call
                                     where c.CallClosedReasonId == null
                                     select c;

                if (searchFilter.CallTypeId.HasValue)
                {
                    scheduledCalls = scheduledCalls
                        .Where(p => p.CallTypeId == searchFilter.CallTypeId.Value);
                }

                if (searchFilter.ScheduledDateFrom != null)
                {
                    var scheduledDateFrom = searchFilter.ScheduledDateFrom.Value.Date;

                    scheduledCalls = scheduledCalls
                        .Where(p => p.ToCallAt >= scheduledDateFrom);
                }

                if (searchFilter.ScheduledDateTo != null)
                {
                    var scheduledDateTo = searchFilter.ScheduledDateTo.Value.Date.AddSeconds((24 * 60 * 60) - 1);// nr of seconds in a day;
                    scheduledCalls = scheduledCalls
                        .Where(p => p.ToCallAt <= scheduledDateTo);
                }

                if (!string.IsNullOrEmpty(searchFilter.CustomerName))
                {
                    scheduledCalls = scheduledCalls
                       .Where(p => (p.CustomerFirstName + " " + p.CustomerLastName).Contains(searchFilter.CustomerName));
                }

                if (!string.IsNullOrEmpty(searchFilter.ReasonForCalling))
                {
                    scheduledCalls = scheduledCalls
                        .Where(p => p.ReasonToCall.Contains(searchFilter.ReasonForCalling));
                }

                scheduledCalls = scheduledCalls
                    .OrderBy(p => p.ToCallAt);

                if (searchFilter.Take.HasValue)
                {
                    return scheduledCalls.Take(searchFilter.Take.Value).ToList();
                }

                return scheduledCalls.ToList();
            }
        }

        public void SaveCalls(IList<Call> callsToSave)
        {
            this.SaveCalls(callsToSave, null);
        }

        private void SaveCalls(IList<Call> callsToSave, Blue.Cosacs.SalesManagement.Hub.Subscribers.Customer customer, WriteScope<Context> scope)
        {
            var newCustomers = new List<CustomerSalesPerson>();

            if (customer != null)
            {
                var csr = scope.Context.CustomerSalesPerson
                    .Find(customer.CustomerId);

                var testNotCall = false;

                if (csr != null)
                {
                    // the customer does not want to receive calls any more?
                    if (csr.DoNotCallAgain)
                    {
                        testNotCall = true;
                        return;
                    }

                    Debug.Assert(testNotCall == csr.DoNotCallAgain, "Customer should not get any call");

                    //the crs may had changed
                    csr.SalesPersonId = customer.SalesPerson;
                }
                else
                {
                    newCustomers.Add(
                        new CustomerSalesPerson
                        {
                            CustomerBranch = short.Parse(customer.CustomerAccount.Substring(0, 3)),
                            CustomerId = customer.CustomerId,
                            DoNotCallAgain = false,
                            MobileNumber = customer.MobileNumber,
                            LandLinePhone = customer.LandLinePhone,
                            SalesPersonId = customer.SalesPerson,
                            TempSalesPersonId = null,
                            TempSalesPersonIdBegin = null,
                            TempSalesPersonIdEnd = null,
                            Email = customer.Email == null ? string.Empty : customer.Email.Trim()
                        });
                }
            }

            InsertCustomersSalesPerson(newCustomers);
            scope.Context.Call.AddRange(callsToSave);

            callSummarySubscriber.FillCallSummary();
        }

        public void SaveCalls(IList<Call> callsToSave, Blue.Cosacs.SalesManagement.Hub.Subscribers.Customer customer)
        {
            using (var scope = Context.Write())
            {
                SaveCalls(callsToSave, customer, scope);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public IEnumerable<string> GetCustomersWithAutoScheduledCalls(DateTime startDate, DateTime endDate)
        {
            using (var scope = Context.Read())
            {
                startDate = startDate.Date;
                endDate = endDate.Date.AddSeconds((24 * 60 * 60) - 1);
                return scope.Context.Call
                    .Where(p => p.ToCallAt >= startDate && p.ToCallAt <= endDate && p.CallClosedReasonId == null && p.CallTypeId == (int)CallTypeEnum.AutoScheduled)
                    .Select(p => p.CustomerId)
                    .ToList();
            }
        }

        public void DeleteCustomersWithAutoScheduledCalls(DateTime startDate, IEnumerable<string> customers = null)
        {
            using (var scope = Context.Write())
            {
                startDate = startDate.Date;
                var data = scope.Context.Call
                    .Where(p =>
                        p.ToCallAt >= startDate
                        && p.CallClosedReasonId == null
                        && p.CallTypeId == (int)CallTypeEnum.AutoScheduled
                        && customers.Contains(p.CustomerId));

                scope.Context.Call.RemoveRange(data);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void UpdateAutoScheduledCalls(DateTime rangeDatesStart, DateTime rangeDatesEnd, Dictionary<string, DateTime> customers)
        {
            using (var scope = Context.Write())
            {
                rangeDatesStart = rangeDatesStart.Date;
                rangeDatesEnd = rangeDatesEnd.Date.AddSeconds((24 * 60 * 60) - 1);
                var customersId = customers.Keys;

                var data = scope.Context.Call
                     .Where(p =>
                         p.ToCallAt >= rangeDatesStart
                         && p.ToCallAt <= rangeDatesEnd
                         && p.CallClosedReasonId == null
                         && p.CallTypeId == (int)CallTypeEnum.AutoScheduled
                         && customersId.Contains(p.CustomerId));

                foreach (var item in data)
                {
                    item.ToCallAt = customers[item.CustomerId];
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void InsertCustomersSalesPerson(IList<CustomerSalesPerson> allocatedCustomers)
        {
            using (var scope = Context.Write())
            {
                scope.Context.CustomerSalesPerson.AddRange(allocatedCustomers);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public HashSet<string> GetCustomerWithCallsFromSource(CallSourceEnum source)
        {
            using (var scope = Context.Read())
            {
                var byteSource = (byte)source;
                return new HashSet<string>(scope.Context.Call
                    .Where(p => p.Source == byteSource && !p.CallClosedReasonId.HasValue)
                    .Select(p => p.CustomerId));
            }
        }

        public void UpdateCustomersSalesPerson(Dictionary<string, CustomerSalesPerson> allocatedCustomers)
        {
            using (var scope = Context.Write())
            {
                var customerIds = new HashSet<string>(allocatedCustomers.Select(x => x.Value.CustomerId));

                var data = scope.Context.CustomerSalesPerson
                    .Where(p => customerIds.Contains(p.CustomerId));

                foreach (var item in data)
                {
                    item.SalesPersonId = allocatedCustomers[item.CustomerId].SalesPersonId;
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public Call GetCall(int callId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Call
                    .Where(p => callId == p.Id)
                    .Select(p => p)
                    .FirstOrDefault();
            }
        }

        public void FlushCalls(CallClosedReason.CallClosedReasonEnum callCloseReasonId, int[] callIds, List<SmsToSend> smsToSend, List<MailsToSend> mailsToSend)
        {
            using (var scope = Context.Write())
            {
                var calls = scope.Context.Call
                    .Where(p => callIds.Contains(p.Id))
                    .ToList();

                foreach (var item in calls)
                {
                    item.CallClosedReasonId = (byte)callCloseReasonId;
                }

                if (smsToSend != null && smsToSend.Any())
                {
                    scope.Context.SmsToSend.AddRange(smsToSend);
                }

                if (mailsToSend != null && mailsToSend.Any())
                {
                    scope.Context.MailsToSend.AddRange(mailsToSend);
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        #region FollowUp Calls

        public IList<FollowUpCall> GetFollowUpCalls()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.FollowUpCall.ToList();
            }
        }

        public void SaveFollowUpCall(FollowUpCall followUpCall)
        {
            using (var scope = Context.Write())
            {
                if (followUpCall.Id != 0)
                {
                    scope.Context.FollowUpCall.Attach(followUpCall);
                    scope.Context.Entry(followUpCall).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    scope.Context.FollowUpCall.Add(followUpCall);
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void DeleteFollowUpCall(int id)
        {
            using (var scope = Context.Write())
            {
                var followUpCall = scope.Context.FollowUpCall
                    .FirstOrDefault(p => p.Id == id);

                if (followUpCall != null)
                {
                    scope.Context.FollowUpCall.Remove(followUpCall);
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void ScheduleFollowUpCalls(Blue.Cosacs.SalesManagement.Hub.Subscribers.Customer customer, IList<Call> callsToSave, IList<MailsToSend> mailsToSave, IList<SmsToSend> smsToSave)
        {
            using (var scope = Context.Write())
            {
                if (callsToSave != null && callsToSave.Any())
                {
                    this.SaveCalls(callsToSave, customer, scope);
                }

                if (mailsToSave != null && mailsToSave.Any())
                {
                    InsertMailsToSend(mailsToSave, scope);
                }

                if (smsToSave != null && smsToSave.Any())
                {
                    InsertSmsToSend(smsToSave, scope);
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        #endregion

        public int LogCall(Call mainCall, Call callbackCall, int[] pendingCalls, IEventStore audit, CustomerSalesPerson customerSalesPerson)
        {
            using (var scope = Context.Write())
            {
                var mainCallId = 0;

                if (mainCall.Id == 0)
                {
                    //Insert Unscheduled Call

                    mainCall.CallClosedReasonId = (byte)Blue.Cosacs.SalesManagement.CallClosedReason.CallClosedReasonEnum.CalledTheCustomer;
                    mainCall.CallTypeId = (byte)CallTypeEnum.UnscheduledCall;

                    scope.Context.Call.Add(mainCall);
                    scope.Context.SaveChanges();

                    mainCallId = mainCall.Id;
                }
                else
                {
                    var existingCall = scope.Context.Call
                        .FirstOrDefault(p => p.Id == mainCall.Id);

                    //Update existing call
                    existingCall.CallClosedReasonId = (byte)Blue.Cosacs.SalesManagement.CallClosedReason.CallClosedReasonEnum.CalledTheCustomer;
                    existingCall.CalledAt = mainCall.CalledAt;
                    existingCall.SpokeToCustomer = mainCall.SpokeToCustomer;
                    existingCall.Comments = mainCall.Comments;
                    existingCall.CalledBy = mainCall.CalledBy;
                }

                Action<CallClosedReason.CallClosedReasonEnum, string, int, string> callLogDelegate = (reasonToClose, customerId, currentCallId, eventType) =>
                {
                    var callToClose = scope.Context.Call
                        .Where(p => p.CustomerId == mainCall.CustomerId && p.Id != currentCallId)
                        .Select(p => p)
                        .ToList();

                    foreach (var item in callToClose)
                    {
                        item.CallClosedReasonId = (byte)reasonToClose;
                    }

                    if (reasonToClose == CallClosedReason.CallClosedReasonEnum.DoNotCallAgain)
                    {
                        var mails = scope.Context.MailsToSend
                            .Where(p => p.CustomerId == mainCall.CustomerId)
                            .Select(p => p)
                            .ToList();

                        scope.Context.MailsToSend.RemoveRange(mails);
                    }

                    audit.LogAsync(
                        new
                        {
                            LogCall = callToClose
                        },
                        eventType);
                };

                if (customerSalesPerson.DoNotCallAgain)
                {
                    var customer = scope.Context.CustomerSalesPerson
                        .FirstOrDefault(p => p.CustomerId == customerSalesPerson.CustomerId);

                    if (customer != null)
                    {
                        customer.DoNotCallAgain = customerSalesPerson.DoNotCallAgain;

                        callLogDelegate.Invoke(CallClosedReason.CallClosedReasonEnum.DoNotCallAgain, customer.CustomerId, mainCall.Id, EventType.DoNotCallAgain);
                    }
                }
                else if (pendingCalls.Any())
                {
                    callLogDelegate.Invoke(CallClosedReason.CallClosedReasonEnum.ClosedInBulkByCSR, customerSalesPerson.CustomerId, mainCall.Id, EventType.UpdateBulkLog);
                }

                //Insert Scheduled Call
                if (callbackCall != null)
                {
                    if (mainCallId != 0)
                    {
                        callbackCall.PreviousCallId = mainCallId;
                    }

                    if (callbackCall.CallTypeId == 0)
                    {
                        callbackCall.CallTypeId = (byte)CallTypeEnum.UnscheduledCall;
                    }

                    scope.Context.Call.Add(callbackCall);

                    audit.LogAsync(
                        new
                        {
                            LogCall = callbackCall
                        },
                    EventType.InsertLog);
                }

                audit.LogAsync(
                    new
                    {
                        LogCall = mainCall
                    },
                    mainCall.Id == 0 ? EventType.InsertLog : EventType.UpdateLog);

                scope.Context.SaveChanges();
                scope.Complete();
            }

            callSummarySubscriber.FillCallSummary();

            return mainCall.Id;
        }

        public IQueryable<CustomerSalesPerson> GetCustomersSalesPerson(bool? doNotCallAgain, IList<string> customers)
        {
            using (var scope = Context.Read())
            {
                var customersSalesPerson = scope.Context.CustomerSalesPerson
                    .Select(p => p);

                if (customers != null && customers.Any())
                {
                    customersSalesPerson = customersSalesPerson
                        .Where(p => customers.Contains(p.CustomerId));
                }

                if (doNotCallAgain.HasValue)
                {
                    customersSalesPerson = customersSalesPerson
                        .Where(p => p.DoNotCallAgain == doNotCallAgain.Value);
                }

                return customersSalesPerson;
            }
        }

        public List<short> AllBranchesNumbers()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.CustomerSalesPerson
                    .Select(p => p.CustomerBranch)
                    .Distinct()
                    .ToList();
            }
        }

        public IList<Customer> GetCustomersWithUnscheduledCallsByCSR(int salesPersonId)
        {
            var customer = new Customer();

            using (var scope = Context.Read())
            {
                var calls = (from csp in scope.Context.CustomerSalesPerson
                             join c in scope.Context.Call on csp.CustomerId equals c.CustomerId
                             where (csp.SalesPersonId == salesPersonId)
                             select new Customer
                             {
                                 Id = c.Id,
                                 CustomerId = c.CustomerId,
                                 CustomerFirstName = c.CustomerFirstName,
                                 CustomerLastName = c.CustomerLastName,
                                 SalesPersonId = csp.SalesPersonId,
                                 Email = csp.Email,
                                 LandLinePhone = csp.LandLinePhone,
                                 MobileNumber = csp.MobileNumber
                             })
                             .ToList();

                return calls;
            }
        }

        public Customer GetCustomerDetails(string customerId)
        {
            using (var scope = Context.Read())
            {
                var customer = (from c in scope.Context.Call
                                join csp in scope.Context.CustomerSalesPerson on c.CustomerId equals csp.CustomerId
                                where (csp.CustomerId == customerId)
                                select new Customer
                                {
                                    CustomerFirstName = c.CustomerFirstName,
                                    CustomerLastName = c.CustomerLastName,
                                    MobileNumber = c.MobileNumber,
                                    LandLinePhone = c.LandLinePhone,
                                    Email = csp.Email,
                                    CustomerId = csp.CustomerId,
                                    SalesPersonId = csp.SalesPersonId
                                }).FirstOrDefault();

                return customer;
            }
        }

        public IList<Call> GetCallsDetails(string customerId)
        {
            using (var scope = Context.Read())
            {
                return (from csp in scope.Context.CustomerSalesPerson
                        join c in scope.Context.Call on csp.CustomerId equals c.CustomerId
                        where c.CustomerId == customerId &&
                              c.CallClosedReasonId == null &&
                              csp.DoNotCallAgain == false
                        select c)
                       .ToList();
            }
        }

        public IEnumerable<CallType> GetCallTypesForBranchManager()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.CallType
                    .Where(p => p.IsSystemType == false &&
                          (p.Id == (byte)CallTypeEnum.AutoScheduled ||
                           p.Id == (byte)CallTypeEnum.BranchManagerCustom ||
                           p.Id == (byte)CallTypeEnum.Prospective))
                    .Select(p => p)
                    .ToList();
            }
        }

        public IEnumerable<CallType> GetCallTypesForCSR()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.CallType
                    .Where(p => p.IsSystemType == false &&
                          (p.Id == (byte)CallTypeEnum.AutoScheduled ||
                           p.Id == (byte)CallTypeEnum.BranchManagerCustom ||
                           p.Id == (byte)CallTypeEnum.Callback ||
                           p.Id == (byte)CallTypeEnum.CsrCustom ||
                           p.Id == (byte)CallTypeEnum.Prospective
                           ))
                    .Select(p => p)
                    .ToList();
            }
        }

        public IList<SalesPersonUnavailableView> GetCsrUnavailable(short userFromBranch, int? csr, DateTime? from, DateTime? to, int? numberOfRecords)
        {
            using (var scope = Context.Read())
            {
                var values = from item in scope.Context.SalesPersonUnavailableView
                             where item.SalesPersonBranch == userFromBranch
                             select item;
                if (csr.HasValue)
                {
                    values = values
                        .Where(p => p.SalesPersonId == csr.Value);
                }

                if (from.HasValue)
                {
                    var dateFrom = from.Value.Date;

                    values = values
                        .Where(p => p.BeggingUnavailable >= dateFrom);
                }

                if (to.HasValue)
                {
                    var dateTo = to.Value.Date;
                    values = values
                    .Where(p => p.EndUnavailable <= dateTo);
                }

                if (numberOfRecords.HasValue)
                {
                    return values
                        .Take(numberOfRecords.Value)
                        .ToList();
                }
                return values.ToList();
            }
        }

        public ScheduledCalls GetBranchManagerCalls(CallSearchFilter searchFilter)
        {
            var calls = new ScheduledCalls();

            using (var scope = Context.Read())
            {
                var branchManagerCalls = scope.Context.BranchManagerCall
                    .Select(p => p);

                if (searchFilter.CallTypeId.HasValue)
                {
                    branchManagerCalls = branchManagerCalls
                        .Where(p => p.CallTypeId == searchFilter.CallTypeId.Value);
                }

                if (searchFilter.ScheduledDateFrom != null)
                {
                    var scheduledDateFrom = searchFilter.ScheduledDateFrom.Value.Date;

                    branchManagerCalls = branchManagerCalls
                        .Where(p => p.ToCallAt >= scheduledDateFrom);
                }

                if (searchFilter.ScheduledDateTo != null)
                {
                    var scheduledDateTo = searchFilter.ScheduledDateTo.Value.Date.AddSeconds((24 * 60 * 60) - 1);// nr of seconds in a day;
                    branchManagerCalls = branchManagerCalls
                        .Where(p => p.ToCallAt <= scheduledDateTo);
                }

                if (!string.IsNullOrEmpty(searchFilter.CustomerName))
                {
                    branchManagerCalls = branchManagerCalls
                       .Where(p => (p.CustomerFirstName + " " + p.CustomerLastName).Contains(searchFilter.CustomerName));
                }

                if (!string.IsNullOrEmpty(searchFilter.ReasonForCalling))
                {
                    branchManagerCalls = branchManagerCalls
                        .Where(p => p.ReasonForCalling.Contains(searchFilter.ReasonForCalling));
                }

                if (searchFilter.CSRId.HasValue)
                {
                    branchManagerCalls = branchManagerCalls
                       .Where(p => p.SalesPersonId == searchFilter.CSRId.Value);
                }

                branchManagerCalls = branchManagerCalls
                    .Where(p => p.CustomerBranch == searchFilter.Branch);

                branchManagerCalls = branchManagerCalls
                  .OrderBy(p => p.ToCallAt);

                calls.NoOfScheduledCalls = branchManagerCalls.Count();

                if (searchFilter.Take.HasValue)
                {
                    branchManagerCalls = branchManagerCalls.Take(searchFilter.Take.Value);
                }

                calls.BranchManagerCalls = branchManagerCalls.ToList();
            }

            return calls;
        }

        public void SaveCsrUnavailable(CsrUnavailable value)
        {
            using (var scope = Context.Write())
            {
                if (value.Id != 0)
                {
                    var existing = scope.Context.CsrUnavailable.FirstOrDefault(p => p.Id == value.Id);
                    if (existing != null)
                    {
                        existing.BeggingUnavailable = value.BeggingUnavailable;
                        existing.EndUnavailable = value.EndUnavailable;
                    }
                }
                else
                {
                    scope.Context.CsrUnavailable.Add(value);
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void DeleteCsrUnavailable(int id)
        {
            using (var scope = Context.Write())
            {
                var value = scope.Context.CsrUnavailable
                    .FirstOrDefault(p => p.Id == id);
                if (value != null)
                {
                    scope.Context.CsrUnavailable.Remove(value);
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public List<IGrouping<byte?, CallToFlush>> CallsToFlush(DateTime today, int daysToFlush)
        {
            using (var scope = Context.Read())
            {
                today = today.Date;

                return scope.Context.Call
                    .Where(p => System.Data.Entity.DbFunctions.DiffDays(p.ToCallAt, today) > daysToFlush)
                    .GroupJoin(
                        scope.Context.CustomerSalesPerson,
                        c => c.CustomerId,
                        cs => cs.CustomerId,
                        (call, custCsr) => new
                        {
                            Call = call,
                            CustomerSalesPerson = custCsr
                        })
                    .SelectMany(
                        p =>
                        p.CustomerSalesPerson.DefaultIfEmpty(),
                        (call, custCsr) => new CallToFlush
                        {
                            AlternativeContactMeanId = call.Call.AlternativeContactMeanId,
                            EmailSubject = call.Call.EmailSubject,
                            Id = call.Call.Id,
                            MailchimpTemplateID = call.Call.MailchimpTemplateID,
                            SmsText = call.Call.SmsText,
                            CustomerId = call.Call.CustomerId,
                            Email = custCsr == null ? null : custCsr.Email,
                            MobileNumber = custCsr.MobileNumber,
                            LandLinePhone = custCsr.LandLinePhone,
                        })
                    .GroupBy(p => p.AlternativeContactMeanId)
                    .ToList();
            }
        }

        public void ScheduleCall(Call callToMake)
        {
            using (var scope = Context.Write())
            {
                var existingSalesPerson = scope.Context.CustomerSalesPerson
                            .Where(p => p.CustomerId == callToMake.CustomerId)
                            .FirstOrDefault();

                //  this is for Customers that don`t have a CSR allocated yet
                if (existingSalesPerson == null)
                {
                    callToMake.CustomerId = null;
                }

                if (!string.IsNullOrEmpty(callToMake.CustomerId))
                {
                    if (existingSalesPerson == null && callToMake.SalesPersonId.HasValue)
                    {
                        var salesPerson = new CustomerSalesPerson()
                        {
                            CustomerId = callToMake.CustomerId,
                            CustomerBranch = callToMake.Branch.Value,
                            MobileNumber = callToMake.MobileNumber,
                            LandLinePhone = callToMake.LandLinePhone
                        };

                        if (callToMake.SalesPersonId.HasValue)
                        {
                            salesPerson.SalesPersonId = callToMake.SalesPersonId.Value;
                        }

                        scope.Context.CustomerSalesPerson.Add(salesPerson);
                        scope.Context.SaveChanges();
                    }
                }
                else
                {
                    callToMake.CustomerId = null;
                    callToMake.Branch = callToMake.Branch.Value;
                }

                scope.Context.Call.Add(callToMake);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public ScheduledCalls GetScheduledCallsByCSR(CallSearchFilter searchFilter)
        {
            var calls = new ScheduledCalls();
            var today = clock.Now.Date;
            using (var scope = Context.Read())
            {
                var myCalls = from c in scope.Context.Call
                              join csp in scope.Context.CustomerSalesPerson on c.CustomerId equals csp.CustomerId into allCalls
                              from call in allCalls.DefaultIfEmpty()
                              where c.SalesPersonId == searchFilter.SalesPersonId &&
                                    c.CallClosedReasonId == null
                              select c;

                var customersAssignedToMe = scope.Context.CustomerSalesPerson
                    .Where(p => p.TempSalesPersonId == searchFilter.SalesPersonId &&
                           p.TempSalesPersonIdBegin != null &&
                           p.TempSalesPersonIdEnd != null &&
                           today >= p.TempSalesPersonIdBegin &&
                           today <= p.TempSalesPersonIdEnd &&
                           p.DoNotCallAgain == false);

                var callsForCustomersAssignedToMe = scope.Context.Call
                    .Where(p => customersAssignedToMe.Select(a => a.CustomerId).Contains(p.CustomerId) &&
                           p.ToCallAt <= customersAssignedToMe.Select(a => a.TempSalesPersonIdEnd).FirstOrDefault() &&
                           p.CallClosedReasonId == null);

                var scheduledCalls = myCalls;
                scheduledCalls = scheduledCalls.Union(callsForCustomersAssignedToMe);

                if (searchFilter.CallTypeId.HasValue)
                {
                    scheduledCalls = scheduledCalls
                        .Where(p => p.CallTypeId == searchFilter.CallTypeId.Value);
                }

                if (searchFilter.ScheduledDateFrom != null)
                {
                    var scheduledDateFrom = searchFilter.ScheduledDateFrom.Value.Date;

                    scheduledCalls = scheduledCalls
                        .Where(p => p.ToCallAt >= scheduledDateFrom);
                }

                if (searchFilter.ScheduledDateTo != null)
                {
                    var scheduledDateTo = searchFilter.ScheduledDateTo.Value.Date.AddSeconds((24 * 60 * 60) - 1);// nr of seconds in a day;
                    scheduledCalls = scheduledCalls
                        .Where(p => p.ToCallAt <= scheduledDateTo);
                }

                if (!string.IsNullOrEmpty(searchFilter.CustomerName))
                {
                    scheduledCalls = scheduledCalls
                       .Where(p => (p.CustomerFirstName + " " + p.CustomerLastName).Contains(searchFilter.CustomerName));
                }

                if (!string.IsNullOrEmpty(searchFilter.ReasonForCalling))
                {
                    scheduledCalls = scheduledCalls
                        .Where(p => p.ReasonToCall.Contains(searchFilter.ReasonForCalling));
                }

                scheduledCalls = scheduledCalls
                  .OrderBy(p => p.ToCallAt);

                if (searchFilter.Take.HasValue)
                {
                    calls.NoOfScheduledCalls = scheduledCalls.Count();
                    calls.Calls = scheduledCalls.Take(searchFilter.Take.Value).ToList();
                }
                else
                {
                    calls.NoOfScheduledCalls = scheduledCalls.Count();
                    calls.Calls = scheduledCalls.ToList();
                }

                return calls;
            }
        }

        private IQueryable<int> GetCallsWithNoCSR(short branch)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Call
                        .Where(p => p.Branch == branch &&
                               p.SalesPersonId == null &&
                               p.CallClosedReasonId == null)
                       .Select(p => p.Id);
            }
        }

        public IQueryable<int> GetUnavailableCSR(short branch)
        {
            using (var scope = Context.Read())
            {
                return from csp in scope.Context.CustomerSalesPerson
                       join c in scope.Context.CsrUnavailable on csp.SalesPersonId equals c.SalesPersonId
                       where csp.CustomerBranch == branch
                       select c.SalesPersonId;
            }
        }

        private IQueryable<int> GetCallsFromUnavailableCSR(short branch)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.CallsFromUnavailableCSRView
                    .Where(p => p.BranchNo == branch)
                    .Select(p => p.CallId);
            }
        }

        public void AllocateUnallocatedCalls(int[] selectedCalls, int salesPersonId, IEventStore audit)
        {
            using (var scope = Context.Write())
            {
                var allCalls = scope.Context.Call
                    .Where(p => selectedCalls.Contains(p.Id))
                    .Select(p => p)
                    .ToList();

                foreach (var call in allCalls)
                {
                    call.SalesPersonId = salesPersonId;
                }

                audit.LogAsync(
                new
                  {
                      Calls = selectedCalls,
                      NewSalesPersonId = salesPersonId
                  },
                  EventType.AllocateCallsToCSR,
                  EventCategory.UpdateCall);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private List<int> GetListOfCallIds(CallSearchFilter searchFilter)
        {
            IQueryable<int> listOfInts = null;

            using (var scope = Context.Read())
            {
                IQueryable<int> listOfSalesPersonIds = null;
                var any = false;

                if (searchFilter.NoCSR)
                {
                    listOfInts = GetCallsWithNoCSR(searchFilter.Branch);
                    any = true;
                }

                if (searchFilter.UnavailableCSR)
                {
                    if (listOfInts != null)
                    {
                        listOfInts = listOfInts.Union(GetCallsFromUnavailableCSR(searchFilter.Branch));
                    }
                    else
                    {
                        listOfInts = GetCallsFromUnavailableCSR(searchFilter.Branch);
                    }
                    any = true;
                }

                if (searchFilter.LockedCSR)
                {
                    listOfSalesPersonIds = searchFilter.LockedCSRList.AsQueryable();

                    var callIds = from c in scope.Context.Call
                                  join csp in scope.Context.CustomerDetailsView on c.SalesPersonId equals csp.SalesPersonId
                                  where listOfSalesPersonIds.Contains(c.SalesPersonId.Value) &&
                                        c.CallClosedReasonId == null &&
                                        csp.DoNotCallAgain == false &&
                                        csp.BranchNo == searchFilter.Branch
                                  select c.Id;

                    if (listOfInts != null)
                    {
                        listOfInts = listOfInts.Union(callIds);
                    }
                    else
                    {
                        listOfInts = callIds;
                    }
                    any = true;
                }

                if (!any)
                {
                    listOfInts = from c in scope.Context.Call
                                 where c.CallClosedReasonId == null &&
                                       c.Branch == searchFilter.Branch
                                 select c.Id;
                }
            }

            return listOfInts == null ? new List<int>() : listOfInts.ToList();
        }

        public CustomerSalesPerson GetCSRDetails(string customerId)
        {
            using (var scope = Context.Write())
            {
                return scope.Context.CustomerSalesPerson
                    .Where(p => p.CustomerId == customerId)
                    .FirstOrDefault();
            }
        }

        #region SalesPersonTargets

        public void SaveCSRTargets(SalesPersonTargets salesPersonTargets, IEventStore audit)
        {
            using (var scope = Context.Write())
            {
                var existingSalesPerson = scope.Context.SalesPersonTargets
                    .Where(p => p.CreatedBy == salesPersonTargets.CreatedBy &&
                           p.Year == salesPersonTargets.Year)
                    .FirstOrDefault();

                if (existingSalesPerson == null)
                {
                    scope.Context.SalesPersonTargets.Add(salesPersonTargets);
                }
                else
                {
                    existingSalesPerson.TargetYear = salesPersonTargets.TargetYear;
                }

                audit.LogAsync(
               new
               {
                   SalesPersonTarget = salesPersonTargets
               },
                 EventType.SaveSalesPersonTargets,
                 EventCategory.SetSalesPersonTargets);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public List<SalesPersonTargets> GetSalesPersonTargets(int salesPersonId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.SalesPersonTargets
                    .Where(p => p.CreatedBy == salesPersonId)
                    .OrderByDescending(p => p.Year)
                    .ToList();
            }
        }

        #endregion

        #region Branch Manager Customers Search - Reallocate Customers

        public void AllocateCustomersToCSR(IList<CustomerSalesPerson> customers, IList<CustomerSalesPerson> oldCustomers, IEventStore audit)
        {
            using (var scope = Context.Write())
            {
                foreach (var item in customers)
                {
                    scope.Context.CustomerSalesPerson.Attach(item);
                    scope.Context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }

                var customerIds = customers.Select(p => p.CustomerId).ToList();

                var allCalls = scope.Context.Call
                    .Where(p => customerIds.Contains(p.CustomerId));

                foreach (var call in allCalls)
                {
                    var csrId = customers.Where(p => p.CustomerId == call.CustomerId &&
                                                 p.TempSalesPersonId == null)
                        .Select(p => p.SalesPersonId).FirstOrDefault();

                    var tempCSRId = customers.Where(p => p.CustomerId == call.CustomerId &&
                                                 p.TempSalesPersonId != null)
                        .Select(p => p.TempSalesPersonId).FirstOrDefault();

                    if (tempCSRId == null && call.CallClosedReasonId == null)
                    {
                        call.SalesPersonId = csrId;
                    }
                }

                audit.LogAsync(
               new
               {
                   UpdatedCustomers = customers,
                   OldCustomers = oldCustomers
               },
                 EventType.AllocateCustomerToCSR,
                 EventCategory.AllocateCustomerToCSR);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        #endregion

        #region Previous Calls

        public IList<PreviousCall> GetPreviousCallsForACustomer(string customerId, int numberOfCalls)
        {
            using (var scope = Context.Write())
            {
                return scope.Context.Call
                    .Where(p => p.CustomerId == customerId &&
                           p.CalledAt != null &&
                           p.CallClosedReasonId != null)
                    .GroupJoin(
                    scope.Context.Call,
                        right => right.Id,
                        left => left.PreviousCallId,
                        (main, reschedule) => new
                        {
                            Calls = main,
                            Reschedule = reschedule.DefaultIfEmpty()
                        })
                    .Select(p => new PreviousCall
                        {
                            CallTypeId = p.Calls.CallTypeId,
                            CallId = p.Calls.Id,
                            CalledAt = p.Calls.CalledAt,
                            Comments = p.Calls.Comments,
                            ReasonForCalling = p.Calls.ReasonToCall,
                            SalesPersonId = p.Calls.SalesPersonId.Value,
                            SpokeToCustomer = p.Calls.SpokeToCustomer,
                            RescheduledOn = p.Reschedule.FirstOrDefault().ToCallAt
                        })
                    .OrderByDescending(p => p.CalledAt)
                    .Take(numberOfCalls)
                    .ToList();
            }
        }

        public string GetCallTypeName(byte callTypeId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.CallType
                    .Where(p => p.Id == callTypeId)
                    .Select(p => p.Name)
                    .FirstOrDefault();
            }
        }

        #endregion

        #region Icons to inactive customer/account settlement calls

        public IList<IconTypes> GetIconTypes()
        {
            using (var scope = Context.Write())
            {
                return scope.Context.IconTypes
                    .ToList();
            }
        }

        public void SaveIconTypes(IList<IconTypes> iconTypes)
        {
            using (var scope = Context.Write())
            {
                foreach (var icon in iconTypes)
                {
                    scope.Context.IconTypes.Attach(icon);
                    scope.Context.Entry(icon).State = System.Data.Entity.EntityState.Modified;
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        #endregion

        #region Branch Manager - Unallocated Calls

        public ScheduledCalls GetBranchManagerUnallocatedCalls(CallSearchFilter searchFilter)
        {
            var calls = new ScheduledCalls();

            using (var scope = Context.Read())
            {
                var listOfInts = GetListOfCallIds(searchFilter);

                if (listOfInts != null)
                {
                    var branchManagerUnallocatedCalls = scope.Context.BranchManagerCall
                                                        .Select(p => p)
                                                        .Where(p => listOfInts.Contains(p.CallId) &&
                                                               (p.DoNotCallAgain.Value == false || p.DoNotCallAgain == null) &&
                                                               p.CustomerBranch.Value == searchFilter.Branch);

                    calls.NoOfScheduledCalls = branchManagerUnallocatedCalls.Count();

                    if (searchFilter.Take.HasValue)
                    {
                        branchManagerUnallocatedCalls = branchManagerUnallocatedCalls.Take(searchFilter.Take.Value);
                    }

                    calls.BranchManagerCalls = branchManagerUnallocatedCalls.ToList();
                }

                return calls;
            }
        }

        #endregion

        public Dictionary<string, int> GetSalesPersonIdByCustomerId(IList<string> customerIds)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.CustomerSalesPerson
                    .Where(p => customerIds.Contains(p.CustomerId))
                    .Select(p => new
                    {
                        CustomerId = p.CustomerId,
                        SalesPersonId = p.SalesPersonId
                    })
                    .ToDictionary(t => t.CustomerId, t => t.SalesPersonId);
            }
        }

        #region InactiveCustomersInteraction

        public IList<AdditionalCustomersInteraction> LoadInactiveCustomersInteraction(AdditionalCustomersInteraction.InteractionType interactionType = AdditionalCustomersInteraction.InteractionType.All)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.AdditionalCustomersInteraction
                    .Select(p=> p);

                if (interactionType != AdditionalCustomersInteraction.InteractionType.All)
                {
                    query = query
                        .Where(p=> p.Id == (byte)interactionType);
                }

                return query.ToList();
            }
        }

        public void UpdateInactiveCustomersInteraction(AdditionalCustomersInteraction value)
        {
            using (var scope = Context.Write())
            {
                scope.Context.Entry<AdditionalCustomersInteraction>(value).State = EntityState.Modified;

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        #endregion

        #region MailsToSend

        public void InsertMailsToSend(MailsToSend value)
        {
            InsertMailsToSend(new List<MailsToSend>() { value });
        }

        public void InsertMailsToSend(IList<MailsToSend> value)
        {
            using (var scope = Context.Write())
            {
                InsertMailsToSend(value, scope);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void InsertMailsToSend(IList<MailsToSend> value, WriteScope<Context> scope)
        {
            scope.Context.MailsToSend.AddRange(value);
            audit.Log(
                        new
                        {
                            Recipients = value.Select(p => p.CustomerId).ToArray()
                        },
                        EventType.ScheduleEmailsToSend);
        }

        public IList<MailsToSend> LoadMailsToSend(DateTime? sendingDate)
        {
            using (var scope = Context.Read())
            {
                var result = scope.Context.MailsToSend
                    .Select(p => p);

                if (sendingDate != null)
                {
                    var to = sendingDate.Value.Date.AddSeconds((24 * 60 * 60) - 1);

                    result = result
                        .Where(p => p.DateToSend <= to);
                }

                return result.ToList();
            }
        }

        public void DeleteMailsToSend(IList<int> ids)
        {
            using (var scope = Context.Write())
            {
                scope.Context.MailsToSend.RemoveRange(scope.Context.MailsToSend.Where(p => ids.Contains(p.id)));
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        #endregion

        #region SmsToSend

        public void InsertSmsToSend(IList<SmsToSend> value)
        {
            using (var scope = Context.Write())
            {
                InsertSmsToSend(value, scope);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void InsertSmsToSend(IList<SmsToSend> value, WriteScope<Context> scope)
        {
            scope.Context.SmsToSend.AddRange(value);
            audit.Log(
                        new
                        {
                            Recipients = value.Select(p => p.CustomerId).ToArray()
                        },
                        EventType.ScheduleSmsToSend);
        }

        public IList<SmsToSend> LoadSmsToSend(DateTime? sendingDate)
        {
            using (var scope = Context.Read())
            {
                var result = scope.Context.SmsToSend
                    .Select(p => p);

                if (sendingDate != null)
                {
                    var to = sendingDate.Value.Date.AddSeconds((24 * 60 * 60) - 1);

                    result = result
                        .Where(p => p.DateToSend <= to);
                }

                return result.ToList();
            }
        }

        public void DeleteSmsToSend(IList<int> ids)
        {
            using (var scope = Context.Write())
            {
                scope.Context.SmsToSend.RemoveRange(scope.Context.SmsToSend.Where(p => ids.Contains(p.Id)));
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        #endregion

        #region SummaryTable

        public void InsertSummaryTable(List<SummaryTable> values)
        {
            using (var scope = Context.Write())
            {
                scope.Context.SummaryTable.AddRange(values);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        #endregion
    }
}
