using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinancialTracker_Web.Models
{
    public class Household
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(25)]
        public string Name { get; set; }


        [Required]
        [MaxLength(255)]
        public string Greeting { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string CreatorId { get; set; }

        public ApplicationUser GetCreator(AppDbContext context = null) {
            if( context == null ) { context = new AppDbContext(); }

            return context.Users.Find(this.CreatorId);
        }

        public virtual ICollection<ApplicationUser> Members { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Invitation> Invitations { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

        public Household() {
            Members = new HashSet<ApplicationUser>();
            BankAccounts = new HashSet<BankAccount>();
            Categories = new HashSet<Category>();
            Invitations = new HashSet<Invitation>();
            Notifications = new HashSet<Notification>();
        }
    }
}