using Blue.Cosacs.Warranty.Repositories;

namespace Blue.Cosacs.Warranty.Subscribers
{
    public class WarrantyServiceCompletion : Hub.Client.Subscriber
    {
        private WarrantySaleRepository warrantySaleRepository;

        public WarrantyServiceCompletion()
        {
            this.warrantySaleRepository = new WarrantySaleRepository();
        }

        public override void Sink(int id, System.Xml.XmlReader message)
        {
            var serviceMessage = Deserialize<Blue.Cosacs.Messages.Service.WarrantyServiceDetail>(message);
            warrantySaleRepository.SetSerialNumber(serviceMessage.Warranty.ContractNumber, serviceMessage.Item.SerialNumber);
        }
    }
}
