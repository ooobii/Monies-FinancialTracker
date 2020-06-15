using System.ComponentModel.DataAnnotations;

namespace FinancialTracker_Web.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        [Required]
        public string RecipientId { get; set; }

        [Required]
        public int ParentHouseholdId { get; set; }


        public virtual ApplicationUser Recipient { get; set; }
        public virtual Household ParentHousehold { get; set; }
    }
}