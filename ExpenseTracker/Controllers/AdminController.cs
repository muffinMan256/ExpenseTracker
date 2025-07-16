using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ToastNotification.Abstractions;
using ToastNotification.Helpers;
using ToastNotification.Notyf;

namespace ExpenseTracker.Controllers
{
    [AllowAnonymous]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly ILogger<AccountController> _logger;

        public AdminController(INotyfService notyfService, ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _notyfService = notyfService;
        }

        //UPDATE - GET
        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogInformation("Eroare editare User");
                _notyfService.Error("A aparut o eroare: E");
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View();
            }
        }
        //UPDATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            return RedirectToAction("Index", "Dashboard");
        }

        // DELETE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                _logger.LogInformation($"Userul cu id-ul {id} a fost sters");
                _notyfService.Success("Userul a fost sters.");
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception)
            {
                _logger.LogError($"Error deleting category with ID {id}");
                throw;
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLockout([FromBody] AppUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            user.LockoutEnabled = model.LockoutEnabled;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Lockout status updated successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Error updating lockout status." });
            }
        }





    }
}
