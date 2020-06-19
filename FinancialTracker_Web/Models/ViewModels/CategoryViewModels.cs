using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialTracker_Web.Models
{
    public class EditCategoryModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }

    public class EditCategoryItemModel : EditCategoryModel
    {
        public int ParentCategoryId { get; set; }
        public decimal AmountBudgeted { get; set; }
    }
}