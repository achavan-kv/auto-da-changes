//using Blue.Cosacs.SalesManagement.Api.Controllers;
//using Blue.Cosacs.SalesManagement.Hub.Subscribers;
//using Blue.Cosacs.SalesManagement.Repositories;
//using Blue.Networking;
//using Blue.Transactions;
//using Moq;
//using NUnit.Framework;
//using StructureMap;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Blue.Cosacs.SalesManagement.Test.Hub.Subscribers
//{
//    [TestFixture]
//    public class InactiveCustomersSubscriber
//    {
//        [TestFixtureSetUp]
//        public virtual void Setup()
//        {
//            ObjectFactory.Initialize(p =>
//            {
//                p.AddRegistry(new Blue.Cosacs.SalesManagement.Test.Hub.Subscribers.Registry());
//            });
//        }

//        [Test]
//        public void InsertCustomerCsr()
//        {
//            #region Setup
//            var mockRepository = new Mock<ISalesManagementRepository>();
//            var inactiveCustomers = new InactiveCustomersController(
//                   ObjectFactory.Container.GetInstance<IClock>(),
//                   mockRepository.Object,
//                   ObjectFactory.Container.GetInstance<IHttpClientJson>());

//            mockRepository.Setup(x => x.GetCustomersSalesPerson(It.IsAny<bool?>(), It.IsAny<IList<string>>())).Returns(new List<CustomerSalesPerson>().AsQueryable());

//            #endregion

//            #region Populate Objects

//            var allCustomers = new List<CustomerLastPurchase>();
//            var customer = new CustomerLastPurchase()
//            {
//                CustomerBranch = 761,
//                CustomerId = "123",
//                DateLastPaid = Convert.ToDateTime("2014-12-10"),
//                FirstName = "First Name",
//                LastName = "Last Name",
//                PhoneNumber = "123675",
//                SalesPerson = 124
//            };

//            allCustomers.Add(customer);

//            var customerSalesPerson = new Dictionary<string, int?>();
//            customerSalesPerson.Add("123", null);

//            #endregion

//            inactiveCustomers.InsertCustomerCsr(allCustomers, customerSalesPerson);

//            mockRepository.Verify(x => x.UpdateCustomersSalesPerson(It.IsAny<Dictionary<string, CustomerSalesPerson>>()), Times.Once());
//            mockRepository.Verify(x => x.InsertCustomersSalesPerson(It.IsAny<IList<CustomerSalesPerson>>()), Times.Once());
//        }

//        [Test]
//        public void GetCustomersToInsert_ReturnCustomers()
//        {
//            #region Setup
//            var mockRepository = new Mock<ISalesManagementRepository>();
//            var inactiveCustomers = new InactiveCustomersController(
//                   ObjectFactory.Container.GetInstance<IClock>(),
//                   mockRepository.Object,
//                   ObjectFactory.Container.GetInstance<IHttpClientJson>());

//            mockRepository.Setup(x => x.GetCustomersSalesPerson(It.IsAny<bool?>(), It.IsAny<IList<string>>())).Returns(new List<CustomerSalesPerson>().AsQueryable());

//            #endregion

//            #region Populate Objects

//            var allCustomers = new List<CustomerLastPurchase>();
//            var customer = new CustomerLastPurchase()
//            {
//                CustomerBranch = 761,
//                CustomerId = "123",
//                DateLastPaid = Convert.ToDateTime("2014-12-10"),
//                FirstName = "First Name",
//                LastName = "Last Name",
//                PhoneNumber = "123675",
//                SalesPerson = 124
//            };

//            allCustomers.Add(customer);

//            #endregion

//            var customers = inactiveCustomers.GetCustomersToInsert(allCustomers);

//            Assert.That(customers.Count(), Is.GreaterThan(0));
//        }

//        [Test]
//        public void GetCustomersToInsert_ReturnNoCustomers()
//        {
//            #region Setup
//            var mockRepository = new Mock<ISalesManagementRepository>();
//            var inactiveCustomers = new InactiveCustomersController(
//                   ObjectFactory.Container.GetInstance<IClock>(),
//                   mockRepository.Object,
//                   ObjectFactory.Container.GetInstance<IHttpClientJson>());

//            mockRepository.Setup(x => x.GetCustomersSalesPerson(It.IsAny<bool?>(), It.IsAny<IList<string>>())).Returns(new List<CustomerSalesPerson>().AsQueryable());

//            #endregion

//            var allCustomers = new List<CustomerLastPurchase>();

//            var customers = inactiveCustomers.GetCustomersToInsert(allCustomers);

//            Assert.That(customers.Count(), Is.EqualTo(0));
//        }

//        [Test]
//        public void GetCustomersSalesPerson_ReturnNoCustomers()
//        {
//            #region Setup
//            var mockRepository = new Mock<ISalesManagementRepository>();
//            var inactiveCustomers = new InactiveCustomersController(
//                   ObjectFactory.Container.GetInstance<IClock>(),
//                   mockRepository.Object,
//                   ObjectFactory.Container.GetInstance<IHttpClientJson>());

//            mockRepository.Setup(x => x.GetCustomersSalesPerson(It.IsAny<bool?>(), It.IsAny<IList<string>>())).Returns(new List<CustomerSalesPerson>().AsQueryable());

//            #endregion

//            var customerIds = new HashSet<string>();

//            var customers = inactiveCustomers.GetCustomersSalesParson(customerIds);

//            Assert.That(customers.Count(), Is.EqualTo(0));
//        }

//        [Test]
//        public void GetCustomersSalesPerson_ReturnCustomers()
//        {
//            #region Setup
//            var mockRepository = new Mock<ISalesManagementRepository>();
//            var inactiveCustomers = new InactiveCustomersController(
//                   ObjectFactory.Container.GetInstance<IClock>(),
//                   mockRepository.Object,
//                   ObjectFactory.Container.GetInstance<IHttpClientJson>());

//            mockRepository.Setup(x => x.GetCustomersSalesPerson(It.IsAny<bool?>(), It.IsAny<IList<string>>())).Returns(new List<CustomerSalesPerson>().AsQueryable());

//            #endregion

//            var allCustomers = new HashSet<string>()
//            {
//                "1",
//                "2",
//                "3",
//                "4"
//            };

//            var customers = inactiveCustomers.GetCustomersSalesParson(allCustomers);

//            Assert.That(customers.Count(), Is.GreaterThan(0));
//        }

//        [Test]
//        public void JoinCustomerWincosacsAndCalls_ReturnCustomers()
//        {
//            #region Setup
//            var mockRepository = new Mock<ISalesManagementRepository>();
//            var inactiveCustomers = new InactiveCustomersController(
//                   ObjectFactory.Container.GetInstance<IClock>(),
//                   mockRepository.Object,
//                   ObjectFactory.Container.GetInstance<IHttpClientJson>());
//            #endregion

//            #region Populate Objects

//            var salesMCustomers = new HashSet<CustomerLastPurchase>()
//            {
//                new CustomerLastPurchase()
//                {
//                    CustomerBranch = 761,
//                    CustomerId = "1",
//                    DateLastPaid = Convert.ToDateTime("2014-12-10"),
//                    FirstName = "a",
//                    LastName = "b",
//                    PhoneNumber = "11111",
//                    SalesPerson = 123
//                }
//            };

//            var wincosacsCustomers = new List<CustomerLastPurchase>()
//            {
//                new CustomerLastPurchase()
//                {
//                    CustomerBranch = 762,
//                    CustomerId = "2",
//                    DateLastPaid = Convert.ToDateTime("2014-12-12"),
//                    FirstName = "aa",
//                    LastName = "bb",
//                    PhoneNumber = "11111111",
//                    SalesPerson = 124
//                }
//            };

//            #endregion

//            var customers = inactiveCustomers.JoinCustomerWincosacsAndCalls(salesMCustomers, wincosacsCustomers);

//            Assert.That(customers.Count(), Is.EqualTo(2));
//        }

//        [Test]
//        public void JoinCustomerWincosacsAndCalls_ReturnNoCustomers()
//        {
//            #region Setup
//            var mockRepository = new Mock<ISalesManagementRepository>();
//            var inactiveCustomers = new InactiveCustomersController(
//                   ObjectFactory.Container.GetInstance<IClock>(),
//                   mockRepository.Object,
//                   ObjectFactory.Container.GetInstance<IHttpClientJson>());
//            #endregion

//            var salesMCustomers = new HashSet<CustomerLastPurchase>();
//            var wincosacsCustomers = new List<CustomerLastPurchase>();

//            var customers = inactiveCustomers.JoinCustomerWincosacsAndCalls(salesMCustomers, wincosacsCustomers);

//            Assert.That(customers.Count(), Is.EqualTo(0));
//        }

//        [Test]
//        public void GetCustomerWithNoCallsSince_ReturnNoData()
//        {
//            #region Setup
//            var mockRepository = new Mock<ISalesManagementRepository>();
//            var inactiveCustomers = new InactiveCustomersController(
//                   ObjectFactory.Container.GetInstance<IClock>(),
//                   mockRepository.Object,
//                   ObjectFactory.Container.GetInstance<IHttpClientJson>());

//            var callPath = @"..\..\CsvSources\Hub.Subscribers\GetCustomerWithNoCallsSince\Call.csv";
//            var customerSalesPersonPath = @"..\..\CsvSources\Hub.Subscribers\GetCustomerWithNoCallsSince\CustomerSalesPerson.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();

//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(callPath).AsQueryable();
//            var mockCall = new Mock<System.Data.Entity.DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);
//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            var customerSalesPersonData = CsvReader.Reader<SalesManagement.CustomerSalesPerson>.Read(customerSalesPersonPath).AsQueryable();
//            var mockCustomerSalesPerson = new Mock<System.Data.Entity.DbSet<SalesManagement.CustomerSalesPerson>>();
//            mockCustomerSalesPerson.SetupQuerableMethods(customerSalesPersonData);
//            mockContext.Setup(c => c.CustomerSalesPerson).Returns(mockCustomerSalesPerson.Object);

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);
//            #endregion

//            var lastDateCall = Convert.ToDateTime("2014-10-14");
//            var result = inactiveCustomers.GetCustomerWithNoCallsSince(lastDateCall);

//            Assert.That(result.Count(), Is.EqualTo(0));
//        }

//        [Test]
//        public void GetCustomerWithNoCallsSince_ReturnCustomersLastPurchase()
//        {
//            #region Setup
//            var mockRepository = new Mock<ISalesManagementRepository>();
//            var inactiveCustomers = new InactiveCustomersController(
//                   ObjectFactory.Container.GetInstance<IClock>(),
//                   mockRepository.Object,
//                   ObjectFactory.Container.GetInstance<IHttpClientJson>());

//            var callPath = @"..\..\CsvSources\Hub.Subscribers\GetCustomerWithNoCallsSince\Call.csv";
//            var customerSalesPersonPath = @"..\..\CsvSources\Hub.Subscribers\GetCustomerWithNoCallsSince\CustomerSalesPerson.csv";

//            var mockReadScope = new Mock<ReadScope<SalesManagement.Context>>();

//            var mockContext = new Mock<SalesManagement.Context>();
//            var callData = CsvReader.Reader<SalesManagement.Call>.Read(callPath).AsQueryable();
//            var mockCall = new Mock<System.Data.Entity.DbSet<SalesManagement.Call>>();
//            mockCall.SetupQuerableMethods(callData);
//            mockContext.Setup(c => c.Call).Returns(mockCall.Object);

//            var customerSalesPersonData = CsvReader.Reader<SalesManagement.CustomerSalesPerson>.Read(customerSalesPersonPath).AsQueryable();
//            var mockCustomerSalesPerson = new Mock<System.Data.Entity.DbSet<SalesManagement.CustomerSalesPerson>>();
//            mockCustomerSalesPerson.SetupQuerableMethods(customerSalesPersonData);
//            mockContext.Setup(c => c.CustomerSalesPerson).Returns(mockCustomerSalesPerson.Object);

//            mockReadScope.Setup(p => p.Context).Returns(mockContext.Object);
//            ObjectFactory.Inject<ReadScope<SalesManagement.Context>>(mockReadScope.Object);
//            #endregion

//            var lastDateCall = Convert.ToDateTime("2014-10-17");
//            var result = inactiveCustomers.GetCustomerWithNoCallsSince(lastDateCall);

//            Assert.That(result.Count(), Is.GreaterThan(0));
//        }

//       // [Test]
//        public void GetInactiveCustomersCallsIcon()
//        {
//            #region Setup

//            var inactiveCustomers = ObjectFactory.GetInstance<InactiveCustomersController>();
//            var path = @"..\..\CsvSources\Hub.Subscribers\GetInactiveCustomersCallsIcon\IconTypes.csv";
//            var mockWriteScope = new Mock<WriteScope<SalesManagement.Context>>();
//            var mockContext = new Mock<SalesManagement.Context>();
//            var iconTypeData = CsvReader.Reader<SalesManagement.IconTypes>.Read(path).AsQueryable();
//            var mockIconTypes = new Mock<System.Data.Entity.DbSet<SalesManagement.IconTypes>>();
//            mockIconTypes.SetupQuerableMethods(iconTypeData);
//            mockWriteScope.Setup(p => p.Context).Returns(mockContext.Object);
//            mockContext.Setup(c => c.IconTypes).Returns(mockIconTypes.Object);

//            ObjectFactory.Inject<WriteScope<SalesManagement.Context>>(mockWriteScope.Object);

//            #endregion

//            var deliveryDate = Convert.ToDateTime("2014-12-22");

//            var icon = inactiveCustomers.GetInactiveCustomersCallsIcon();

//            Assert.IsNotEmpty(icon);
//        }
//    }
//}
