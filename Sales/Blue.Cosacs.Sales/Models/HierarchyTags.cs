using System;

namespace Blue.Cosacs.Sales.Models
{
    [Serializable]
    public class HierarchyTags
    {
        public int LevelId { get; set; }
        public string TagName { get; set; }
        public int TagId { get; set; }
    }
}