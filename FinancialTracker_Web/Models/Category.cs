using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebGrease.Css.Extensions;

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

        public DateTime CreatedAt { get; set; }



        public decimal GetTotalBudgetedAmount() {
            return CategoryItems.Sum(ci => ci.AmountBudgeted ?? 0);
        }

        public decimal GetThisMonthTotalBudgetUsage() {
            return GetTotalBudgetUsage(DateTime.Now.Month, DateTime.Now.Year);
        }
        public decimal GetThisMonthTotalBudgetUsageAmount() {
            return GetTotalBudgetUsageAmount(DateTime.Now.Month, DateTime.Now.Year);
        }
        public decimal GetTotalBudgetUsage(int month, int year) {
            decimal output = 0;
            foreach( var ci in CategoryItems ) {
                output += ci.GetBudgetUsage(month, year);
            }
            return output;
        }
        public decimal GetTotalBudgetUsageAmount(int month, int year) {
            decimal output = 0;
            foreach( var ci in CategoryItems ) {
                output += ci.GetTransactionTotals(month, year) * -1;
            }
            return output;
        }




        public virtual Household ParentHousehold { get; set; }
        public virtual ICollection<CategoryItem> CategoryItems { get; set; }

    }
}