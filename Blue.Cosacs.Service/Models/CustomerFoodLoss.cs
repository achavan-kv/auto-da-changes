using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Service.Models
{
    public class CustomerFoodLoss
    {
        public class FoodLossItem
        {
            public string item { get; set; }
            public decimal value { get; set; }
        }

        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public Int32 CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public short Branch { get; set; }
        public bool Internal { get; set; }
        public string CustomerId { get; set; }
        public string CustomerTitle { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerAddressLine1 { get; set; }
        public string CustomerAddressLine2 { get; set; }
        public string CustomerAddressLine3 { get; set; }
        public string CustomerPostcode { get; set; }
        public int AllocationTechnician { get; set; }
        public DateTime AllocationServiceScheduledOn { get; set; }
        public CustomerFoodLoss.FoodLossItem[] FoodLoss { get; set; }
    }
}
