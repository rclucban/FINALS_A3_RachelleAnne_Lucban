using BudgetTracker.Data;
using BudgetTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Services
{
    // Ito na ang magiging EF Core implementation mo
    public class BudgetService : IBudgetService
    {
        private readonly ApplicationDbContext _context;

        public BudgetService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🎯 C. I-implement ang Add Transaction
        public async Task AddTransactionAsync(Transaction transaction)
        {
            // Siguraduhing may Categories na naka-save sa DB bago ito gamitin!
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        // 🎯 C. I-implement ang Get Monthly Report
        public async Task<Dictionary<string, decimal>> GetMonthlyReportAsync(string userId, int year, int month)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date.Year == year && t.Date.Month == month)
                .ToListAsync();

            return transactions
                .GroupBy(t => t.Category.Name)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
        }

        // 🎯 C. I-implement ang Get Categories
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}