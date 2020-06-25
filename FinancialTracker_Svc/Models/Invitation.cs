namespace FinancialTracker_Svc.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentHouseholdId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public bool HasBeenProcessed { get; set; }
    }
}