using System.ComponentModel.DataAnnotations;

namespace FinancialTracker_Web.Models
{
    public class BankAccountType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
    }
}