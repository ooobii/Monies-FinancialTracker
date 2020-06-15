using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinancialTracker_Web.Models
{
    public class Category
    {
        public int Id { get; set; }

        public int ParentHouseholdId { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(25)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public decimal? AmountBudgeted { get; set; }

        public virtual Household ParentHousehold { get; set; }
        public virtual ICollection<CategoryItem> Items { get; set; }

        public Category() {
            Items = new HashSet<CategoryItem>();
        }
    }
}