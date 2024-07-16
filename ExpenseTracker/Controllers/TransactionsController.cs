using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using ToastNotification.Abstractions;
using ExpenseTracker.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly ILogger<AccountController> _logger;
        private IMapper _mapper;

        public TransactionsController(INotyfService notyfService, ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _notyfService = notyfService;
        }


        // INDEX - GET
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.Transactions.Include(t => t.Category).ToListAsync();

            var model = transactions.Select(a => new TransactionModel
            {
                TransactionId = a.TransactionId,
                CategoryId = a.CategoryId,
                Amount = a.Amount,
                Note = a.Note,
                Date = a.Date,
                CategoryTitleWithIcon = a.CategoryTitleWithIcon,
                FormattedAmount = a.FormattedAmount,
                Category = a.Category
            }
            ).ToList();
            return View(model);
        }

        public async Task<IActionResult> AssignTransactions(TransactionModel model)
        {
            var category = await _context.Categories.FindAsync(model.CategoryId);

            if (category == null)
            {
                _logger.LogInformation("Category not found.");
                _notyfService.Error("Category not found.");
                return View("Index", new List<TransactionModel>());
            }

            ViewBag.Categories = PopulateCategory(model.CategoryId);

            if (ModelState.IsValid)
            {
                // Create a new Transaction object
                var transaction = new TransactionModel()
                {
                    CategoryId = model.CategoryId,
                    Amount = model.Amount,
                    Note = model.Note,
                    Date = model.Date
                };

                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Transaction with ID {transaction.TransactionId} added to database.");
                _notyfService.Success("Transaction added successfully.");

                return RedirectToAction("Index");
            }

            // If ModelState is not valid, return to Index view with empty list
            return View("Index", new List<TransactionModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {

            ViewBag.Categories = PopulateCategory(0);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionModel model)
        {
            var category = await _context.Categories.FindAsync(model.CategoryId);

            if (category is null)
            {
                _logger.LogInformation("Categoria nu a fost gasita.");
                return View();

            }

            var transaction = new TransactionModel()
            {
                CategoryId = model.CategoryId,
                Amount = model.Amount,
                Note = model.Note,
                Date = model.Date,
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

            var model = _mapper.Map<TransactionModel>(transaction);
            ViewBag.Categories = PopulateCategory(model.CategoryId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TransactionModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Transactions.Any(t => t.TransactionId == model.TransactionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            ViewBag.Categories = PopulateCategory(model.CategoryId);
            return View();
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _logger.LogInformation("Tranzactia a fost stearsa din baza de date.");
                _notyfService.Information("Tranzactia a fost stearsa.");

                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            _notyfService.Information("Can delete record");
            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<SelectListItem>> PopulateCategory(int categoryId)
        {
            var lstCategory = await _context.Categories.ToListAsync();

            Category defaultCategory = new Category() { CategoryId = 0, Title = "Choose Category" };
            //lstCategory.Insert(0, defaultCategory);
            return lstCategory.Select(d => new SelectListItem()
            {
                Text = d.TitleWithIcon,
                Value = d.CategoryId.ToString(),
                Selected = categoryId == d.CategoryId ? true : false
            }).ToList();
        }
    }
}
