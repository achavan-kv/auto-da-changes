using Blue.Cosacs.Warranty.Repositories;
using System;

namespace Blue.Cosacs.Warranty.Subscribers
{
    public class WarrantySaleCancel : Hub.Client.Subscriber
    {
        private WarrantySaleRepository warrantySaleRepository;

        public WarrantySaleCancel()
        {
            this.warrantySaleRepository = new WarrantySaleRepository();
        }

        public override void Sink(int id, System.Xml.XmlReader message)
        {
            var saleMessage = Deserialize<Blue.Cosacs.Messages.Warranty.SalesOrderCancel>(message);
            var saleId = warrantySaleRepository.UpdateStatus(saleMessage,saleMessage.Status);

            //Update the WarrantyPotentia Quantity
            if (saleMessage.ItemQuantity != null)
            {
                warrantySaleRepository.UpdateWarrantyPotentialQuantity(saleMessage);
            }

            //if (saleId.HasValue)
            //{
            //    Solr.SolrIndex.IndexWarrantySale(new int[] { saleId.Value });
            //}
        }
    }
}
