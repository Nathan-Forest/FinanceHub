namespace FinanceHub.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = "📦"; // Emoji for visual display!
        public string Color { get; set; } = "#6b7280"; // Hex color code
        
        // Navigation property - one category has many transactions
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}