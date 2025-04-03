using System.ComponentModel.DataAnnotations;
using Blue.Cosacs.Warehouse.Utils;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blue.Cosacs.Warehouse
{
    public partial class BookingHistoryView
    {
        //[NotMapped]
        //public BookingAction DeliveryOrCollectionEnum
        //{
        //    get
        //    {
        //        return Blue.Cosacs.Warehouse.Utils.DeliveryOrCollectionHelpers.StringToBookingAction(this.DeliveryOrCollection);
        //    }
        //    set
        //    {
        //        this.DeliveryOrCollection = Blue.Cosacs.Warehouse.Utils.DeliveryOrCollectionHelpers.BookingActionToString(value);
        //    }
        //}

        [NotMapped]
        public string DeliveryOrCollectionDescription
        {
            get { return Blue.Cosacs.Warehouse.Utils.DeliveryOrCollection.FromString(DeliveryOrCollection).Name; }
        }
    }
}
