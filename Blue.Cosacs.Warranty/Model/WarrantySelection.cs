using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantySelection
    {
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
