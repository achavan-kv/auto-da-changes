using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warehouse.Search
{
    public class Load
    {
        public string Id { get { return string.Format("Load:{0}", LoadId); } }
        public string Type { get { return "Load"; } }

        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public string[] DeliveryEmployees { get; set; }

        public int LoadId { get; set; }
        public string Truck { get; set; }
        public string Driver { get; set; }
        public int ItemsCount { get; set; }

        public string DeliveryBranchName { get; set; }

        public string ConfirmedOn { get; set; }
        public string ConfirmedBy { get; set; }
        public string DeliveryStatus { get; set; }
        public string[] DeliveryZones { get; set; }
    }
}
