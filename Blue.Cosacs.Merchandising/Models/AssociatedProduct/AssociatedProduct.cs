namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class AssociatedProductHierarchy
    {
        public string Division { get; set; }
        public string Department { get; set; }
        public string Class { get; set; }
    }

    public class AssociatedProduct
    {
        public AssociatedProductHierarchy HierarchyClass { get; set; }
        public IDictionary<string, string> Hierarchy { get; set; }

        public string SKU { get; set; }

        public string PosDescription { get; set; }

        public string LongDescription { get; set; }

        public int Id { get; set; }

        public bool StatusValid { get; set; }
    }
}