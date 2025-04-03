using System.Collections.Generic;

namespace Blue.Cosacs.NonStocks.Models.WinCosacs
{
    public class HierarchyInfo
    {
        public string Name { get; set; }
        public List<Node> Data { get; set; }

        public class Node
        {
            public string Key { get; set; }
            public string Name { get; set; }
        }
    }
}
