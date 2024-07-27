using System.Globalization;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
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

        public DashboardController(ApplicationDbContext context, INotyfService notyfService, ILogger<AccountController> logger, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _notyfService = notyfService;
        }

        //INDEX - GET
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }
    }
}