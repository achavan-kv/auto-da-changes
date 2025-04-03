//using Blue.Cosacs.SalesManagement.Messages;
//using Blue.Cosacs.SalesManagement.Repositories;
//using Blue.Networking;
//using Moq;
//using NUnit.Framework;
//using StructureMap;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;

//namespace Blue.Cosacs.SalesManagement.Test.Hub.Subscribers
//{
//    [TestFixture]
//    public class ScheduleFollowUpCalls
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
//        public void GetCallDate_ReturnCallDate()
//        {
//            var scheduleFollowUpCalls = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Hub.Subscribers.ScheduleFollowUpCalls>();

//            var deliveryDate = Convert.ToDateTime("2014-12-22");

//            var date = scheduleFollowUpCalls.GetCallDate(FollowUpCallsTimePeriods.Day, 1, deliveryDate);

//            Assert.NotNull(date);
//        }

//        [Test]
//        public void GetCallDate_ReturnNoDate()
//        {
//            var scheduleFollowUpCalls = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Hub.Subscribers.ScheduleFollowUpCalls>();

//            var deliveryDate = new DateTime();

//            var date = scheduleFollowUpCalls.GetCallDate(FollowUpCallsTimePeriods.Day, 0, deliveryDate);

//            Assert.That(date, Is.EqualTo(new DateTime()));
//        }

//        [Test]
//        [ExpectedException(typeof(NotImplementedException))]
//        public void GetCallDate_ReturnNotImplementedException()
//        {
//            var scheduleFollowUpCalls = ObjectFactory.GetInstance<Blue.Cosacs.SalesManagement.Hub.Subscribers.ScheduleFollowUpCalls>();

//            var deliveryDate = new DateTime();

//            var date = scheduleFollowUpCalls.GetCallDate(new FollowUpCallsTimePeriods(), 0, deliveryDate);

//            Assert.Throws<NotImplementedException>(() => date.ToShortDateString());
//        }

//        [Test]
//        public void InsertFollowUpCalls_InsertNewCSR()
//        {
//            var mockRepository = new Mock<ISalesManagementRepository>();

//            var customer = new SalesManagement.Hub.Subscribers.Customer()
//            {
//                CustomerId = "1",
//                CustomerAccount = "76111132",
//                FirstName = "aa",
//                LastName = "bb",
//                Phone = "111",
//                SalesPerson = 123
//            };

//            var warehouseDeliver = new SalesManagement.Messages.WarehouseDeliver()
//            {
//                Date = Convert.ToDateTime("2014-12-11")
//            };

//            mockRepository.Setup(x => x.GetCustomersSalesPerson(It.IsAny<bool?>(), It.IsAny<IList<string>>())).Returns(new List<CustomerSalesPerson>().AsQueryable());
//            mockRepository.Setup(x => x.InsertCustomersSalesPerson(It.IsAny<IList<CustomerSalesPerson>>()));
//            mockRepository.Setup(x => x.GetFollowUpCalls()).Returns(new List<FollowUpCall>());

//            var scheduleFollowUpCalls = new Blue.Cosacs.SalesManagement.Hub.Subscribers.ScheduleFollowUpCalls(
//                ObjectFactory.Container.GetInstance<IClock>(),
//                mockRepository.Object,
//                ObjectFactory.Container.GetInstance<IHttpClientJson>());

//            scheduleFollowUpCalls.InsertFollowUpCalls(customer, warehouseDeliver);

//            mockRepository.Verify(x => x.SaveCalls(It.IsAny<IList<Call>>()), Times.Once());
//            mockRepository.Verify(x => x.InsertCustomersSalesPerson(It.IsAny<IList<CustomerSalesPerson>>()), Times.Once());
//        }

//        [Test]
//        public void InsertFollowUpCalls_UpdateCSR_WhenDoNotCallAgainIsFalse()
//        {
//            var mockRepository = new Mock<ISalesManagementRepository>();

//            var customer = new SalesManagement.Hub.Subscribers.Customer()
//            {
//                CustomerId = "1",
//                CustomerAccount = "76111132",
//                FirstName = "aa",
//                LastName = "bb",
//                Phone = "111",
//                SalesPerson = 123
//            };

//            var warehouseDeliver = new SalesManagement.Messages.WarehouseDeliver()
//            {
//                Date = Convert.ToDateTime("2014-12-11")
//            };

//            var customerSalesPerson = new CustomerSalesPerson()
//            {
//                CustomerBranch = 761,
//                CustomerId = "11",
//                DoNotCallAgain = false,
//                PhoneNumber = "1111",
//                SalesPersonId = 123,
//                TempSalesPersonId = null,
//                TempSalesPersonIdBegin = null,
//                TempSalesPersonIdEnd = null
//            };

//            var customersList = new List<CustomerSalesPerson>();

//            customersList.Add(customerSalesPerson);

//            mockRepository.Setup(x => x.GetCustomersSalesPerson(It.IsAny<bool?>(), It.IsAny<IList<string>>())).Returns(customersList.AsQueryable());
//            mockRepository.Setup(x => x.InsertCustomersSalesPerson(It.IsAny<IList<CustomerSalesPerson>>()));
//            mockRepository.Setup(x => x.GetFollowUpCalls()).Returns(new List<FollowUpCall>());

//            var scheduleFollowUpCalls = new Blue.Cosacs.SalesManagement.Hub.Subscribers.ScheduleFollowUpCalls(
//                ObjectFactory.Container.GetInstance<IClock>(),
//                mockRepository.Object,
//                ObjectFactory.Container.GetInstance<IHttpClientJson>());

//            scheduleFollowUpCalls.InsertFollowUpCalls(customer, warehouseDeliver);

//            mockRepository.Verify(x => x.SaveCalls(It.IsAny<IList<Call>>()), Times.Once());
//            mockRepository.Verify(x => x.InsertCustomersSalesPerson(It.IsAny<IList<CustomerSalesPerson>>()), Times.Exactly(0));
//        }

//        [Test]
//        public void InsertFollowUpCalls_DoNothing_WhenDoNotCallAgainIsTrue()
//        {
//            var mockRepository = new Mock<ISalesManagementRepository>();

//            #region Populate Objects
//            var customer = new SalesManagement.Hub.Subscribers.Customer()
//            {
//                CustomerId = "1",
//                CustomerAccount = "76111132",
//                FirstName = "aa",
//                LastName = "bb",
//                Phone = "111",
//                SalesPerson = 123
//            };

//            var warehouseDeliver = new SalesManagement.Messages.WarehouseDeliver()
//            {
//                Date = Convert.ToDateTime("2014-12-11")
//            };

//            var customerSalesPerson = new CustomerSalesPerson()
//            {
//                CustomerBranch = 761,
//                CustomerId = "11",
//                DoNotCallAgain = true,
//                PhoneNumber = "1111",
//                SalesPersonId = 123,
//                TempSalesPersonId = null,
//                TempSalesPersonIdBegin = null,
//                TempSalesPersonIdEnd = null
//            };

//            var customersList = new List<CustomerSalesPerson>();
//            customersList.Add(customerSalesPerson);
//#endregion

//            #region Setup
//            mockRepository.Setup(x => x.GetCustomersSalesPerson(It.IsAny<bool?>(), It.IsAny<IList<string>>())).Returns(customersList.AsQueryable());
//            mockRepository.Setup(x => x.InsertCustomersSalesPerson(It.IsAny<IList<CustomerSalesPerson>>()));

//            var scheduleFollowUpCalls = new Blue.Cosacs.SalesManagement.Hub.Subscribers.ScheduleFollowUpCalls(
//                ObjectFactory.Container.GetInstance<IClock>(),
//                mockRepository.Object,
//                ObjectFactory.Container.GetInstance<IHttpClientJson>());

//            #endregion

//            scheduleFollowUpCalls.InsertFollowUpCalls(customer, warehouseDeliver);

//            mockRepository.Verify(x => x.SaveCalls(It.IsAny<IList<Call>>()), Times.Exactly(0));
//            mockRepository.Verify(x => x.InsertCustomersSalesPerson(It.IsAny<IList<CustomerSalesPerson>>()), Times.Exactly(0));
//        }
//    }
//}
