// FILE: IBudgetService.cs

using BudgetTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IBudgetService
{
    // ✅ FIX 1: Para sa GET: /Transactions/Create
    Task<IEnumerable<Category>> GetCategoriesAsync();

    // ✅ FIX 2: Para sa POST: /Transactions/Create
    Task AddTransactionAsync(Transaction transaction);

    // ✅ FIX 3: Para sa GET: /Transactions/Report
    Task<IEnumerable<Transaction>> GetMonthlyTransactionsAsync(int month);

    // ✅ FIX 4: Para sa Summary Data
    Dictionary<string, decimal> GetSummaryData(IEnumerable<Transaction> transactions);
}