using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;

namespace FinancialTracker_Web.Models
{
    public class CategoryItem
    {
        public int Id { get; set; }

        public int ParentCategoryId { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(25)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public decimal? AmountBudgeted { get; set; }



        public decimal GetThisMonthBudgetUsage() {
            return GetBudgetUsage(DateTime.Now.Month, DateTime.Now.Year);
        }
        public decimal GetThisMonthBudgetUsageAmount() {
            return GetTransactionTotals(DateTime.Now.Month, DateTime.Now.Year) * -1;
        }
        public decimal GetBudgetUsage(int month, int year) {
            if( AmountBudgeted != null ) {
                if( AmountBudgeted > 0 ) {
                    return GetTransactionTotals(month, year) / AmountBudgeted.Value * -1;
                }
            }
            return 0;
        }

        public decimal GetTransactionTotals(int month, int year) {
            return Transactions.Where(t => t.OccuredAt.Month == month && t.OccuredAt.Year == year).Sum(t => t.GetAmount());
        }



        public virtual Category ParentCategory { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}