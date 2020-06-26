using System;
using System.Collections.Generic;

namespace FinancialTracker_Svc.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int ParentHouseholdId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CategoriesContainer
    {
        public ICollection<Category> Categories { get; set; }

        public CategoriesContainer(ICollection<Category> c) {
            Categories = c;
        }
    }
}