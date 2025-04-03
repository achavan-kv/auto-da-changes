using System.ComponentModel.DataAnnotations.Schema;

namespace Blue.Cosacs.Warehouse
{
    public partial class PickListView : IDeliverable, INonStocksService
    {
        [NotMapped]
        public string OrderedOnString
        {
            get
            {
                return OrderedOn.ToString();        // #10163 jec
            }
        }

        [NotMapped]
        public string DeliveryOrCollectionDescription
        {
            get { return Utils.DeliveryOrCollection.FromString(DeliveryOrCollection).Name; }
        }
    }
}
