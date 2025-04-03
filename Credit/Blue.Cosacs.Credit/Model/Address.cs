using Blue.Cosacs.Credit.Extensions;
namespace Blue.Cosacs.Credit.Model
{
    public class Address
    {
        public Address()
        { 
        }

        internal Address(Credit.ProposalAddress a)
        {
            this.AddressType = a.AddressType;
            this.Line1 = a.AddressLine1;
            this.Line2 = a.AddressLine2;
            this.City = a.City;
            this.PostCode = a.PostCode;
            this.DeliveryArea = a.DeliveryArea;
            this.ProposalId = a.ProposalId;
        }

        public Credit.ProposalAddress ToTable(int prosposalId)
         {
             return new Credit.ProposalAddress()
             {
                 AddressType = this.AddressType.SafeTrim(),
                 AddressLine1 = this.Line1.SafeTrim(),
                 AddressLine2 = this.Line2.SafeTrim(),
                 City = this.City.SafeTrim(),
                 PostCode = this.PostCode.SafeTrim(),
                 DeliveryArea = this.DeliveryArea.SafeTrim(),
                 ProposalId = prosposalId
             };
         }

        public string AddressType { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string DeliveryArea { get; set; }
        public int ProposalId { get; set; }
    }
}
