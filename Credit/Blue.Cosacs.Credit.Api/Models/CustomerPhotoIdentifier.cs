using System;

namespace Blue.Cosacs.Credit.Api.Models
{
    public class CustomerPhotoIdentifier
    {
        public int CustomerId { get; set; }
        public Guid PhotoIdentifier { get; set; }
    }
}
