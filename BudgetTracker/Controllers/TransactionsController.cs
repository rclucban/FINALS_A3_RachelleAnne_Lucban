using BudgetTracker.Models;
using BudgetTracker.Models.ViewModels; // Ginagamit ang TransactionViewModel at ReportViewModel
using BudgetTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System;
using System.Linq;

[Authorize]
public class TransactionsController : Controller
{
    private readonly IBudgetService _budgetService;
    private readonly UserManager<IdentityUser> _userManager;

    public TransactionsController(IBudgetService budgetService, UserManager<IdentityUser> userManager)
    {
        _budgetService = budgetService;
        _userManager = userManager;
    }

    // ----------------------------------------------------------------------
    // GET: /Transactions/Create (Display Add Expense Form)
    // ----------------------------------------------------------------------
    public async Task<IActionResult> Create()
    {
        var categories = await _budgetService.GetCategoriesAsync();

        var viewModel = new TransactionViewModel
        {
            CategoryList = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToList(),
            Date = DateTime.Now
        };

        return View(viewModel);
    }

    // ----------------------------------------------------------------------
    // POST: /Transactions/Create (Process Expense Submission)
    // ----------------------------------------------------------------------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TransactionViewModel model)
    {
        var userId = _userManager.GetUserId(User);

        if (ModelState.IsValid && userId != null)
        {
            var transaction = new Transaction
            {
                Amount = model.Amount,
                CategoryId = model.CategoryId,
                Description = model.Description,
                Date = model.Date,
                UserId = userId
            };

            await _budgetService.AddTransactionAsync(transaction);

            return RedirectToAction(nameof(Report));
        }

        // If validation fails, reload categories and return to view to preserve dropdown
        model.CategoryList = (await _budgetService.GetCategoriesAsync()).Select(c => new SelectListItem
        {
            Value = c.CategoryId.ToString(),
            Text = c.Name
        }).ToList();

        return View(model);
    }


    // ----------------------------------------------------------------------
    // GET: /Transactions/Report (Generate Monthly Report)
    // ----------------------------------------------------------------------
    public async Task<IActionResult> Report()
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        // Get report for the current month
        var reportData = await _budgetService.GetMonthlyReportAsync(userId, DateTime.Now.Year, DateTime.Now.Month);

        // GINAWA ANG TAMANG VIEWMODEL
        var viewModel = new ReportViewModel
        {
            SummaryData = reportData
        };

        // **NAAYOS ITO:** Ipinasa na ang TAMANG OBJECT: viewModel (ReportViewModel)
        return View(viewModel);
    }
}