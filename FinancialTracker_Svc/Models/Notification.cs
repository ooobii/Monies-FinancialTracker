namespace FinancialTracker_Svc.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RecipientId { get; set; }
        public int ParentHouseholdId { get; set; }
    }
}