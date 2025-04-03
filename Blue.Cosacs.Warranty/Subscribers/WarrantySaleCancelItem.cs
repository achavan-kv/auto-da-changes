using Blue.Cosacs.Warranty.Repositories;

namespace Blue.Cosacs.Warranty.Subscribers
{
    public class WarrantySaleCancelItem : Hub.Client.Subscriber
    {
        private WarrantySaleRepository warrantySaleRepository;
        private WarrantyPotentialSaleRepository warrantyPotentialSaleRepository;

        public WarrantySaleCancelItem()
        {
            this.warrantySaleRepository = new WarrantySaleRepository();
            this.warrantyPotentialSaleRepository = new WarrantyPotentialSaleRepository();
        }

        public override void Sink(int id, System.Xml.XmlReader message)
        {
            var saleMessage = Deserialize<Blue.Cosacs.Messages.Warranty.SalesOrderCancelItem>(message);
            warrantySaleRepository.WarrantySaleCancelItem(saleMessage);
            warrantyPotentialSaleRepository.MarkItemAsReturned(saleMessage);

            if (saleMessage.TotalQuantity != null)
            {
                warrantySaleRepository.UpdateWarrantyPotentialQuantity(saleMessage);
            }
        }
    }
}
