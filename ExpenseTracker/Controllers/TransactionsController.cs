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
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.Transactions.ToListAsync();
            return View(transactions);
        }



        // CREATE - GET
        public async Task<IActionResult> Create()
        {
            List<ExpenseTracker.Models.CategoryModel> categories = await _context.Categories.ToListAsync();
            List<Transaction> transactions = new List<Transaction>();
            ViewModelCatTrans ctvm = new ViewModelCatTrans()
            {
                CategoriesList = categories,
                TransactionsList = transactions
            };

            return View(ctvm);
        }
        // CREATE - Post
        [HttpPost]
        public async Task<IActionResult> Create(ViewModelCatTrans model)
        {
            if (ModelState.IsValid)
            {
                var transaction = new TransactionModel()
                {
                    CategoryId = model.Transactions.CategoryId,
                    Amount = model.Transactions.Amount,
                    Note = model.Transactions.Note,
                    Date = model.Transactions.Date
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



        // EDIT - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<TransactionModel>(transaction);
            return View(model);
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

