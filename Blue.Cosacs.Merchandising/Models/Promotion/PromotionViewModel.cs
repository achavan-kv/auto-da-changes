using System.Collections.Generic;

namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class PromotionViewModel
    {
        public Dictionary<string, string> Hierarchy { get; set; }
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> HierarchyOptions { get; set; }

        public List<LocationListItemViewModel> Locations { get; set; }

        public Promotion Promotion { get; set; }
    }

    public class Promotion
    {
        public Promotion()
        {
            Details = new List<PromotionDetail>();
        }

        public int? Id { get; set; }

        [Required(ErrorMessage = "Promotion name is required")]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Fascia { get; set; }

        public int? LocationId { get; set; }

        [ReadOnly(true)]
        public string Location { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public int? Type { get; set; }

        [ReadOnly(true)]
        public string PromotionType { get; set; }

        public List<PromotionDetail> Details { get; set; }
    }

    public enum PromotionType
    {
        SetPrice = 1,
        ValueOff = 2,
        DiscountPercentage = 3
    }

    public class PromotionDetail
    {
        public PromotionDetail()
        {
            Hierarchies = new List<PromotionHierarchy>();
        }

        public int? Id { get; set; }

        public int? ProductId { get; set; }

        [ReadOnly(true)]
        public string Sku { get; set; }

        [ReadOnly(true)]
        public decimal OriginalPrice { get; set; }

        [ReadOnly(true)]
        public decimal AverageWeightedCost { get; set; }

        [ReadOnly(true)]
        public string Name { get; set; }

        [ReadOnly(true)]
        public string PriceTypeName { get; set; }

        [Required]
        public int PriceType { get; set; }

        /// <summary>
        /// This is the value of the product in this promotion
        /// It is not related to sets/combos
        /// </summary>
        [Range(0, int.MaxValue)]
        public decimal? SetPrice { get; set; }

        [Range(0, int.MaxValue)]
        public decimal? ValueDiscount { get; set; }

        [Range(0, 100)]
        public decimal? PercentDiscount { get; set; }

        public decimal? TaxRate { get; set; }

        public List<PromotionHierarchy> Hierarchies { get; set; }
    }

    public class PromotionHierarchy
    {
        public int? Id { get; set; }

        [Required]
        public string LevelName { get; set; }

        [Required]
        public string TagName { get; set; }

        public int LevelId { get; set; }

        public int TagId { get; set; }
    }

    public enum PriceType
    {
        Regular = 1,
        Cash = 2,
        DutyFree = 3
    }
}