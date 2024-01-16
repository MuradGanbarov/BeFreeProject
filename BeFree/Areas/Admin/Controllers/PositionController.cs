using BeFree.Areas.Admin.ViewModels.Pagination;
using BeFree.Areas.Admin.ViewModels;
using BeFree.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeFree.DAL;

namespace BeFree.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PositionController : Controller
    {
        private readonly AppDbContext _context;

        public PositionController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Categories.CountAsync();
            List<Position> positions = await _context.Positions.Include(c => c.Employees).Skip(page * 3).Take(3).ToListAsync();

            PaginationVM<Position> vm = new()
            {
                CurrentPage = page + 1,
                TotalPage = Math.Ceiling(count / 3),
                Items = positions
            };


            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PositionCreateVM vm)
        {
            if (!ModelState.IsValid) return View();
            bool check = await _context.Positions.AnyAsync(c => c.Name.ToLower().Trim() == vm.Name.ToLower().Trim());
            if (check)
            {
                ModelState.AddModelError("Name", "This position already exists");
                return View(vm);
            }

            Position position = new()
            {
                Name = vm.Name
            };

            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Position existed = await _context.Positions.Include(c => c.Employees).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            PositionUpdateVM vm = new()
            {
                Name = existed.Name,
            };
            return View(vm);

        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, PositionUpdateVM vm)
        {

            if (id <= 0) return BadRequest();
            if (!ModelState.IsValid) return View(vm);
            Position existed = await _context.Positions.Include(c => c.Employees).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            existed.Name = vm.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Position existed = await _context.Positions.Include(c => c.Employees).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            _context.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
