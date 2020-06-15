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

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string RecipientId { get; set; }

        [Required]
        public int ParentHouseholdId { get; set; }


        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
        public virtual Household ParentHousehold { get; set; }
    }
}