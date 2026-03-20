using Microsoft.AspNetCore.Mvc;
using FinanceHub.Models;

namespace FinanceHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        // In-memory storage (temporary - we'll use a database later!)
        private static List<Transaction> _transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                Amount = 50.00m,
                Category = "food",
                Description = "Lunch at cafe",
                Date = DateTime.Now.AddDays(-2),
                Type = "expense"
            },
            new Transaction
            {
                Id = 2,
                Amount = 1000.00m,
                Category = "income",
                Description = "Salary",
                Date = DateTime.Now.AddDays(-5),
                Type = "income"
            }
        };

        // GET: api/transactions
        [HttpGet]
        public ActionResult<IEnumerable<Transaction>> GetAllTransactions()
        {
            return Ok(_transactions);
        }

        // GET: api/transactions/1
        [HttpGet("{id}")]
        public ActionResult<Transaction> GetTransaction(int id)
        {
            var transaction = _transactions.FirstOrDefault(t => t.Id == id);
            
            if (transaction == null)
            {
                return NotFound(new { message = "Transaction not found" });
            }
            
            return Ok(transaction);
        }

        // POST: api/transactions
        [HttpPost]
        public ActionResult<Transaction> CreateTransaction(Transaction transaction)
        {
            // Auto-generate ID
            transaction.Id = _transactions.Any() 
                ? _transactions.Max(t => t.Id) + 1 
                : 1;
            
            transaction.Date = DateTime.Now;
            
            _transactions.Add(transaction);
            
            return CreatedAtAction(
                nameof(GetTransaction), 
                new { id = transaction.Id }, 
                transaction
            );
        }

        // DELETE: api/transactions/1
        [HttpDelete("{id}")]
        public ActionResult DeleteTransaction(int id)
        {
            var transaction = _transactions.FirstOrDefault(t => t.Id == id);
            
            if (transaction == null)
            {
                return NotFound(new { message = "Transaction not found" });
            }
            
            _transactions.Remove(transaction);
            
            return Ok(new { message = "Transaction deleted successfully" });
        }
    }
}