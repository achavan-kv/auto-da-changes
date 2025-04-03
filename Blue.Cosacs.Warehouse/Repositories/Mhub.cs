using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.Messages.Warehouse;
using Blue.Hub.Client;

namespace Blue.Cosacs.Warehouse
{
    using Blue.Cosacs.Messages.Merchandising.BookingMessage;

    //strange conventions you guys have around here
    public class Mhub
    {
        public Mhub() : this(StructureMap.ObjectFactory.GetInstance<IPublisher>())
        {
        }

        public Mhub(IPublisher publisher)
        {
            this.publisher = publisher;
        }

        private readonly IPublisher publisher;

        public void Deliver(BookingMessage message)
        {
            publisher.Publish<Context, BookingMessage>("Merchandising.Booking.Receive", message);
        }

        public void Cancel(BookingMessage message)
        {
            publisher.Publish<Context, BookingMessage>("Merchandising.Booking.Cancel", message);
        } 
    }
}
