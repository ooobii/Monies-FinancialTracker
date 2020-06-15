using System.ComponentModel.DataAnnotations;

namespace FinancialTracker_Web.Models
{
    public class Invitation
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(25)]
        public string Name { get; set; }
        
        public int ParentHouseholdId { get; set; }

        public virtual Household ParentHousehold { get; set; }
    }
}