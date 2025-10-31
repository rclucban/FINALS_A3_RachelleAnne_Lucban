using BudgetTracker.Models;

namespace BudgetTracker.Services
{
    public interface IBudgetService
    {
        // CRUD operation
        Task AddTransactionAsync(Transaction transaction);

        // Report Generation
        Task<Dictionary<string, decimal>> GetMonthlyReportAsync(string userId, int year, int month);

        // Utility: Fetch all Categories for dropdowns
        Task<IEnumerable<Category>> GetCategoriesAsync();
    }
}