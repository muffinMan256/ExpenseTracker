using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using ToastNotification.Abstractions;
using ExpenseTracker.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using ExpenseTracker.Models.ViewModel;
using TransactionModel = ExpenseTracker.Models.TransactionModel;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly ILogger<AccountController> _logger;
        private IMapper _mapper;

        public TransactionsController(INotyfService notyfService, ApplicationDbContext context,
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _notyfService = notyfService;
        }



        // INDEX - GET
        //public async Task<IActionResult> Index()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var transactions = await _context.Transactions.Where(t => t.UserId == user.Id).ToListAsync();
        //    return View(transactions);
        //}

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var transactions = await _context.Transactions
                .Where(t => t.UserId == user.Id)
                .Include(t => t.Category) 
                //.Include(t=>t.Category.Icon )
                .ToListAsync();

            return View(transactions);
        }


        // CREATE - GET
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            List<ExpenseTracker.Models.CategoryModel> categories = await _context.Categories.ToListAsync();
            //List<TransactionModel> transactions = new List<DbLoggerCategory.Database.Transaction>();
            ViewModelCatTrans ctvm = new ViewModelCatTrans()
            {
                CategoriesList = categories,
                //TransactionsList = transactions,
                UserId = user.Id
                
            };

            return View(ctvm);
        }
        // CREATE - POST
        [HttpPost]
        public async Task<IActionResult> Create(ViewModelCatTrans model)
        {
            ModelState.Remove("Transactions.Category");
            if (ModelState.IsValid)
            {
                var transaction = new TransactionModel()
                {
                    UserId = model.UserId,
                    CategoryId = model.Transactions.CategoryId,
                    Amount = model.Transactions.Amount,
                    Note = model.Transactions.Note,
                    Date = model.Transactions.Date,
                };

                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Tranzactioa cu id-ul {model.Transactions.TransactionId} a fost adaugata");
                _notyfService.Success("Tranzactia a fost adaugata cu success.");
                return RedirectToAction("Index");
            }

            _notyfService.Warning("Atentie! M nu a fost valid.");
            return View(model);
        }

        //EDIT - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                _logger.LogInformation("Transaction not found");
                _notyfService.Error("Error T");
                return NotFound();
            }

            List<CategoryModel> categories = await _context.Categories.ToListAsync();
            ViewModelCatTrans ctvm = new ViewModelCatTrans
            {
                CategoriesList = categories,
                Transactions = new Models.TransactionModel()
                {
                    TransactionId = transaction.TransactionId,
                    Amount = transaction.Amount,
                    Note = transaction.Note,
                    Date = transaction.Date,
                    CategoryId = transaction.CategoryId // Initialize the CategoryId
                },
                Categories = new CategoryModel { CategoryId = transaction.CategoryId } // Ensure this is initialized
            };

            return View(ctvm);
        }


        //EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ViewModelCatTrans model, int id)
        {
            try
            {
                var transaction = await _context.Transactions.FindAsync(id);
                ModelState.Remove("UserId");
                ModelState.Remove("Categories.Icon");
                ModelState.Remove("Categories.Type");
                ModelState.Remove("Categories.Title");
                ModelState.Remove("Categories.Priority");
                ModelState.Remove("Categories.Recurring");
                ModelState.Remove("Transactions.CategoryId");
                ModelState.Remove("Transactions.UserId");
                ModelState.Remove("Transactions.Category");
                if (ModelState.IsValid)
                {

                    if (transaction == null)
                    {
                        _logger.LogInformation("Transaction not found");
                        _notyfService.Error("Error T");
                        return NotFound();
                    }

                    transaction.Amount = model.Transactions.Amount;
                    transaction.Note = model.Transactions.Note;
                    transaction.Date = model.Transactions.Date;
                    transaction.CategoryId = model.Categories.CategoryId;

                    _context.Update(transaction);
                    await _context.SaveChangesAsync();

                    _notyfService.Success("Transaction updated successfully");
                    return RedirectToAction("Index");
                }

                _notyfService.Warning("A aparut o eroare.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(model.Transactions.TransactionId))
                {
                    return NotFound();
                }
                throw;
            }
            model.CategoriesList = await _context.Categories.ToListAsync();
            return View(model);
        }
        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }

        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var transaction = await _context.Transactions.FindAsync(id);
                if (transaction == null)
                {
                    _logger.LogError("Tranzactia nu a fost gasita");
                    return NotFound();
                }

                _logger.LogInformation($"Tranzactia cu id-ul {id} a fost stearsa");
                _notyfService.Success("Tranzactia a fost stearsa.");
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                _logger.LogError($"Error deleting category with ID {id}");
                throw;
            }
        }






    }
}

