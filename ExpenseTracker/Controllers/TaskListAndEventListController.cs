using AutoMapper;
using ExpenseTracker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ToastNotification.Abstractions;
using ExpenseTracker.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ExpenseTracker.Controllers
{
    public class TaskListAndEventListController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly ILogger<AccountController> _logger;

        public TaskListAndEventListController(ILogger<AccountController> logger, INotyfService notyfService, ApplicationDbContext context, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _notyfService = notyfService;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        //TaskList
        // INDEX - GET
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, string searchStringEvents)
        {
            var user = await _userManager.GetUserAsync(User);
            
            //Sorting
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["PrioritySortParm"] = sortOrder == "priority" ? "priority_desc" : "priority";
            ViewData["StatusSortParm"] = sortOrder == "status" ? "status_desc" : "status";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentFilterEvents"] = searchStringEvents;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var events = _context.EventList.Where(t => t.UserId == user.Id);
            if (!String.IsNullOrEmpty(searchStringEvents))
            {
                events = events.Where((s => s.EventName.Contains(searchStringEvents)));
            }



            var tasks = _context.TaskList.Where(t => t.UserId == user.Id);
            if (!String.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(s => s.TaskName.Contains(searchString) || s.TaskPriority.Contains(searchString));
            }

            if (string.IsNullOrWhiteSpace(sortOrder))
            {
                sortOrder = "sort1_desc";
            }

            ViewBag.Sort1Parm = sortOrder == "sort1_desc" ? "sort1_asc" : "sort1_desc";
            ViewBag.Sort2Parm = sortOrder == "sort2_asc" ? "sort2_desc" : "sort2_asc";
            ViewBag.Sort3Parm = sortOrder == "sort3_asc" ? "sort3_desc" : "sort3_asc";
            ViewBag.Sort4Parm = sortOrder == "sort4_asc" ? "sort4_desc" : "sort4_asc";
            ViewBag.CurrentSort = sortOrder;

            switch (sortOrder)
            {
                case "sort1_asc":
                    tasks = tasks.OrderBy(t => t.TaskName);
                    break;

                case "sort1_desc":
                    tasks = tasks.OrderByDescending(t => t.TaskName);
                    break;
                case "sort2_asc":
                    tasks = tasks.OrderBy(t => t.TaskPriority == "High" ? 1
                        : t.TaskPriority == "Medium" ? 2 : 3);
                    break;
                case "sort2_desc":
                    tasks = tasks.OrderByDescending(t => t.TaskPriority == "High" ? 1
                        : t.TaskPriority == "Medium" ? 2 : 3);
                    break;
                case "sort3_asc":
                    tasks = tasks.OrderBy(t => t.TaskDate);
                    break;
                case "sort3_desc":
                    tasks = tasks.OrderByDescending(t => t.TaskDate);
                    break;
                case "sort4_asc":
                    tasks = tasks.OrderBy(t => t.TaskStatus ? 2 : 1);
                    break;
                case "sort4_desc":
                    tasks = tasks.OrderByDescending(t => t.TaskStatus ? 2 : 1);
                    break;
                default:
                    tasks = tasks.OrderBy(t => t.TaskDate);
                    break;
            }
            var viewModelTasks = tasks.Select(task => new ViewModelCatTrans
            {
                Tasks = task
            });

            var viewModelEvents = events.Select(events => new ViewModelCatTrans
            {
                Events = events
            });

            ViewData["EventsViewModel"] = viewModelEvents.ToList();

            int pageSize = 5;
            return View(await ViewModelCatTrans.PaginatedList<ViewModelCatTrans>.CreateAsync(viewModelTasks.AsNoTracking(), pageNumber ?? 1, pageSize));

        }
        // CREATE - GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            List<TaskModel> tasks = new List<TaskModel>();

            ViewModelCatTrans ctvm = new ViewModelCatTrans()
            {
                TaskModelList = tasks,
                UserId = user.Id
            };

            return View(ctvm);
        }

        //CREATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ViewModelCatTrans model)
        {
            if (ModelState.IsValid)
            {
                var task = new TaskModel()
                {
                    UserId = model.UserId,
                    TaskName = model.Tasks.TaskName,
                    TaskDate = model.Tasks.TaskDate,
                    TaskPriority = model.Tasks.TaskPriority,
                    TaskStatus = model.Tasks.TaskStatus
                };

                await _context.TaskList.AddAsync(task);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Taskul cu id-ul {model.Tasks.ListId} a fost adaugata");
                _notyfService.Success("Taskul a fost adaugata cu success.");
                return RedirectToAction("Index");
            }
            _notyfService.Warning("Atentie! M nu a fost valid.");
            return View(model);
        }

        //EDIT - GET
        [HttpGet]
        public async Task<IActionResult> EditTask(int id)
        {
            var task = await _context.TaskList.FindAsync(id);
            if (task == null)
            {
                _logger.LogInformation("Taskul nu a fost gasit");
                _notyfService.Error("Taskul nu a fost gasit");
                return View("Index");
            }

            ViewModelCatTrans ctvm = new ViewModelCatTrans()
            {
                Tasks = new TaskModel()
                {
                        ListId = task.ListId,
                        TaskDate = task.TaskDate,
                        TaskName = task.TaskName,
                        TaskPriority = task.TaskPriority,
                }
            };
                
            return View(ctvm);
        }

        //EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTask(ViewModelCatTrans model, int id)
        {
            try
            {
                var task = await _context.TaskList.FindAsync(id);
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

                    if (task == null)
                    {
                        _logger.LogInformation("Task not found");
                        _notyfService.Error("Error Task");
                        return NotFound();
                    }

                    task.TaskName = model.Tasks.TaskName;
                    task.TaskDate = model.Tasks.TaskDate;
                    task.TaskPriority = model.Tasks.TaskPriority;
                    task.TaskStatus = model.Tasks.TaskStatus;

                    _context.Update(task);
                    await _context.SaveChangesAsync();

                    _notyfService.Success("Task updated successfully");
                    return RedirectToAction("Index");
                }

                _notyfService.Warning("A aparut o eroare.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(model.Tasks.ListId))
                {
                    return NotFound();
                }
                throw;
            }
            model.CategoriesList = await _context.Categories.ToListAsync();
            return View(model);

        }
        private bool TaskExists(int id)
        {
            return _context.TaskList.Any(e => e.ListId == id);
        }

        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _context.TaskList.FindAsync(id);
                if (task == null)
                {
                    _logger.LogError("Tranzactia nu a fost gasita");
                    return NotFound();
                }

                _logger.LogInformation($"Taskul cu id-ul {id} a fost sters");
                _notyfService.Success("Taskul a fost sters.");
                _context.TaskList.Remove(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                _logger.LogError($"Error deleting task with ID {id}");
                throw;
            }
        }



        //Event List

        //CREATE - GET
        public async Task<IActionResult> CreateEvent()
        {
            var user = await _userManager.GetUserAsync(User);
            List<EventModel> events = new List<EventModel>();

            ViewModelCatTrans ctvm = new ViewModelCatTrans()
            {
                EventModelList = events,
                UserId = user.Id
            };

            return View(ctvm);
        }

        //CREATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(ViewModelCatTrans model)
        {
            //ModelState.Remove("Events.UserId");
            if (ModelState.IsValid)
            {
                var events = new EventModel()
                {
                    UserId = model.UserId,
                    EventName = model.Events.EventName
                };

                await _context.EventList.AddAsync(events);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Taskul cu id-ul {model.Events.EventId} a fost adaugata");
                _notyfService.Success("Eventul a fost adaugat cu success.");
                return RedirectToAction("Index");
            }
            _notyfService.Warning("Atentie! M nu a fost valid.");
            return View(model);
        }

        //EDIT - GET
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var events = await _context.EventList.FindAsync(id);
            if (events == null)
            {
                _logger.LogInformation("Eventul nu a fost gasit");
                _notyfService.Error("Eventul nu a fost gasit");
                return View("Index");
            }

            ViewModelCatTrans ctvm = new ViewModelCatTrans()
            {
                Events = new EventModel()
                {
                    EventId = events.EventId,
                    EventName = events.EventName
                }
            };

            return View(ctvm);
        }

        //EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(ViewModelCatTrans model, int id)
        {
            try
            {
                var events = await _context.EventList.FindAsync(id);
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

                    if (events == null)
                    {
                        _logger.LogInformation("Event not found");
                        _notyfService.Error("Error Event");
                        return NotFound();
                    }

                    events.EventName = model.Events.EventName;
                    _context.Update(events);
                    await _context.SaveChangesAsync();

                    _notyfService.Success("Events updated successfully");
                    return RedirectToAction("Index");
                }

                _notyfService.Warning("A aparut o eroare.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExist(model.Events.EventId))
                {
                    return NotFound();
                }
                throw;
            }
            model.CategoriesList = await _context.Categories.ToListAsync();
            return View(model);

        }
        private bool EventExist(int id)
        {
            return _context.EventList.Any(e => e.EventId == id);
        }

        //DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var events = await _context.EventList.FindAsync(id);
                if (events == null)
                {
                    _logger.LogError("Eventul nu a fost gasit");
                    return NotFound();
                }

                _logger.LogInformation($"Eventul cu id-ul {id} a fost sters");
                _notyfService.Success("Eventul a fost sters.");
                _context.EventList.Remove(events);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                _logger.LogError($"Error deleting event with ID {id}");
                throw;
            }
        }
    }
}
