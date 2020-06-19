using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

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

        public string SenderId { get; set; }
        public string RecipientId { get; set; }

        public virtual Household ParentHousehold { get; set; }

        public bool HasBeenProcessed { get; set; }


        private bool IsValidInvite(AppDbContext context) {
            //check if household exists
            var house = context.Households.Find(this.ParentHouseholdId);
            if( house == null ) { return false; }

            //check if sender still exists
            var sender = context.Users.Find(this.SenderId);
            if( sender == null ) { return false; }

            if( sender.HouseholdId != this.ParentHouseholdId ) { return false; }

            //check if recipient exists
            var recipient = context.Users.Find(this.RecipientId);
            if( recipient == null ) { return false; }


            //check if sender is still member of inviting household, or if recipient is already part of household
            if( sender.HouseholdId != this.ParentHouseholdId ) { return false; }
            if( recipient.HouseholdId == this.ParentHouseholdId ) { return false; }


            //all checks pass, return true.
            return true;
        }
        private bool CommitRecipientToHousehold(AppDbContext context, bool saveChanges = true) {
            try {
                context.Households.Find(this.ParentHouseholdId).Members.Add(context.Users.Find(RecipientId));
                if( saveChanges ) {
                    context.SaveChanges();
                    return context.Households.Find(this.ParentHouseholdId).Members.Select(u => u.Id).Contains(RecipientId);
                }
                return true;
            } catch {
                return false;
            }
        }




        public enum InviteResult
        {
            Success = 0,
            FailureNoInvite = 1,
            FailureBadCaller = 2,
            FailureInvalidInvite = 3
        }
        public static InviteResult ProcessInvite(AppDbContext context, int inviteId, IPrincipal caller, bool saveChanges = true) {
            var invite = context.Invitations.Find(inviteId);
            if( invite == null ) { return InviteResult.FailureNoInvite; }

            //make sure invite processor is the recipient
            if( caller.Identity.GetUserId() != invite.RecipientId ) { return InviteResult.FailureBadCaller; }

            //make sure that the invite is still valid.
            if( !invite.IsValidInvite(context) ) { return InviteResult.FailureInvalidInvite; }

            //attempt to process the invite; result of adding member to household 
            invite.HasBeenProcessed = invite.CommitRecipientToHousehold(context);
            if( saveChanges ) context.SaveChanges();
            return InviteResult.Success;
        }
    }
}