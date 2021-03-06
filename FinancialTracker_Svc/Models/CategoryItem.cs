﻿using System.Collections.Generic;

namespace FinancialTracker_Svc.Models
{
    public class CategoryItem
    {
        public int Id { get; set; }
        public int ParentCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? AmountBudgeted { get; set; }
    }


    public class CategoryItemsContainer
    {
        public ICollection<CategoryItem> CategoryItems { get; set; }

        public CategoryItemsContainer(ICollection<CategoryItem> ci) {
            CategoryItems = ci;
        }
    }
}