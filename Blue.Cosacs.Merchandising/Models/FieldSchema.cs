namespace Blue.Cosacs.Merchandising.Models
{
    using System.ComponentModel.DataAnnotations;

    public class FieldSchema
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Value { get; set; }

        public string DisplayName { get; set; }
    }
}