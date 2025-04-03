//using Blue.Cosacs.SalesManagement.Api.Controllers;
//using Blue.Cosacs.SalesManagement.Repositories;
//using Blue.Networking;
//using Moq;
//using NUnit.Framework;
//using StructureMap;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Http;

//namespace Blue.Cosacs.SalesManagement.Test.Controllers
//{
//    [TestFixture]
//    public class AllocateCustomersToCSR : ApiController
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
//        public void Get()
//        {
//            var mockHttpClientJson = new Mock<IHttpClientJson>();
//            var responseJson = new ResponseJson<List<AllocateCustomers>>();
//            responseJson.Body = new List<AllocateCustomers>();
//            Func<ResponseJson<List<AllocateCustomers>>> y = () => responseJson;

//            var createRequest = RequestJson<byte[]>.Create("/cosacs", WebRequestMethods.Http.Post);

//            mockHttpClientJson.Setup(c => c.Do<byte[], List<AllocateCustomers>>(
//                It.IsAny<RequestJson<byte[]>>()
//                )).Returns(y);

//            ObjectFactory.Inject<IHttpClientJson>(mockHttpClientJson.Object);

//            var httpClientJson = StructureMap.ObjectFactory.Container.GetInstance<IHttpClientJson>();
//            httpClientJson.Do<byte[], List<AllocateCustomers>>(createRequest);

//            var allocateCustomersToCSR = new Blue.Cosacs.SalesManagement.Api.Controllers.AllocateCustomersToCSRController(ObjectFactory.Container.GetInstance<IClock>(),
//               ObjectFactory.Container.GetInstance<ISalesManagementRepository>(),
//               ObjectFactory.Container.GetInstance<IHttpClientJson>());

//            allocateCustomersToCSR.Request = new System.Net.Http.HttpRequestMessage();
//            var result = allocateCustomersToCSR.Get();

//            Assert.IsTrue(result.IsSuccessStatusCode);
//        }

//        [Test]
//        public void GetAllocatedCustomersToInsert()
//        {
//            var mockHttpClientJson = new Mock<IHttpClientJson>();
//            ObjectFactory.Inject<IHttpClientJson>(mockHttpClientJson.Object);

//            var allocateCustomersToCSR = ObjectFactory.GetInstance<AllocateCustomersToCSRController>();

//            var customersFromWinCosacs = new List<AllocateCustomers>();
//            var customerFromWincosacs = new AllocateCustomers()
//            {
//                CustomerId = "1",
//                SalesPerson = 2
//            };

//            customersFromWinCosacs.Add(customerFromWincosacs);

//            var newAllocatedCustomers = new HashSet<string>();
//            newAllocatedCustomers.Add("1");

//            var result = allocateCustomersToCSR.GetAllocatedCustomersToInsert(customersFromWinCosacs, newAllocatedCustomers);

//            Assert.That(result.Count(), Is.GreaterThan(0));
//        }
//    }
//}
