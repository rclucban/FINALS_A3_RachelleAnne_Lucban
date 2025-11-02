// FILE: BudgetTracker/Models/ViewModels/ReportViewModel.cs

using BudgetTracker.Models; // Tiyakin na kasama ito para sa 'Transaction' Model
using System.Collections.Generic;

namespace BudgetTracker.Models.ViewModels
{
    public class ReportViewModel
    {
        // Summary data para sa Income/Expense totals
        public Dictionary<string, decimal> SummaryData { get; set; } = new Dictionary<string, decimal>();

        // ✅ ANG FIX: Idagdag ang Listahan ng Transactions
        public IEnumerable<Transaction> Transactions { get; set; } = Enumerable.Empty<Transaction>();
    }
}