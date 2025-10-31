using BudgetTracker.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;

namespace BudgetTracker.Services
{
    // Ito ay magsisilbing in-memory database
    public class InMemoryBudgetService : IBudgetService
    {
        // Gumamit ng ConcurrentDictionary para sa thread safety, 
        // at i-group ang transactions by UserID.
        private static readonly ConcurrentDictionary<string, List<Transaction>> _userTransactions = new();
        private static readonly List<Category> _defaultCategories = new()
        {
            new Category { CategoryId = 1, Name = "Groceries" },
            new Category { CategoryId = 2, Name = "Rent" },
            new Category { CategoryId = 3, Name = "Utilities" },
            new Category { CategoryId = 4, Name = "Entertainment" }
        };

        public Task AddTransactionAsync(Transaction transaction)
        {
            if (!_userTransactions.ContainsKey(transaction.UserId))
            {
                _userTransactions[transaction.UserId] = new List<Transaction>();
            }
            _userTransactions[transaction.UserId].Add(transaction);
            return Task.CompletedTask;
        }

        public Task<Dictionary<string, decimal>> GetMonthlyReportAsync(string userId, int year, int month)
        {
            if (!_userTransactions.TryGetValue(userId, out var transactions))
            {
                return Task.FromResult(new Dictionary<string, decimal>());
            }

            var monthlyReport = transactions
                .Where(t => t.Date.Year == year && t.Date.Month == month)
                .Join(_defaultCategories, // I-join sa in-memory categories
                      t => t.CategoryId,
                      c => c.CategoryId,
                      (t, c) => new { t.Amount, CategoryName = c.Name })
                .GroupBy(t => t.CategoryName)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

            return Task.FromResult(monthlyReport);
        }

        public Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return Task.FromResult<IEnumerable<Category>>(_defaultCategories);
        }
    }
}