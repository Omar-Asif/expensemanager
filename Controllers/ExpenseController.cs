using expensemanager.Constants;
using expensemanager.Data;
using expensemanager.Filters;
using expensemanager.Models;
using expensemanager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace expensemanager.Controllers
{
    /// <summary>
    /// Manages expense CRUD operations enforcing per-user data isolation.
    /// </summary>
    [AuthorizeUser]
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _db;
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpenseController"/>.
        /// </summary>
        public ExpenseController(ApplicationDbContext db) => _db = db;

        /// <summary>
        /// Lists current user's expenses and total amount.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var items = await _db.Expenses
                .AsNoTracking()
                .Where(e => e.UserId == userId)
                .Include(e => e.Category)
                .OrderByDescending(e => e.Date)
                .Select(e => new ExpenseListItemViewModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    Amount = e.Amount,
                    Date = e.Date,
                    CategoryTitle = e.Category!.Title
                }).ToListAsync();

            var vm = new ExpenseListViewModel
            {
                Items = items,
                TotalAmount = items.Sum(i => i.Amount)
            };
            return View(vm);
        }

        /// <summary>
        /// Shows expense details for the current user.
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var entity = await _db.Expenses
                .AsNoTracking()
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (entity is null) return NotFound();

            var vm = new ExpenseDetailsViewModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Amount = entity.Amount,
                Date = entity.Date,
                Description = entity.Description,
                CategoryTitle = entity.Category?.Title ?? ""
            };
            return View(vm);
        }

        /// <summary>
        /// Returns the create form.
        /// </summary>
        public async Task<IActionResult> Create()
        {
            var vm = new ExpenseFormViewModel
            {
                Date = DateTime.UtcNow,
                Categories = await GetCategorySelectListAsync()
            };
            return View(vm);
        }

        /// <summary>
        /// Creates a new expense for the current user.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategorySelectListAsync();
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var entity = new Expense
            {
                Title = model.Title,
                Amount = model.Amount,
                Date = model.Date,
                Description = model.Description,
                CategoryId = model.CategoryId,
                UserId = userId
            };
            _db.Expenses.Add(entity);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Returns the edit form for an expense owned by the current user.
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var entity = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (entity is null) return NotFound();

            var vm = new ExpenseFormViewModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Amount = entity.Amount,
                Date = entity.Date,
                Description = entity.Description,
                CategoryId = entity.CategoryId,
                Categories = await GetCategorySelectListAsync()
            };
            return View(vm);
        }

        /// <summary>
        /// Updates an expense, enforcing ownership.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExpenseFormViewModel model)
        {
            if (id != model.Id) return BadRequest();

            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var entity = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (entity is null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategorySelectListAsync();
                return View(model);
            }

            entity.Title = model.Title;
            entity.Amount = model.Amount;
            entity.Date = model.Date;
            entity.Description = model.Description;
            entity.CategoryId = model.CategoryId;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Shows delete confirmation for an expense owned by the current user.
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var entity = await _db.Expenses
                .AsNoTracking()
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (entity is null) return NotFound();

            var vm = new ExpenseDetailsViewModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Amount = entity.Amount,
                Date = entity.Date,
                Description = entity.Description,
                CategoryTitle = entity.Category?.Title ?? string.Empty
            };
            return View(vm);
        }

        /// <summary>
        /// Deletes an expense, enforcing ownership.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var entity = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (entity is null) return NotFound();

            _db.Expenses.Remove(entity);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Builds the category dropdown list ordered by title.
        /// </summary>
        private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync()
        {
            var categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Title).ToListAsync();
            return categories.Select(c => new SelectListItem(c.Title, c.Id.ToString()));
        }
    }
}
