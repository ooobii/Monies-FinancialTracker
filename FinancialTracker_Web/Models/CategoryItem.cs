namespace FinancialTracker_Web.Models
{
    public class CategoryItem
    {
        public int Id { get; set; }
        public int ParentCategoryId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public virtual Category ParentCategory { get; set; }
    }
}