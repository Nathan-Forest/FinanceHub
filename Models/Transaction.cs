namespace FinanceHub.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        
        // Foreign key to Category
        public int CategoryId { get; set; }
        
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Type { get; set; } = "expense"; // "expense" or "income"
        
        // Navigation property - each transaction belongs to one category
        public Category? Category { get; set; }
    }
}