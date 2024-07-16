using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToastNotification.Abstractions;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(INotyfService notyfService, ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _notyfService = notyfService;
        }



        //REGISTER - GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            _logger.LogInformation("Start Register");
            return View("LoginRegister");
        }

        //REGISTER - POST
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(LoginRegisterCombined model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_userManager == null)
                    {
                        _logger.LogError("UserManager is not initialized.");
                        throw new InvalidOperationException("UserManager is not initialized.");
                    }

                    if (model == null)
                    {
                        _logger.LogError("Model is null.");
                        throw new InvalidOperationException("Model is null.");
                    }

                    if (string.IsNullOrEmpty(model.RegisterModel.Password))
                    {
                        _logger.LogError("Password is null or empty.");
                        throw new InvalidOperationException("Password is null or empty.");
                    }

                    var user = new AppUser
                    {
                        Email = model.RegisterModel.Email,
                        UserName = model.RegisterModel.Email
                    };

                    var result = await _userManager.CreateAsync(user, model.RegisterModel.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User {user.UserName} created successfully.");
                        _notyfService.Information("User created successfully.");
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        _logger.LogError("Failed to create user.");
                        return RedirectToAction("Register", "Account");
                    }
                }

                ModelState.AddModelError("", "Password was not secure enough");
                _notyfService.Error("Eroare Inregistrare");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException?.ToString());
            }
            return View("LoginRegister");

        }


        //LOGIN - GET
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View("LoginRegister");
        }
        //LOGIN - POST
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRegisterCombined model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.LoginModel.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.LoginModel.Password, false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"utilizatorul cu numele{user.Email} s-a logat pe pagina");
                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                ModelState.AddModelError("", "Invalid login attempt.");
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Login", "Account");
        }



        //LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User has logged out");
            _notyfService.Information("Utilizator a fost delogat cu success");
            return RedirectToAction("Login", "Account");
        }

        //UPDATE - GET
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View(user);
            }
        }
        //UPDATE - POST
        [HttpPost]
        public async Task<IActionResult> Update(AppUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogError("Nu exista utilizatorul");
                    _notyfService.Error("Utilizatorul nu a fost găsit.");
                    return RedirectToAction("Index", "Dashboard");
                }
                user.Email = model.Email;
                user.UserName = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Birthday = model.Birthday;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"a fost actualizat utilizatorul {user.UserName} cu success");
                    _notyfService.Information("Utilizator actualizat cu success");
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    _logger.LogInformation("Eroare actualizare");
                    _notyfService.Error("A aparut o eroare la actualizare");
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            _logger.LogInformation("Check Db to see if updated");
            return View("Edit");
        }

        //Reset Password
        //[HttpGet]
        //public async Task<IActionResult> Reset(RegisterModel model)
        //{
        //    var user = await _userManager.ChangePasswordAsync(model);
        //    return View();
        //}
    }
}
