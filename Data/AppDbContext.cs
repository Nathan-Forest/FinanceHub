using Microsoft.EntityFrameworkCore;
using FinanceHub.Models;

namespace FinanceHub.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }  // NEW TABLE!

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Food & Dining", Icon = "🍕", Color = "#f59e0b" },
                new Category { Id = 2, Name = "Transportation", Icon = "🚗", Color = "#3b82f6" },
                new Category { Id = 3, Name = "Entertainment", Icon = "🎬", Color = "#a855f7" },
                new Category { Id = 4, Name = "Shopping", Icon = "🛍️", Color = "#ec4899" },
                new Category { Id = 5, Name = "Bills & Utilities", Icon = "💡", Color = "#eab308" },
                new Category { Id = 6, Name = "Health & Fitness", Icon = "🏥", Color = "#10b981" },
                new Category { Id = 7, Name = "Income", Icon = "💰", Color = "#22c55e" },
                new Category { Id = 8, Name = "Other", Icon = "📦", Color = "#6b7280" }
            );

            // Seed transactions with category references
            modelBuilder.Entity<Transaction>().HasData(
                new Transaction
                {
                    Id = 1,
                    Amount = 50.00m,
                    CategoryId = 1,  // Food & Dining
                    Description = "Lunch at cafe",
                    Date = DateTime.Now.AddDays(-2),
                    Type = "expense"
                },
                new Transaction
                {
                    Id = 2,
                    Amount = 2000.00m,
                    CategoryId = 7,  // Income
                    Description = "Monthly salary",
                    Date = DateTime.Now.AddDays(-5),
                    Type = "income"
                }
            );

            // Configure relationship
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId);
        }
    }
}