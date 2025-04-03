using System.ComponentModel.DataAnnotations.Schema;
using Blue.Cosacs.Warehouse.Utils;

namespace Blue.Cosacs.Warehouse
{
    public partial class Booking : System.ICloneable, INonStocksService
    {
        [NotMapped]
        public int BookingId
        {
            get
            {
                return Id;
            }
            set
            {
                Id = value;
            }
        }

        [NotMapped]
        public string DeliveryOrCollectionDescription
        {
            get { return Utils.DeliveryOrCollection.FromString(DeliveryOrCollection).Name; }
        }

        [NotMapped]
        public string OrderedOnString
        {
            get
            {
                return OrderedOn.ToString("dddd, dd MMMM yyyy, h:mm tt");
            }
        }

        public bool IsAutoPicked
        {
            get
            {
                return StockBranch != DeliveryBranch
                    || Utils.DeliveryOrCollection.Collection.Code == DeliveryOrCollection;
            }
        }

        public bool AutoPick()
        {
            if (IsAutoPicked)
            {
                PickingRejected = false;
                PickQuantity = Quantity;
                PickingComment = AutoUser.AutoPickConfirm.Comment;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is rejected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is rejected; otherwise, <c>false</c>.
        /// </value>
        [NotMapped]
        public bool IsRejected
        {
            get
            {
                return (PickingRejected.HasValue && PickingRejected.Value) ||
                        (ScheduleRejected.HasValue && ScheduleRejected.Value) ||
                        (DeliveryRejected.HasValue && DeliveryRejected.Value);
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
