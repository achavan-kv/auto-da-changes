using System;
using Blue;

namespace Blue.Cosacs.NonStocks.Test.Mocks
{
    internal class IClockMock : IClock
    {
        public DateTime Now
        {
            get
            {
                return new DateTime(
                    2015, DateTime.Now.Month, DateTime.Now.Day,
                    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second,
                    DateTime.Now.Millisecond);
            }
        }

        public DateTime UtcNow
        {
            get
            {
                return this.Now.ToUniversalTime();
            }
        }
    }
}
