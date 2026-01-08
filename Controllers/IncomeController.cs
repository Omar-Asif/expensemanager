using expensemanager.Constants;
using expensemanager.Data;
using expensemanager.Filters;
using expensemanager.Models;
using expensemanager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace expensemanager.Controllers
{
    [AuthorizeUser]
    public class IncomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public IncomeController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var items = await _db.Incomes.AsNoTracking()
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.Date)
                .Select(i => new IncomeListItemViewModel
                {
                    Id = i.Id,
                    Title = i.Title,
                    Amount = i.Amount,
                    Date = i.Date
                }).ToListAsync();

            var vm = new IncomeListViewModel
            {
                Items = items,
                TotalAmount = items.Sum(x => x.Amount)
            };
            return View(vm);
        }

        public IActionResult Create()
        {
            return View(new IncomeFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncomeFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var entity = new Income
            {
                Title = model.Title,
                Amount = model.Amount,
                Date = model.Date,
                Description = model.Description,
                UserId = userId
            };
            _db.Incomes.Add(entity);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var entity = await _db.Incomes.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
            if (entity is null) return NotFound();

            var vm = new IncomeFormViewModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Amount = entity.Amount,
                Date = entity.Date,
                Description = entity.Description
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IncomeFormViewModel model)
        {
            if (id != model.Id) return BadRequest();
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var entity = await _db.Incomes.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
            if (entity is null) return NotFound();
            if (!ModelState.IsValid) return View(model);

            entity.Title = model.Title;
            entity.Amount = model.Amount;
            entity.Date = model.Date;
            entity.Description = model.Description;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var entity = await _db.Incomes.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
            if (entity is null) return NotFound();

            var vm = new IncomeListItemViewModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Amount = entity.Amount,
                Date = entity.Date
            };
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var entity = await _db.Incomes.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
            if (entity is null) return NotFound();
            _db.Incomes.Remove(entity);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
