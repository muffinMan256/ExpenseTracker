using System.Globalization;
using AutoMapper;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToastNotification.Abstractions;

namespace ExpenseTracker.Controllers
{
    public class DashboardController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;

        public DashboardController(ApplicationDbContext context, INotyfService notyfService, ILogger<AccountController> logger, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _logger = logger;
            _notyfService = notyfService;
        }

        //INDEX - GET
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new ViewModelCatTrans();

            if (User.IsInRole("User"))
            {
                List<CategoryModel> categories = await _context.Categories.ToListAsync();
                var transactions = await _context.Transactions
                    .Where(t => t.UserId == user.Id)
                    .ToListAsync();

                if (transactions == null || transactions.Count == 0)
                {
                    return View(viewModel); // Return an empty viewModel
                }

                var transactionsWithCategoryType = transactions
                    .Join(categories,
                        t => t.CategoryId,
                        c => c.CategoryId,
                        (t, c) => new { Transaction = t, CategoryType = c.Type })
                    .ToList();

                decimal totalExpenses = transactionsWithCategoryType
                    .Where(tc => tc.CategoryType == "Expense")
                    .Sum(tc => tc.Transaction.Amount);

                decimal totalIncome = transactionsWithCategoryType
                    .Where(tc => tc.CategoryType == "Income")
                    .Sum(tc => tc.Transaction.Amount);

                decimal savings = totalIncome * 10 / 100;
                decimal balance = totalIncome - totalExpenses;

                viewModel.CategoriesList = categories;
                viewModel.TransactionModelList = transactions;
                viewModel.UserId = user.Id;
                viewModel.TotalExpenses = totalExpenses;
                viewModel.TotalIncome = totalIncome;
                viewModel.Balance = balance;
                viewModel.Savings = savings;
            }
            else if (User.IsInRole("Admin"))
            {
                var users = await _userManager.Users.ToListAsync();
                viewModel.Users = users;
            }

            return View(viewModel);
        }


    }
}