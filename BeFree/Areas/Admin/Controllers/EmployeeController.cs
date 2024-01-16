using BeFree.Areas.Admin.Models.Utilities.Enums;
using BeFree.Areas.Admin.Models.Utilities.Extentions;
using BeFree.Areas.Admin.ViewModels;
using BeFree.Areas.Admin.ViewModels.Employee;
using BeFree.Areas.Admin.ViewModels.Pagination;
using BeFree.DAL;
using BeFree.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace BeFree.Areas.Admin.Controllers
{
    [Area("Admin")]

    [Authorize(Roles = "Admin,Moderator")]
    [AutoValidateAntiforgeryToken]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Employees.CountAsync();
            List<Employee> employees = await _context.Employees.Include(e => e.Position).Skip(page * 3).Take(3).ToListAsync();

            PaginationVM<Employee> vm = new()
            {
                CurrentPage = page + 1,
                TotalPage = Math.Ceiling(count / 3),
                Items = employees
            };
            return View(vm);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create()
        {
            EmployeeCreateVM vm = new()
            {
                Positions = await _context.Positions.ToListAsync(),
            };
            return View(vm);

        }

        [HttpPost]


        public async Task<IActionResult> Create(EmployeeCreateVM vm)
        {
            if (!ModelState.IsValid) return View();
            bool checkPosition = await _context.Positions.Include(p=>p.Employees).AnyAsync(p => p.Id == vm.PositionId);
            if (!checkPosition)
            {
                vm.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("Position", "This position doesn't exists");
                return View(vm);
            }
            if (!vm.Photo.IsValidType(FileType.Image))
            {
                vm.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("Photo", "Photo should be image type");
                return View(vm);
            }
            if (!vm.Photo.IsValidSize(5, FileSize.Megabyte))
            {
                vm.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("Photo", "Photo size can be less or equal 5 mb");
                return View(vm);
            }

            Employee employee = new()
            {
                Name = vm.Name,
                Surname = vm.Surname,
                Facebook = vm.Facebook,
                GooglePlus = vm.GooglePlus,
                Twitter = vm.Twitter,
                PositionId = vm.PositionId,
                ImageURL = await vm.Photo.CreateAsync(_env.WebRootPath, "assets","img")
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Employee existed = await _context.Employees.Include(e => e.Position).FirstOrDefaultAsync(e => e.Id == id);
            if (existed is null) return NotFound();
            EmployeeUpdateVM vm = new()
            {
                Name = existed.Name,
                Surname = existed.Surname,
                Facebook = existed.Facebook,
                GooglePlus = existed.GooglePlus,
                Twitter = existed.Twitter,
                ImageURL = existed.ImageURL,
                PositionId = existed.PositionId,
                Positions = await _context.Positions.ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, EmployeeUpdateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Positions = await _context.Positions.ToListAsync();
                return View(vm);
            }
            Employee existed = await _context.Employees.Include(e => e.Position).FirstOrDefaultAsync(e => e.Id == id);
            if (existed is null) return NotFound();
            if (vm.Photo is not null)
            {
                if (!vm.Photo.IsValidType(FileType.Image))
                {
                    vm.Positions = await _context.Positions.ToListAsync();
                    ModelState.AddModelError("Photo", "Photo should be image type");
                    return View(vm);
                }
                if (!vm.Photo.IsValidSize(5, FileSize.Megabyte))
                {
                    vm.Positions = await _context.Positions.ToListAsync();
                    ModelState.AddModelError("Photo", "Photo size can be less or equal 5 mb");
                    return View(vm);
                }
				string NewImage = await vm.Photo.CreateAsync(_env.WebRootPath, "assets", "img");
				existed.ImageURL.Delete(_env.WebRootPath, "assets", "img");
                existed.ImageURL = NewImage;

			}

            existed.Name = vm.Name;
            existed.Surname = vm.Surname;
            existed.PositionId = vm.PositionId;
            existed.Facebook = vm.Facebook;
            existed.GooglePlus = vm.GooglePlus;
            existed.Twitter = vm.Twitter;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        [Authorize(Roles = "Admin,Moderator")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Employee existed = await _context.Employees.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();
            existed.ImageURL.Delete(_env.WebRootPath, "assets", "img");
            _context.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




    }
}
