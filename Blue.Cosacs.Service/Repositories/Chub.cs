using Blue.Cosacs.Messages.Service;
using Blue.Hub.Client;

namespace Blue.Cosacs.Service
{
    public class Chub
    {
        public Chub(IPublisher publisher)
        {
            this.publisher = publisher;
        }

        private readonly IPublisher publisher;

        public void ServiceSummary(ServiceSummary serviceSummary)
        {
            publisher.Publish<Context,ServiceSummary>("Cosacs.Service.Summary", serviceSummary);
        }

        public void ServicePayment(ServicePayment servicePayment)
        {
            publisher.Publish<Context,ServicePayment>("Cosacs.Service.Payment", servicePayment);
        }

        public void ServiceCharges(ServiceCharges serviceCharges)
        {
            publisher.Publish<Context,ServiceCharges>("Cosacs.Service.Charges", serviceCharges);
        }

        public void ServiceParts(ServiceParts serviceParts)
        {
            publisher.Publish<Context,ServiceParts>("Cosacs.Service.Parts", serviceParts);
        }

        public void WarrantyServiceCompleted(WarrantyServiceDetail serviceDetails)
        {
            publisher.Publish<Context,WarrantyServiceDetail>("Cosacs.Service.WarrantyServiceCompleted", serviceDetails);
        }

        public void ServiceDetail(ServiceDetail serviceDetail)
        {
            publisher.Publish<Context, ServiceDetail>("Cosacs.Service.Detail", serviceDetail);
        }
    }
}
