using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using ToastNotification.Abstractions;
using ExpenseTracker.Data;

namespace ExpenseTracker.Controllers
{
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


        // GET: Category
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }


        // GET: Category/Add
        public IActionResult Add()
        {
            return View();
        }


        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoryModel model)
        {
            if (ModelState.IsValid)
            {

                    var newCat = new Category()
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

            _notyfService.Warning("Atentie! Modelul nu a fost valid.");
            return View(model);

        }


        // GET: Category/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            //var model = new CategoryModel()
            //{
            //    CategoryId = category.CategoryId,
            //    Title = category.Title,
            //    Icon = category.Icon,
            //    Type = category.Type,
            //    CreationDate = category.CreationDate,
            //    Note = category.Note,
            //    Priority = category.Priority,
            //    Recurring = category.Recurring

            //};

            var model = _mapper.Map<CategoryModel>(category);
            return View(model);
        }

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

                    // Update the existing category with the values from the passed-in category
                    //existingCategory.Title = model.Title;
                    //existingCategory.Icon = model.Icon;
                    //existingCategory.Type = model.Type;
                    //existingCategory.CreationDate = model.CreationDate;
                    //existingCategory.Note = model.Note;
                    //existingCategory.Priority = model.Priority;
                    //existingCategory.Recurring = model.Recurring;

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


        // POST: Category/Delete/5
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
