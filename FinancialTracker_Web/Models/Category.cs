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
            decimal budget = 0;
            decimal spent = 0;

            foreach( var ci in CategoryItems ) {
                budget += ci.GetAmountBudgeted();
                spent += ci.GetTransactionTotals(month, year);
            }

            if (spent > 0) { spent = 0; }
            else { spent *= -1;  }

            if( budget > 0 ) {
                output = spent / budget;
                return output;
            } else {
                return 0;
            }
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