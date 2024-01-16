using BeFree.Areas.Admin.ViewModels;
using BeFree.Areas.Admin.ViewModels.Pagination;
using BeFree.DAL;
using BeFree.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFree.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Categories.CountAsync();
            List<Category> categories = await _context.Categories.Include(c => c.Products).Skip(page * 3).Take(3).ToListAsync();

            PaginationVM<Category> vm = new()
            {
                CurrentPage = page + 1,
                TotalPage = Math.Ceiling(count / 3),
                Items = categories
            };


            return View(vm);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM vm)
        {
            if (!ModelState.IsValid) return View();
            bool check = await _context.Categories.AnyAsync(c=>c.Name.ToLower().Trim()==vm.Name.ToLower().Trim());
            if (check)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View(vm);
            }

            Category category = new()
            {
                Name = vm.Name
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Category existed = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            CategoryUpdateVM vm = new()
            {
                Name = existed.Name,
            };
            return View(vm);

        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,CategoryUpdateVM vm)
        {
            
            if (id <= 0) return BadRequest();
            if (!ModelState.IsValid) return View(vm); 
            Category existed = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            existed.Name = vm.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Category existed = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            _context.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


    }
}
