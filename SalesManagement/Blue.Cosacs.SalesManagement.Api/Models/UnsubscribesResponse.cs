namespace Blue.Cosacs.SalesManagement.Api.Models
{
    public class SmsUnsubscribesResponse
    {
        public string CustomerId { get; set; }
        public bool IsUnsubscribe { get; set; }
    }

    public class EmailUnsubscribesResponse
    {
        public string Email { get; set; }
        public bool IsUnsubscribe { get; set; }
    }
}