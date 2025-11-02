// FILE: BudgetService.cs

using BudgetTracker.Data;
using BudgetTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Tiyakin na kasama ito

public class BudgetService : IBudgetService
{
    private readonly ApplicationDbContext _context;

    public BudgetService(ApplicationDbContext context)
    {
        _context = context;
    }

    // ✅ IMPLEMENTATION 1: Get Categories
    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    // ✅ IMPLEMENTATION 2: Add Transaction
    public async Task AddTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    // ✅ IMPLEMENTATION 3: Get Monthly Transactions (Para sa Report)
    public async Task<IEnumerable<Transaction>> GetMonthlyTransactionsAsync(int month)
    {
        // Tinitiyak na kasama ang Category object (.Include(t => t.Category))
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.Date.Month == month)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    // ✅ IMPLEMENTATION 4: Get Summary Data
    public Dictionary<string, decimal> GetSummaryData(IEnumerable<Transaction> transactions)
    {
        return transactions
            .GroupBy(t => t.Category.Type)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t => t.Amount)
            );
    }
} 