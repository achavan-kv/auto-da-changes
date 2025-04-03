using System.ComponentModel.DataAnnotations.Schema;
using Blue.Cosacs.Warehouse.Utils;

namespace Blue.Cosacs.Warehouse
{
    using System;
    using System.Runtime.Serialization;

    public partial class BookingView : IDeliverable, ICloneable, INonStocksService
    {
        [NotMapped, System.Runtime.Serialization.DataMemberAttribute()]
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

        [NotMapped, System.Runtime.Serialization.DataMemberAttribute()]
        public string DeliveryOrCollectionDescription
        {
            get { return Utils.DeliveryOrCollection.FromString(DeliveryOrCollection).Name; }
        }

        [NotMapped, System.Runtime.Serialization.DataMemberAttribute()]
        public string OrderedOnString
        {
            get
            {
                return OrderedOn.ToString();
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

        [NotMapped, System.Runtime.Serialization.DataMemberAttribute()]
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


    //BOC CR 2018-13
    public partial class DeliveryView : IDeliverable, ICloneable, INonStocksService
    {
        [NotMapped, System.Runtime.Serialization.DataMemberAttribute()]
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

        [NotMapped, System.Runtime.Serialization.DataMemberAttribute()]
        public string DeliveryOrCollectionDescription
        {
            get { return Utils.DeliveryOrCollection.FromString(DeliveryOrCollection).Name; }
        }

        [NotMapped, System.Runtime.Serialization.DataMemberAttribute()]
        public string OrderedOnString
        {
            get
            {
                return OrderedOn.ToString();
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

        [NotMapped, System.Runtime.Serialization.DataMemberAttribute()]
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
    //EOC CR 2018-13
}


