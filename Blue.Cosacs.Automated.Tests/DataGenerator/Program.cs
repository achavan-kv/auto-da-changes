using System;

namespace DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var bookingGenerator = new BookingGenerator();
            bookingGenerator.InsertBookings();
            Console.ReadLine();
        }
    }
}
