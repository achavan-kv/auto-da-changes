using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Models
{
    public class HierarchyTagImport
    {
        public string Level { get; set; }
        public int LevelId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
    }
}