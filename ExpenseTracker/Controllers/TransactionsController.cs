using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using ExpenseTracker.ViewModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Identity;
using ToastNotification.Abstractions;

namespace ExpenseTracker.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly ILogger<AccountController> _logger;

        public TransactionsController(INotyfService notyfService, ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _notyfService = notyfService;
        }

        //View Model 
        public async Task<IActionResult> AssignTransactions(CategoryTransactionCombinedViewModel model)
        {
            var category = await _context.Categories.FindAsync(model.Transactions.CategoryId);

            if (category == null)
            {
                _logger.LogInformation("Category not found.");
                _notyfService.Error("Category not found.");
                return View("Index", new List<CategoryTransactionCombinedViewModel>());
            }

            model.Categories = new List<Category> { category };

            if (ModelState.IsValid)
            {
                // Create a new Transaction object
                var transaction = new Transaction
                {
                    CategoryId = model.Transactions.CategoryId,
                    Amount = model.Transactions.Amount,
                    Note = model.Transactions.Note,
                    Date = model.Transactions.Date
                };

                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Transaction with ID {transaction.TransactionId} added to database.");
                _notyfService.Success("Transaction added successfully.");

                return RedirectToAction("Index");
            }

            // If ModelState is not valid, return to Index view with empty list
            return View("Index", new List<CategoryTransactionCombinedViewModel>());
        }







        // GET: Transactions/ViewAll
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.Transactions.Include(t => t.Category).ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            // Create a list of CategoryTransactionCombinedViewModel
            var model = transactions.Select(transaction => new CategoryTransactionCombinedViewModel
            {
                Transactions = transaction,
                Categories = categories
            }).ToList();

            return View(model);
        }



        // Get: Transactions/Add
        [HttpGet]
        public  IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Transaction transactions)
        {
            var category = await _context.Categories.FindAsync(transactions.CategoryId);

            if (category is null)
            {
                _logger.LogInformation("Categoria nu a fost gasita.");
                return View();

            }

            var transaction = new Transaction()
            {
                Category = category,
                Amount = transactions.Amount,
                Note = transactions.Note,
                Date = transactions.Date,
            };

            if (ModelState.IsValid)
            {
                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"transactia cu numarul {transaction.TransactionId} s-a adaugat in baza de date");
                _notyfService.Success("Tranzactia a fost adaugata.");
                return RedirectToAction("Index");
            }

            return View("Index");
        }




        // GET: Transactions/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogInformation("Nu exista transactia in baza de date.");
                _notyfService.Error("Eroare.");

            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                _logger.LogInformation("Eroare gasire tranzactie");
                _notyfService.Information("Eroare");
            }
            PopulateCategory();

            return View(transaction);
        }

        // POST: Transactions/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (id != transaction.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Transactions.Any(t => t.TransactionId == transaction.TransactionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            PopulateCategory();
            return View();
        }


        // Transactions/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                _logger.LogInformation("Tranzactia a fost stearsa din baza de date.");
                _notyfService.Information("Tranzactia a fost stearsa.");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void PopulateCategory()
        {
            var categoryCollection = _context.Categories.ToList();
            Category defaultCategory = new Category() { CategoryId = 0, Title = "Choose Category" };
            // it's going to be inserted at the 0 index
            categoryCollection.Insert(0, defaultCategory);
            ViewBag.Categories = categoryCollection;
        }
    }
}
