using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceHub.Data;
using FinanceHub.Models;
using System.Linq;

namespace FinanceHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Constructor - DbContext injected automatically!
        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetAllTransactions()
        {
            var transactions = await _context.Transactions.ToListAsync();
            return Ok(transactions);
        }

        // GET: api/transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound(new { message = "Transaction not found" });
            }

            return Ok(transaction);
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTransaction),
                new { id = transaction.Id },
                transaction
            );
        }

        // PUT: api/transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound(new { message = "Transaction not found" });
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Transaction deleted successfully" });
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }

        // GET: api/transactions/summary
        [HttpGet("summary")]
        public async Task<ActionResult> GetSummary()
        {
            try
            {
                // Load all transactions into memory first
                var transactions = await _context.Transactions.ToListAsync();

                // Now sum in C# (not SQL!)
                
                var totalIncome = transactions
                    .Where(t => t.Type == "income")
                    .Sum(t => t.Amount);

                var totalExpenses = transactions
                    .Where(t => t.Type == "expense")
                    .Sum(t => t.Amount);

                var transactionCount = transactions.Count;

                var averageExpense = transactions
                    .Where(t => t.Type == "expense")
                    .DefaultIfEmpty()
                    .Average(t => t?.Amount ?? 0);

                var summary = new
                {
                    totalIncome,
                    totalExpenses,
                    netBalance = totalIncome - totalExpenses,
                    transactionCount,
                    averageExpense
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/transactions/by-category
        [HttpGet("by-category")]
        public async Task<ActionResult> GetByCategory()
        {
            try
            {
                // Get all categories first
                var categories = await _context.Categories.ToListAsync();

                // Get all expense transactions
                var expenseTransactions = await _context.Transactions
                    .Where(t => t.Type == "expense")
                    .ToListAsync();

                // Group in memory to avoid EF complications
                var groupedData = expenseTransactions
                    .GroupBy(t => t.CategoryId)
                    .Select(g =>
                    {
                        var category = categories.FirstOrDefault(c => c.Id == g.Key);
                        return new
                        {
                            categoryId = g.Key,
                            category = category?.Name ?? "Unknown",
                            icon = category?.Icon ?? "📦",
                            color = category?.Color ?? "#6b7280",
                            totalAmount = g.Sum(t => t.Amount),
                            transactionCount = g.Count()
                        };
                    })
                    .ToList();

                // Calculate percentages
                var total = groupedData.Sum(x => x.totalAmount);
                var result = groupedData.Select(x => new
                {
                    x.categoryId,
                    x.category,
                    x.icon,
                    x.color,
                    x.totalAmount,
                    x.transactionCount,
                    percentage = total > 0 ? (double)(x.totalAmount / total * 100) : 0
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/transactions/monthly
        [HttpGet("monthly")]
        public async Task<ActionResult> GetMonthlyBreakdown()
        {
            try
            {
                var transactions = await _context.Transactions.ToListAsync();

                var monthlyData = transactions
                    .GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .Select(g => new
                    {
                        year = g.Key.Year,
                        month = g.Key.Month,
                        income = g.Where(t => t.Type == "income").Sum(t => t.Amount),
                        expenses = g.Where(t => t.Type == "expense").Sum(t => t.Amount),
                        net = g.Where(t => t.Type == "income").Sum(t => t.Amount) -
                              g.Where(t => t.Type == "expense").Sum(t => t.Amount),
                        transactionCount = g.Count()
                    })
                    .OrderByDescending(x => x.year)
                    .ThenByDescending(x => x.month)
                    .ToList();

                return Ok(monthlyData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/transactions/recent
        [HttpGet("recent")]
        public async Task<ActionResult> GetRecentTransactions([FromQuery] int count = 10)
        {
            var recent = await _context.Transactions
                .Include(t => t.Category)
                .OrderByDescending(t => t.Date)
                .Take(count)
                .ToListAsync();

            return Ok(recent);
        }

        // GET: api/transactions/date-range
        [HttpGet("date-range")]
        public async Task<ActionResult> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return Ok(transactions);
        }
    }
}