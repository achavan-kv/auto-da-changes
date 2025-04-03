using System.ComponentModel.DataAnnotations.Schema;

namespace Blue.Cosacs.Warehouse
{
    using System;

    public partial class BookingPendingView : IDeliverable
    {
        [NotMapped]
        public string OrderedOnString
        {
            get
            {
                return OrderedOn.ToString();
            }
        }

        [NotMapped]
        public int BookingId
        {
            get { return Id; }
            set { BookingId = value; }
        }

        [NotMapped]
        public string DeliveryOrCollectionDescription
        {
            get { return Utils.DeliveryOrCollection.FromString(DeliveryOrCollection).Name; }
        }
    }
}
