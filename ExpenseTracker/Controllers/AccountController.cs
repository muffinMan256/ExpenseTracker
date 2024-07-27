using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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
        //private readonly MyEmailSender _myEmailSender;

        public AccountController(INotyfService notyfService, ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _notyfService = notyfService;
        }



        //REGISTER - GET
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            _logger.LogInformation("Start Register");
            return View();
        }
        //REGISTER - POST
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Email == "admin@admin.com")
                    {
                        var userAdmin = await _userManager.FindByEmailAsync(model.Email);
                        if (userAdmin == null)
                        {
                            userAdmin = new AppUser
                            {
                                Email = model.Email,
                                UserName = model.UserName
                            };
                            var createResult = await _userManager.CreateAsync(userAdmin, model.Password);
                            if (createResult.Succeeded)
                            {
                                var addRoleResult = await _userManager.AddToRoleAsync(userAdmin, "Admin");
                                if (!addRoleResult.Succeeded)
                                {
                                    _logger.LogError("Failed to add admin role to user.");
                                    _notyfService.Error("Error: A.R.");
                                }
                            }
                            else
                            {
                                _logger.LogError("Failed to create admin user.");
                                _notyfService.Error("Error: A.C.");
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Admin already exists in the Database.");
                            _notyfService.Error("Nu aveti access!");
                        }
                    }
                    else
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

                        if (string.IsNullOrEmpty(model.Password))
                        {
                            _logger.LogError("Password is null or empty.");
                            throw new InvalidOperationException("Password is null or empty.");
                        }

                        var user = new AppUser
                        {
                            Email = model.Email,
                            UserName = model.UserName
                        };

                        var result = await _userManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            var resultRole = await _userManager.AddToRoleAsync(user, "User");
                            if (resultRole.Succeeded)
                            {
                                _logger.LogInformation($"User {user.UserName} with role User created successfully.");
                                _notyfService.Information("User created successfully.");
                                return RedirectToAction("Login", "Account");
                            }
                            else
                            {
                                _logger.LogError($"Failed to add role to user {user.UserName}.");
                                // Optionally delete the user if adding to role fails
                                await _userManager.DeleteAsync(user);
                            }
                        }
                        else
                        {
                            _logger.LogError("Failed to create user.");
                            _notyfService.Error("Error: C.U.");
                        }

                        _notyfService.Error("Failed to create user.");
                        return RedirectToAction("Register", "Account");
                    }
                }
                else
                {
                    _logger.LogError("Modelul nu este valid");
                    _notyfService.Error("Eroare Inregistrare");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException?.ToString());
            }
            return View("Login");

        }

        //private async Task<bool> SendEmailConfirmationAsync(AppUser user, string email)
        //{
        //    try
        //    {
        //        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //        var returnUrl = Url.Content("~/");
        //        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //        var callbackUrl = Url.Page(
        //            "/Account/ConfirmEmail",
        //            pageHandler: null,
        //            values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
        //            protocol: Request.Scheme);

        //        _myEmailSender.SendEmail(email, "Confirm your Email",
        //            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error sending email confirmation.");
        //        return false;
        //    }
        //}



        //LOGIN - GET
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            _logger.LogInformation("Start Login");
            return View();
        }
        //LOGIN - POST
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.UserName))
                {
                    _logger.LogInformation("UserName is null or empty");
                    _notyfService.Error("UserName is required");
                    return RedirectToAction("Login", "Account");
                }
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"utilizatorul cu numele {user.Email} s-a logat pe pagina");
                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                _logger.LogInformation("Utilizatorul nu se poate loga");
                _notyfService.Error("Eroare U");
                return RedirectToAction("Login", "Account");
            }
            _logger.LogInformation("Modelul nu este valid");
            _notyfService.Error("Eroare M");
            return RedirectToAction("Login", "Account");
        }


        //UPDATE - GET
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Update()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogInformation("Eroare editare User");
                _notyfService.Error("A aparut o eroare: E");
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View(user);
            }
        }
        //UPDATE - POST
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
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
                user.UserName = model.UserName;
                user.Email = model.Email;
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
            return View();
        }


        //RESET PASSWORD - GET
        [HttpGet]
        public async Task<IActionResult> ResetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogInformation("Eroare editare User");
                _notyfService.Error("A aparut o eroare: E");
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View(user);
            }
        }
        //RESET PASSWORD - POST
        [HttpPost]
        public async Task<IActionResult> ResetPassword(AppUser model)
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
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"a fost actualizata parola userului {user.UserName} cu success");
                    _notyfService.Information("parola a fost actualizat cu success");
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
            return View();
        }


        //LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User has logged out");
            _notyfService.Information($"Utilizator a fost delogat cu success");
            return RedirectToAction("Login", "Account");
        }

    }

}
