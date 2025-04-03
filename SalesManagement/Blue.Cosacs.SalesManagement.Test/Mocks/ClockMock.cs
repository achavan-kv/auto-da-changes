//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Blue.Cosacs.SalesManagement.Test.Mocks
//{
//    internal class ClockMock : IClock
//    {
//        public DateTime Now
//        {
//            get
//            {
//                return new DateTime(
//                    2050, DateTime.Now.Month, DateTime.Now.Day,
//                    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second,
//                    DateTime.Now.Millisecond);
//            }
//        }

//        public DateTime UtcNow
//        {
//            get
//            {
//                return this.Now.ToUniversalTime();
//            }
//        }
//    }
//}
