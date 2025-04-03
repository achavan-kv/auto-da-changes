//using Blue.Cosacs.SalesManagement.Repositories;
//using Blue.Cosacs.SalesManagement.Test.Mocks;
//using Blue.Networking;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Blue.Cosacs.SalesManagement.Test.Hub.Subscribers
//{
//    public class Registry : StructureMap.Configuration.DSL.Registry
//    {
//        public Registry()
//        {
//            For<IClock>().Singleton().Add<ClockMock>();
//            For<ISalesManagementRepository>().Add<SalesManagementRepositoryMock>();
//            For<IHttpClientJson>().Add<HttpClientJsonMock>();
//        }
//    }
//}
