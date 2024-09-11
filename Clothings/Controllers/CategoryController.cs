using Clothings.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothings.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var items = _context.categories.ToList();
            return View(items);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == 0)
            {
                Category category = new Category();
                return View(category);
            }
            else
            {
                var items = _context.categories.FirstOrDefault(u => u.Id == id);
                return View(items);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Upsert(int? id, Category category)
        {
            if (id == null)
            {
                var foundItem = await _context.categories.FirstOrDefaultAsync(u => u.Name == category.Name);
                if (foundItem != null)
                {
                    TempData["AlertMessage"] = category.Name + " is an existing item found in the list. so not added to the list";
                    return RedirectToAction("Index");
                }
                await _context.categories.AddAsync(category);
                TempData["AlertMessage"] = category.Name + " has added into the category list";
            }
            else
            {
                var items = await _context.categories.FirstOrDefaultAsync(u => u.Id == id);
                items.Name = category.Name;
                TempData["AlertMessage"] = category.Name + " has Edited into the category list";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var items = _context.categories.FirstOrDefault(u => u.Id == id);
            return View(items);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Category category)
        {
            var items = _context.categories.FirstOrDefault(u => u.Id == category.Id);
            _context.categories.Remove(items);
            TempData["AlertMessage"] = category.Name + "has deleted from the category list";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
