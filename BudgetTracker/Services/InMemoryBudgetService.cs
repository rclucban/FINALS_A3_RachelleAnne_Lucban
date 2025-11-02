using BudgetTracker.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetTracker.Services
{
    // Ito ay magsisilbing in-memory database
    public class InMemoryBudgetService : IBudgetService
    {
        // Gumamit ng ConcurrentDictionary para sa thread safety, 
        // at i-group ang transactions by UserID.
        private static readonly ConcurrentDictionary<string, List<Transaction>> _userTransactions = new();

        // In-memory Categories (Ito ay kulang ng 'Type' property, pero aayusin natin sa logic)
        private static readonly List<Category> _defaultCategories = new()
        {
            // Idagdag ang Type para magamit sa GetSummaryData
            new Category { CategoryId = 1, Name = "Groceries", Type = "Expense" },
            new Category { CategoryId = 2, Name = "Rent", Type = "Expense" },
            new Category { CategoryId = 3, Name = "Salary", Type = "Income" }, // Sample Income
            new Category { CategoryId = 4, Name = "Utilities", Type = "Expense" },
            new Category { CategoryId = 5, Name = "Entertainment", Type = "Expense" }
        };

        // ----------------------------------------------------------------------
        // 1. AddTransactionAsync (Tugma na ito)
        // ----------------------------------------------------------------------
        public Task AddTransactionAsync(Transaction transaction)
        {
            if (!_userTransactions.ContainsKey(transaction.UserId))
            {
                _userTransactions[transaction.UserId] = new List<Transaction>();
            }

            // Hanapin ang category at i-attach sa transaction (mahalaga para sa summary)
            var category = _defaultCategories.FirstOrDefault(c => c.CategoryId == transaction.CategoryId);
            if (category != null)
            {
                transaction.Category = category;
            }

            _userTransactions[transaction.UserId].Add(transaction);
            return Task.CompletedTask;
        }

        // ----------------------------------------------------------------------
        // 2. GetCategoriesAsync (Tugma na ito)
        // ----------------------------------------------------------------------
        public Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return Task.FromResult<IEnumerable<Category>>(_defaultCategories);
        }

        // ----------------------------------------------------------------------
        // 3. ✅ FIX: GetMonthlyTransactionsAsync (Kailangan ng IBudgetService)
        // ----------------------------------------------------------------------
        public Task<IEnumerable<Transaction>> GetMonthlyTransactionsAsync(int month)
        {
            // Para sa In-Memory, ibabalik lang natin ang transactions sa kasalukuyang user
            // (Assumed na may logic ka para kumuha ng UserId sa service, pero ibabalik muna natin ang sample)

            // Simpleng logic: ibalik ang transactions ng unang user na may data
            var firstUserId = _userTransactions.Keys.FirstOrDefault();

            if (firstUserId == null || !_userTransactions.TryGetValue(firstUserId, out var transactions))
            {
                return Task.FromResult<IEnumerable<Transaction>>(Enumerable.Empty<Transaction>());
            }

            // I-filter by month at tiyakin na may Category Object
            var monthlyTransactions = transactions
                .Where(t => t.Date.Month == month)
                .Select(t => {
                    t.Category = _defaultCategories.FirstOrDefault(c => c.CategoryId == t.CategoryId);
                    return t;
                })
                .Where(t => t.Category != null) // Siguraduhin na may category
                .ToList();

            return Task.FromResult<IEnumerable<Transaction>>(monthlyTransactions);
        }

        // ----------------------------------------------------------------------
        // 4. ✅ FIX: GetSummaryData (Kailangan ng IBudgetService)
        // ----------------------------------------------------------------------
        public Dictionary<string, decimal> GetSummaryData(IEnumerable<Transaction> transactions)
        {
            if (transactions == null || !transactions.Any())
            {
                return new Dictionary<string, decimal>();
            }

            // I-group base sa Category Type ("Income" o "Expense") at i-sum ang Amount
            return transactions
                .Where(t => t.Category != null && !string.IsNullOrEmpty(t.Category.Type))
                .GroupBy(t => t.Category.Type)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(t => t.Amount)
                );
        }
    }
}