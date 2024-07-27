using AutoMapper;
using ExpenseTracker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ToastNotification.Abstractions;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;

namespace ExpenseTracker.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class CategoryController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;

        public CategoryController(INotyfService notyfService, ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _notyfService = notyfService;
            _mapper = mapper;
        }


        // INDEX - GET
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // ADD - GET
        public IActionResult Add()
        {
            return View();
        }
        // ADD - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoryModel model)
        {
            if (ModelState.IsValid)
            {   
                    //de ce nu folosim CategoryModel
                    var newCat = new CategoryModel()
                    {
                        Title = model.Title,
                        Icon = model.Icon,
                        Type = model.Type,
                        CreationDate = model.CreationDate,
                        Note = model.Note,
                        Priority = model.Priority,
                        Recurring = model.Recurring
                    };

                     
                await _context.Categories.AddAsync(newCat);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Categoria cu id-ul {model.CategoryId} numele {model.Title} a fost adaugata");
                _notyfService.Success("Categoria a fost adaugata cu success.");
                return RedirectToAction(nameof(Index));
            }

            _notyfService.Warning("Atentie! M nu a fost valid.");
            return View(model);

        }

        // EDIT - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<CategoryModel>(category);
            return View(model);
        }
        // EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingCategory = await _context.Categories.FindAsync(model.CategoryId);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }

                    existingCategory = _mapper.Map(model, existingCategory);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Categoria cu id-ul {model.CategoryId} a fost actualizata");
                    _notyfService.Success("Categoria a fost actualizata cu success.");
                    return RedirectToAction("Index");
                }
                _notyfService.Warning("A aparut o eroare.");
                return View(model);
                

            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"{ex.InnerException}");
                return RedirectToAction("Index");
            }
        }
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return NotFound();
                }

                _logger.LogInformation($"Categoria cu id-ul {id} a fost stearsa");
                _notyfService.Success("Categoria a fost stearsa.");
                _context.Categories.Remove(category);
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
