using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialTracker_Web.Models
{
    public class Household
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Greeting { get; set; }


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