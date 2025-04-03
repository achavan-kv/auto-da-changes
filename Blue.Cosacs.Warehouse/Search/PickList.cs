using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warehouse.Search
{
    public class PickList
    {
        public string Id { get { return string.Format("PickList:{0}", PickListNo); } }
        public string Type { get { return "PickList"; } }
        public int PickListNo { get; set; }

        public string CreatedOn { get; set; }
        public string ConfirmedOn { get; set; }
        public string PickedOn { get; set; }

        public string ConfirmedBy { get; set; }
        public string CreatedBy { get; set; }
        public string CheckedBy { get; set; }
        public string PickedBy { get; set; }
        public string PickingStatus { get; set; }

        public string[] PickingEmployees { get; set; }
        public string[] Trucks { get; set; }
        public int[] BookingIds { get; set; }
        public string StockBranchName { get; set; }
    }
}
