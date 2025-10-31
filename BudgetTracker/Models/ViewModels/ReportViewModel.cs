using System.Collections.Generic;

namespace BudgetTracker.Models.ViewModels
{
    public class ReportViewModel
    {
        // Ito ang magdadala ng Dictionary data sa View
        public Dictionary<string, decimal> SummaryData { get; set; } = new Dictionary<string, decimal>();
    }
}