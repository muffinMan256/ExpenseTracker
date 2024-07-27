using ExpenseTracker.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToastNotification.Abstractions;



namespace ExpenseTracker.Controllers
{


    [AllowAnonymous]
    public class GoogleController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly ILogger<AccountController> _logger;

        public GoogleController(INotyfService notyfService, ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _notyfService = notyfService;
        }


        public async Task Login()
        {
            _logger.LogInformation("Start Login");
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }


        //GOOGLE RESPONSE - Post
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            // Authenticate the user using Google
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });
            var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(emailClaim);
                    if (user == null)
                    {
                        // Optionally, register the user if they don't exist
                        user = new AppUser()
                        {
                            UserName = emailClaim,
                            Email = emailClaim

                        };
                        var createResult = await _userManager.CreateAsync(user);
                        if (!createResult.Succeeded)
                        {
                            _logger.LogError("User registration failed.");
                            _notyfService.Error("User registration failed");
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            var resultRole = await _userManager.AddToRoleAsync(user, "User");
                            _logger.LogInformation($"User {user.UserName} with role User created successfully.");
                            _notyfService.Information("User created successfully.");
                }   
                    }
                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Clear the external authentication cookie to prevent looping
                    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                    _logger.LogInformation($"User {user.Email} logged in with Google.");
                    return RedirectToAction("Index", "Dashboard");
        }
    }
}
