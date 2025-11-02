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
            // ✅ FIX 1: Ginawang mas malinaw ang dropdown (e.g., "Salary (Income)")
            CategoryList = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = $"{c.Name} ({c.Type})" // Idagdag ang Type para mas malinaw ang Savings option
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

        // ✅ FIX 2: Added a check for ModelState.IsValid and userId != null
        if (ModelState.IsValid && userId != null)
        {
            var transaction = new Transaction
            {
                Amount = model.Amount,
                CategoryId = model.CategoryId, // <--- Ito ang Category ID ng Savings
                Description = model.Description,
                Date = model.Date,
                UserId = userId
            };

            await _budgetService.AddTransactionAsync(transaction);

            // Redirect sa Report page para makita agad ang resulta
            return RedirectToAction(nameof(Report));
        }

        // If validation fails, reload categories and return to view to preserve dropdown
        // ✅ Tiyakin na iniload ulit ang CategoryList na may malinaw na Text
        model.CategoryList = (await _budgetService.GetCategoriesAsync()).Select(c => new SelectListItem
        {
            Value = c.CategoryId.ToString(),
            Text = $"{c.Name} ({c.Type})"
        }).ToList();

        return View(model);
    }


    // ----------------------------------------------------------------------
    // GET: /Transactions/Report (Generate Monthly Report)
    // ----------------------------------------------------------------------
    public async Task<IActionResult> Report()
    {
        var allTransactions = await _budgetService.GetMonthlyTransactionsAsync(DateTime.Now.Month);
        var summaryData = _budgetService.GetSummaryData(allTransactions);

        var viewModel = new ReportViewModel
        {
            SummaryData = summaryData,
            // I-assign ang na-fetch na transactions sa ViewModel
            Transactions = allTransactions
        };

        return View(viewModel);
    }
}