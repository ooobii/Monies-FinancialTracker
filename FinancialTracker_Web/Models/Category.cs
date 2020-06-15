using System.Collections.Generic;

namespace FinancialTracker_Web.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int ParentHouseholdId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public decimal? AmountBudgeted { get; set; }

        public virtual Household ParentHousehold { get; set; }
        public virtual ICollection<CategoryItem> Items { get; set; }

        public Category() {
            Items = new HashSet<CategoryItem>();
        }
    }
}