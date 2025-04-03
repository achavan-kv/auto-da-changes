using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.Messages.Warehouse;
using Blue.Hub.Client;

namespace Blue.Cosacs.Warehouse
{
    //http://www.urbandictionary.com/define.php?term=chub
    public class Chub
    {
        public Chub() : this(StructureMap.ObjectFactory.GetInstance<IPublisher>()) { }

        public Chub(IPublisher publisher)
        {
            this.publisher = publisher;
        }

        private readonly IPublisher publisher;

        public void Deliver(Message<WarehouseDeliver> message)
        {
            publisher.Publish<Context,WarehouseDeliver>("Cosacs.Booking.Deliver", message.Payload, message.CorrelationId);
        }

        public void DeliverMany(IEnumerable<Message<WarehouseDeliver>> messages)
        {
            messages.ToList().ForEach(Deliver);
        }

        public void Cancel(Message<WarehouseCancel> message)
        {
            publisher.Publish<Context,WarehouseCancel>("Cosacs.Booking.Cancel", message.Payload, message.CorrelationId);
        } 
    }
}
