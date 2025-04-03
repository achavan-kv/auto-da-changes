using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class ZonedTechniciansResult
    {
        public string ZoneCode { get; set; }
        public string ZoneName { get; set; }
        public List<DropDownItem> Technicians { get; set; }
    }
}
