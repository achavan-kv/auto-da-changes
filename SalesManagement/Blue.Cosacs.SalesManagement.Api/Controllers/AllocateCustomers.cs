namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    public class AllocateCustomers
    {
        public int SalesPerson { get; set; }
        public string CustomerId { get; set; }
        public string MobileNumber { get; set; }
        public string LandLinePhone { get; set; }
        public string Email { get; set; }
        public short CustomerBranch { get; set; }
    }
}