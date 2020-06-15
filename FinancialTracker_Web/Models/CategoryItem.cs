using System.ComponentModel.DataAnnotations;

namespace FinancialTracker_Web.Models
{
    public class CategoryItem
    {
        public int Id { get; set; }

        [Required]
        public int ParentCategoryId { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(25)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public virtual Category ParentCategory { get; set; }
    }
}