namespace Blue.Cosacs.SalesManagement
{
    public partial class AdditionalCustomersInteraction
    {
        public enum InteractionType
        { 
            All = 0,
            InactiveCustomer = 1,
            InstalmentEnding = 2
        }

        public ContactMeanEnum ContactMeansIdEnum 
        {
            get
            {
                return (ContactMeanEnum)this.ContactMeansId;
            }
        }
    }
}
