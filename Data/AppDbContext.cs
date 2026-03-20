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

        // This represents the Transactions table
        public DbSet<Transaction> Transactions { get; set; }

        // Optional: Configure model relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed some initial data
            modelBuilder.Entity<Transaction>().HasData(
                new Transaction
                {
                    Id = 1,
                    Amount = 50.00m,
                    Category = "food",
                    Description = "Initial test transaction",
                    Date = DateTime.Now.AddDays(-5),
                    Type = "expense"
                },
                new Transaction
                {
                    Id = 2,
                    Amount = 1000.00m,
                    Category = "income",
                    Description = "Test salary",
                    Date = DateTime.Now.AddDays(-10),
                    Type = "income"
                }
            );
        }
    }
}