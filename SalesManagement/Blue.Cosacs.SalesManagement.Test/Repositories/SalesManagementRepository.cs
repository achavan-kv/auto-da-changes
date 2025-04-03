//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Moq;
//using NUnit.Framework;
//using StructureMap;
//using Blue.Cosacs.SalesManagement.Repositories;
//using CsvHelper;
//using System.Data.Entity;
//using Blue.Transactions;
//using Blue.Events;
//using Blue.Cosacs.SalesManagement.Hub.Subscribers;
//using Blue.Networking;
//using Blue.Cosacs.SalesManagement.Test.Mocks;

//namespace Blue.Cosacs.SalesManagement.Test.Repositories
//{
//   // [TestFixture]
//    public class SalesManagementRepository
//    {
//       // [TestFixtureSetUp]
//        public virtual void Setup()
//        {
//            ObjectFactory.Initialize(p =>
//                {
//                    p.AddRegistry(new Blue.Cosacs.SalesManagement.Test.Repositories.Registry());
//                });
//        }

//       // [Test]
//        public void GetCallTypes_ReturnCallTypes()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();

//            var path = @"..\..\CsvSources\Repositories\GetCallTypes\CallType.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();


//            var mockContext = new Mock<SalesManagement.Context>();
//            var callTypeData = CsvReader.Reader<SalesManagement.CallType>.Read(path).AsQueryable();
//            var mockCallType = new Mock<System.Data.Entity.DbSet<SalesManagement.CallType>>();
//            mockCallType.SetupQuerableMethods(callTypeData);

//            mockContext.Setup(c => c.CallType).Returns(mockCallType.Object);
//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var result = salesManagementRepository.GetCallTypes();
//            Assert.That(result.Count(), Is.GreaterThan(0));
//        }

//        #region GetScheduledCalls
//    //    [Test]
//        public void GetScheduledCalls_SearchByAllFilters_ReturnData()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetScheduledCalls\Call.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockCall = new Mock<System.Data.Entity.DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            var callSearchFilter = new CallSearchFilter()
//            {
//                CallTypeId = 5,
//                CustomerName = "FirstName",
//                ReasonForCalling = "reason",
//                ScheduledDateFrom = Convert.ToDateTime("2014-10-11"),
//                ScheduledDateTo = Convert.ToDateTime("2014-10-16")
//            };

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var result = salesManagementRepository.GetScheduledCalls(callSearchFilter);
//            Assert.That(result.Count(), Is.EqualTo(2));
//        }

//      //  [Test]
//        public void GetScheduledCalls_SearchByCallType_Callback_ReturnCallbackData()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetScheduledCalls\Call.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            var callSearchFilter = new CallSearchFilter()
//            {
//                CallTypeId = 8
//            };

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var result = salesManagementRepository.GetScheduledCalls(callSearchFilter);
//            Assert.That(result.Count(), Is.EqualTo(1));
//        }

//       // [Test]
//        public void GetScheduledCalls_SearchByScheduledDateFrom_ReturnData()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetScheduledCalls\Call.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            var callSearchFilter = new CallSearchFilter()
//            {
//                ScheduledDateFrom = Convert.ToDateTime("2014-10-11")
//            };

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var result = salesManagementRepository.GetScheduledCalls(callSearchFilter);
//            Assert.That(result.Count(), Is.GreaterThan(1));
//        }

//       // [Test]
//        public void GetScheduledCalls_SearchByScheduledDateTo_ReturnData()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetScheduledCalls\Call.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            var callSearchFilter = new CallSearchFilter()
//            {
//                ScheduledDateTo = Convert.ToDateTime("2014-10-15")
//            };

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var result = salesManagementRepository.GetScheduledCalls(callSearchFilter);
//            Assert.That(result.Count(), Is.GreaterThan(1));
//        }

//      //  [Test]
//        public void GetScheduledCalls_SearchByCustomerName_ReturnData()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetScheduledCalls\Call.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            var callSearchFilter = new CallSearchFilter()
//            {
//                CustomerName = "FirstName"
//            };

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var result = salesManagementRepository.GetScheduledCalls(callSearchFilter);
//            Assert.That(result.Count(), Is.GreaterThan(1));
//        }

//      //  [Test]
//        public void GetScheduledCalls_SearchByReasonForCalling_ReturnData()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetScheduledCalls\Call.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            var callSearchFilter = new CallSearchFilter()
//            {
//                ReasonForCalling = "reason"
//            };

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var result = salesManagementRepository.GetScheduledCalls(callSearchFilter);
//            Assert.That(result.Count(), Is.GreaterThan(1));
//        }

//        #endregion

//        #region GetCustomerSalesPerson

//      //  [Test]
//        public void GetCustomersSalesPerson_ReturnAllCustomers()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetCustomerSalesPerson\CustomerSalesPerson.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var customerSalesPersonData = CsvReader.Reader<SalesManagement.CustomerSalesPerson>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.CustomerSalesPerson>>();
//            mockCall.SetupQuerableMethods(customerSalesPersonData);

//            mockContext.Setup(c => c.CustomerSalesPerson).Returns(mockCall.Object);

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var result = salesManagementRepository.GetCustomersSalesPerson(null, null);

//            Assert.That(result.Count(), Is.GreaterThan(1));
//        }

//      //  [Test]
//        public void GetCustomersSalesPerson_ReturnExistingCustomers()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetCustomerSalesPerson\CustomerSalesPerson.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var customerSalesPersonData = CsvReader.Reader<SalesManagement.CustomerSalesPerson>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.CustomerSalesPerson>>();
//            mockCall.SetupQuerableMethods(customerSalesPersonData);

//            mockContext.Setup(c => c.CustomerSalesPerson).Returns(mockCall.Object);

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var customers = new List<string>();
//            customers.Add("Test2");
//            customers.Add("Test3");

//            var result = salesManagementRepository.GetCustomersSalesPerson(null, customers);

//            Assert.That(result.Count(), Is.EqualTo(2));
//        }

//       // [Test]
//        public void GetCustomersSalesPerson_ReturnNoCustomers()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetCustomerSalesPerson\CustomerSalesPerson.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var customerSalesPersonData = CsvReader.Reader<SalesManagement.CustomerSalesPerson>.Read(path).AsQueryable();
//            var mockCustomerSalesPerson = new Mock<DbSet<SalesManagement.CustomerSalesPerson>>();
//            mockCustomerSalesPerson.SetupQuerableMethods(customerSalesPersonData);

//            mockContext.Setup(c => c.CustomerSalesPerson).Returns(mockCustomerSalesPerson.Object);

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var customers = new List<string>();
//            customers.Add("ABC");

//            var result = salesManagementRepository.GetCustomersSalesPerson(null, customers);

//            Assert.That(result.Count(), Is.EqualTo(0));
//        }

//        #endregion

//        #region GetCustomersWithAutoScheduledCalls

//     //   [Test]
//        public void GetCustomersWithAutoScheduledCalls_ReturnNoCustomers()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetCustomersWithAutoScheduledCalls\Call.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var startDate = Convert.ToDateTime("2014-10-20");
//            var endDate = Convert.ToDateTime("2014-10-20");

//            var result = salesManagementRepository.GetCustomersWithAutoScheduledCalls(startDate, endDate);

//            Assert.That(result.Count(), Is.EqualTo(0));
//        }

//      //  [Test]
//        public void GetCustomersWithAutoScheduledCalls_ReturnCustomersWithAutoScheduledCalls()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetCustomersWithAutoScheduledCalls\Call.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var startDate = Convert.ToDateTime("2014-10-13");
//            var endDate = Convert.ToDateTime("2014-10-13");

//            var result = salesManagementRepository.GetCustomersWithAutoScheduledCalls(startDate, endDate);

//            Assert.That(result.Count(), Is.GreaterThan(1));
//        }

//        #endregion

//      //  [Test]
//        public void GetCustomerWithCallsFromSource_ReturnInactiveCustomers()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetCustomerWithCallsFromSource\Call.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var source = CallSourceEnum.InactiveCustomers;

//            var result = salesManagementRepository.GetCustomerWithCallsFromSource(source);

//            Assert.That(result.Count(), Is.GreaterThan(1));
//        }

//        #region GetCall
//       // [Test]
//        public void GetCall_ReturnCall()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetCall\Call.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var result = salesManagementRepository.GetCall(1);

//            Assert.That(result.Id, Is.EqualTo(1));
//        }

//       // [Test]
//        public void GetCall_ReturnNoCall()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetCall\Call.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//            var result = salesManagementRepository.GetCall(0);

//            Assert.IsNull(result);
//        }

//        #endregion

//      //  [Test]
//        public void InsertCustomersSalesPerson()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();

//            var mockWriteScope = new Mock<WriteScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var mockCustomerSalesPerson = new Mock<DbSet<SalesManagement.CustomerSalesPerson>>();

//            mockContext.Setup(c => c.CustomerSalesPerson).Returns(mockCustomerSalesPerson.Object);

//            mockWriteScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<WriteScope<SalesManagement.Context>>(mockWriteScope.Object);

//            var customerSalesPersonList = new List<CustomerSalesPerson>();

//            var customerSalesPerson = new CustomerSalesPerson()
//            {
//                CustomerId = "1",
//                SalesPersonId = 1,
//                TempSalesPersonId = null,
//                TempSalesPersonIdBegin = null,
//                PhoneNumber = "222222",
//                DoNotCallAgain = true,
//                TempSalesPersonIdEnd = null
//            };

//            customerSalesPersonList.Add(customerSalesPerson);

//            salesManagementRepository.InsertCustomersSalesPerson(customerSalesPersonList);

//            mockCustomerSalesPerson.Verify(x => x.AddRange(It.IsAny<IList<CustomerSalesPerson>>()), Times.Once());
//            mockContext.Verify(x => x.SaveChanges(), Times.Once());
//        }

//        // [Test]
//        public void SaveCalls()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();

//            var mockWriteScope = new Mock<WriteScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            mockWriteScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<WriteScope<SalesManagement.Context>>(mockWriteScope.Object);

//            var callsList = new List<Call>();

//            var call = new Call()
//            {
//                Id = 5,
//                CallTypeId = 1,
//                ReasonToCall = "reason",
//                ToCallAt = Convert.ToDateTime("2014-10-22"),
//                SpokeToCustomer = false,
//                CreatedOn = Convert.ToDateTime("2014-10-19"),
//                Source = 1
//            };

//            callsList.Add(call);

//            salesManagementRepository.SaveCalls(callsList);

//            mockCall.Verify(x => x.AddRange(It.IsAny<IList<Call>>()), Times.Once());
//            mockContext.Verify(x => x.SaveChanges(), Times.Once());
//        }

//        #region DeleteCustomersWithAutoScheduledCalls

//     //   [Test]
//        public void DeleteCustomersWithAutoScheduledCalls_AddCustomersList()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();

//            var path = @"..\..\CsvSources\Repositories\DeleteCustomersWithAutoScheduledCalls\Call.csv";

//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockWriteScope = new Mock<WriteScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            mockWriteScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<WriteScope<SalesManagement.Context>>(mockWriteScope.Object);

//            var startDate = Convert.ToDateTime("2014-10-12");
//            var customers = new List<string>();
//            customers.Add("1");

//            salesManagementRepository.DeleteCustomersWithAutoScheduledCalls(startDate, customers);

//            mockCall.Verify(x => x.RemoveRange(It.IsAny<IEnumerable<Call>>()), Times.Once());
//            mockContext.Verify(x => x.SaveChanges(), Times.Once());

//        }

//       // [Test]
//        public void DeleteCustomersWithAutoScheduledCalls_NoCustomersList()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();

//            var path = @"..\..\CsvSources\Repositories\DeleteCustomersWithAutoScheduledCalls\Call.csv";

//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockWriteScope = new Mock<WriteScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            mockWriteScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<WriteScope<SalesManagement.Context>>(mockWriteScope.Object);

//            var startDate = Convert.ToDateTime("2014-10-12");

//            salesManagementRepository.DeleteCustomersWithAutoScheduledCalls(startDate);

//            mockCall.Verify(x => x.RemoveRange(It.IsAny<IEnumerable<Call>>()), Times.Once());
//            mockContext.Verify(x => x.SaveChanges(), Times.Once());

//        }

//        #endregion

//       // [Test]
//        public void UpdateCustomersSalesPerson()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();

//            var path = @"..\..\CsvSources\Repositories\UpdateCustomersSalesPerson\CustomerSalesPerson.csv";

//            var customerSalesPersonData = CsvReader.Reader<SalesManagement.CustomerSalesPerson>.Read(path).AsQueryable();
//            var mockWriteScope = new Mock<WriteScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var mockCustomerSalesPerson = new Mock<DbSet<SalesManagement.CustomerSalesPerson>>();
//            mockCustomerSalesPerson.SetupQuerableMethods(customerSalesPersonData);

//            mockContext.Setup(c => c.CustomerSalesPerson).Returns(mockCustomerSalesPerson.Object);

//            mockWriteScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<WriteScope<SalesManagement.Context>>(mockWriteScope.Object);

//            var allocatedCustomers = new Dictionary<string, CustomerSalesPerson>();

//            var customerSalesPerson = new CustomerSalesPerson()
//            {
//                CustomerId = "1",
//                SalesPersonId = 1,
//                TempSalesPersonId = null,
//                TempSalesPersonIdBegin = null,
//                PhoneNumber = "222222",
//                DoNotCallAgain = true,
//                TempSalesPersonIdEnd = null
//            };

//            allocatedCustomers.Add("1", customerSalesPerson);

//            salesManagementRepository.UpdateCustomersSalesPerson(allocatedCustomers);

//            mockContext.Verify(x => x.SaveChanges(), Times.Once());
//        }

//        #region LogCall

//        // [Test]
//        public void LogCall_InsertCall()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\LogCall\Call.csv";

//            var mockWriteScope = new Mock<WriteScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            mockWriteScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<WriteScope<SalesManagement.Context>>(mockWriteScope.Object);

//            var mockAudit = new Mock<IEventStore>();

//            var insertCall = new Call()
//            {
//                Id = 5,
//                CallTypeId = 1,
//                ReasonToCall = "reason",
//                ToCallAt = Convert.ToDateTime("2014-10-22"),
//                SpokeToCustomer = false,
//                CreatedOn = Convert.ToDateTime("2014-10-19"),
//                Source = 1
//            };

//            var updateCall = new Call()
//            {
//                Id = 1,
//                CalledAt = Convert.ToDateTime("2014-10-24"),
//                SpokeToCustomer = false,
//                Comments = "comments"
//            };

//            var pendingCalls = new int[0];

//            var customerSalesPerson = new CustomerSalesPerson()
//            {
//                CustomerId = "1",
//                DoNotCallAgain = false
//            };

//            salesManagementRepository.LogCall(updateCall, insertCall, pendingCalls, mockAudit.Object, customerSalesPerson);

//            mockCall.Verify(x => x.Add(It.IsAny<Call>()), Times.Once());
//            mockContext.Verify(x => x.SaveChanges(), Times.Once());
//        }

//        //[Test]
//        public void LogCall_UpdateCall_UpdateBulkCalls()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\LogCall\Call.csv";

//            var mockWriteScope = new Mock<WriteScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            mockWriteScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<WriteScope<SalesManagement.Context>>(mockWriteScope.Object);

//            var mockAudit = new Mock<IEventStore>();

//            var insertCall = new Call();

//            var updateCall = new Call()
//            {
//                Id = 1,
//                CalledAt = Convert.ToDateTime("2014-10-24"),
//                SpokeToCustomer = false,
//                Comments = "comments"
//            };

//            var pendingCalls = new int[2] { 1, 2 };

//            var customerSalesPerson = new CustomerSalesPerson()
//            {
//                CustomerId = "1",
//                DoNotCallAgain = false
//            };

//            salesManagementRepository.LogCall(updateCall, insertCall, pendingCalls, mockAudit.Object, customerSalesPerson);

//            mockCall.Verify(x => x.Add(It.IsAny<Call>()), Times.Once());
//            mockContext.Verify(x => x.SaveChanges(), Times.Once());
//        }

//        // [Test]
//        public void LogCall_UpdateCall_UpdateBulkCalls_UpdateDoNotCallAgain()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\LogCall\Call.csv";

//            var mockWriteScope = new Mock<WriteScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            mockWriteScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<WriteScope<SalesManagement.Context>>(mockWriteScope.Object);

//            var mockAudit = new Mock<IEventStore>();

//            var insertCall = new Call();

//            var updateCall = new Call()
//            {
//                Id = 1,
//                CalledAt = Convert.ToDateTime("2014-10-24"),
//                SpokeToCustomer = false,
//                Comments = "comments"
//            };

//            var pendingCalls = new int[2] { 1, 2 };

//            var customerSalesPerson = new CustomerSalesPerson()
//            {
//                CustomerId = "1",
//                DoNotCallAgain = true
//            };

//            salesManagementRepository.LogCall(updateCall, insertCall, pendingCalls, mockAudit.Object, customerSalesPerson);

//            mockCall.Verify(x => x.Add(It.IsAny<Call>()), Times.Once());
//            mockContext.Verify(x => x.SaveChanges(), Times.Once());
//        }

//        #endregion

//      //  [Test]
//        public void UpdateAutoScheduledCalls()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\UpdateAutoScheduledCalls\Call.csv";

//            var mockWriteScope = new Mock<WriteScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            mockCall.SetupQuerableMethods(callData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            mockWriteScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<WriteScope<SalesManagement.Context>>(mockWriteScope.Object);

//            var startDate = Convert.ToDateTime("2014-10-12");
//            var endDate = Convert.ToDateTime("2014-10-17");
//            var customers = new Dictionary<string, DateTime>();
//            customers.Add("1", Convert.ToDateTime("2014-10-14"));
//            customers.Add("2", Convert.ToDateTime("2014-10-14"));

//            salesManagementRepository.UpdateAutoScheduledCalls(startDate, endDate, customers);

//            mockContext.Verify(x => x.SaveChanges(), Times.Once());
//        }

//        #region FollowUpCalls

//        //  [Test]
//        public void SaveFollowUpCall()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();

//            var mockWriteScope = new Mock<WriteScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var mockFollowUpCall = new Mock<DbSet<SalesManagement.FollowUpCall>>();

//            mockContext.Setup(c => c.FollowUpCall).Returns(mockFollowUpCall.Object);

//            mockWriteScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<WriteScope<SalesManagement.Context>>(mockWriteScope.Object);

//            var followUpCall = new FollowUpCall()
//            {
//                Id = 1,
//                Quantity = 1,
//                ReasonToCall = "reason",
//                TimePeriod = 1
//            };

//            salesManagementRepository.SaveFollowUpCall(followUpCall);

//            mockFollowUpCall.Verify(x => x.Add(It.IsAny<FollowUpCall>()), Times.Once());
//            mockContext.Verify(x => x.SaveChanges(), Times.Once());
//        }

//        #endregion

//        #region Branch Manager - Unallocated Calls

//      //  [Test]
//        public void GetBranchManagerUnallocatedCalls_NoSearchFilterChecked_ReturnNoUnallocatedCalls()
//        {
//            GetBranchManagerUnallocatedCalls_Setup();
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();

//            var searchFilter = new CallSearchFilter();
//            searchFilter.LockedCSRList = new int[] { 123 };
//            searchFilter.Branch = 760;
//            var result = salesManagementRepository.GetBranchManagerUnallocatedCalls(searchFilter);

//            Assert.That(result.NoOfScheduledCalls, Is.EqualTo(0));
//        }

//       // [Test]
//        public void GetBranchManagerUnallocatedCalls_NoSearchFilterChecked_ReturnNoCalls()
//        {
//            GetBranchManagerUnallocatedCalls_Setup();
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();

//            var searchFilter = new CallSearchFilter();
//            searchFilter.LockedCSRList = new int[] { 1234 };
//            var result = salesManagementRepository.GetBranchManagerUnallocatedCalls(searchFilter);

//            Assert.That(result.NoOfScheduledCalls, Is.EqualTo(0));
//        }

//        private void GetBranchManagerUnallocatedCalls_Setup()
//        {
//            var salesManagementRepository = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Repositories.SalesManagementRepository>();
//            var path = @"..\..\CsvSources\Repositories\GetBranchManagerUnallocatedCalls\Call.csv";
//            var customerSalesPersonPath = @"..\..\CsvSources\Repositories\GetBranchManagerUnallocatedCalls\CustomerSalesPerson.csv";
//            var callsFromUnavailableCSRViewPath = @"..\..\CsvSources\Repositories\GetBranchManagerUnallocatedCalls\CallsFromUnavailableCSRView.csv";
//            var customerDetailsView = @"..\..\CsvSources\Repositories\GetBranchManagerUnallocatedCalls\CustomerDetailsView.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(path).AsQueryable();
//            var mockCall = new Mock<DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);

//            var customerSalesPersonData = CsvReader.Reader<SalesManagement.CustomerSalesPerson>.Read(customerSalesPersonPath).AsQueryable();
//            var mockCustomerSalesPerson = new Mock<DbSet<SalesManagement.CustomerSalesPerson>>();
//            mockCustomerSalesPerson.SetupQuerableMethods(customerSalesPersonData);

//            var callsFromUnavailableCSRView = CsvReader.Reader<SalesManagement.CallsFromUnavailableCSRView>.Read(callsFromUnavailableCSRViewPath).
//                                                                                                            AsQueryable();
//            var mockView = new Mock<DbSet<SalesManagement.CallsFromUnavailableCSRView>>();
//            mockView.SetupQuerableMethods(callsFromUnavailableCSRView);

//            var customerDetailsViewData = CsvReader.Reader<SalesManagement.CustomerDetailsView>.Read(customerDetailsView).AsQueryable();
//            var mockCustomerDetailsView = new Mock<DbSet<SalesManagement.CustomerDetailsView>>();
//            mockCustomerDetailsView.SetupQuerableMethods(customerDetailsViewData);

//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);
//            mockContext.Setup(c => c.CustomerSalesPerson).Returns(mockCustomerSalesPerson.Object);
//            mockContext.Setup(c => c.CallsFromUnavailableCSRView).Returns(mockView.Object);
//            mockContext.Setup(c => c.CustomerDetailsView).Returns(mockCustomerDetailsView.Object);
//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);

//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);

//        }
//        #endregion
//    }
//}
