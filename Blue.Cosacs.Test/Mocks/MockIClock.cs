using System;

namespace Blue.Cosacs.Test.Mocks
{
    internal class MockIClock : IClock
    {
        public DateTime Now
        {
            get
            {
                return DateTime.Now.AddYears(50);
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
