using BeFree.Areas.Admin.ViewModels.Pagination;
using BeFree.Areas.Admin.ViewModels.Setting;
using BeFree.DAL;
using BeFree.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFree.Areas.Admin.Controllers
{
    [Area("Admin")]

    [Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;

        public SettingController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Settings.CountAsync();
            List<Setting> settings = await _context.Settings.Skip(page*3).Take(3).ToListAsync();

            PaginationVM<Setting> vm = new()
            {
                CurrentPage = page+1,
                TotalPage = Math.Ceiling(count/3),
                Items = settings
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

        public async Task<IActionResult> Create(SettingCreateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            bool check = await _context.Settings.AnyAsync(s=>s.Key.ToLower().Trim()==vm.Key.ToLower().Trim()&& s.Value.ToLower().Trim()==vm.Value.ToLower().Trim());
            if (check)
            {
                ModelState.AddModelError("Key && Value", "This setting already exists");
                return View(vm);
            }

            Setting setting = new()
            {
                Key = vm.Key,
                Value = vm.Value,
            };

            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));   

        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Setting existed = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();

            SettingUpdateVM vm = new() { Key = existed.Key, Value = existed.Value };
            return View(vm);


        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, SettingUpdateVM vm)
        {
            if (id <= 0) return BadRequest();
            if(!ModelState.IsValid) return View();
            Setting existed = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
            existed.Key = vm.Key;
            existed.Value = vm.Value;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Setting existed = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();

            _context.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }



    }
}
