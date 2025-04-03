using NUnit.Framework;
using System.Linq;
using W = Blue.Cosacs.Warehouse;

namespace Blue.Cosacs.Test.Warehouse
{
    [TestFixture]
    public class BookingTest
    {
        //[Test]
        public void Insert()
        {
            using (var scope = W.Context.Write())
            {
                scope.Context.Booking.Add(new W.Booking
                    {
                        Id = 1,
                        CustomerName = "Steve"
                    });
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }
    }
}
