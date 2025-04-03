using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
	public class DeliveryConfirm
	{
		public string resourceType { get; set; }
		public string source { get; set; }
		[Required]
		public string checkoutID { get; set; }
		public List<items> items { get; set; }
	}

	public class items
	{
		[Required]
		public string orderId { get; set; }
		public string type { get; set; }
		public string itemNo { get; set; }
		public int quantityOrdered { get; set; }
		public int quantityDelivered { get; set; }
		public string addressType { get; set; }
		public bool delivered { get; set; }
		public string comments { get; set; }
		public string deliveryDate { get; set; }
		public int employeeId { get; set; }

	}
}
