using BeFree.Areas.Admin.Models.Utilities.Enums;
using BeFree.Areas.Admin.Models.Utilities.Extentions;
using BeFree.Areas.Admin.ViewModels;
using BeFree.Areas.Admin.ViewModels.Pagination;
using BeFree.Areas.Admin.ViewModels.Product;
using BeFree.DAL;
using BeFree.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFree.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Products.CountAsync();
            List<Product> products = await _context.Products.Include(p=>p.Category).Skip(page*3).Take(3).ToListAsync();
            PaginationVM<Product> vm = new()
            {
                CurrentPage = page+1,
                TotalPage = Math.Ceiling(count/3),
                Items = products
            };

            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            ProductCreateVM vm = new()
            {
                Categories = await _context.Categories.ToListAsync(),
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if(!ModelState.IsValid)
            {
                vm.Categories = await _context.Categories.ToListAsync();
                return View(vm);
            }

            bool checkProduct = await _context.Products.Include(p => p.Category).AnyAsync(p => p.Name.ToLower().Trim() == vm.Name.ToLower().Trim());
            if(checkProduct)
            {
                vm.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("Name", "This product already exists");
                return View(vm);
            }
            bool checkCategory = await _context.Categories.Include(c=>c.Products).AnyAsync(p => p.Name.ToLower().Trim() == vm.Name.ToLower().Trim());
            if (!checkCategory)
            {
                vm.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("Name", "This category doesn't exists");
                return View(vm);
            }

            if (!vm.Photo.IsValidType(FileType.Image))
            {
                vm.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("Photo", "Photo should be image type");
                return View(vm);
            }
            if (!vm.Photo.IsValidSize(5, FileSize.Megabyte))
            {
                vm.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("Photo", "Photo size can be less or equal 5 mb");
                return View(vm);
            }
            Product product = new()
            {
                Name = vm.Name,
                CategoryId = vm.CategoryId,
                ImageURL = await vm.Photo.CreateAsync(_env.WebRootPath, "assets", "galer"),
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.Include(p=>p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if(existed is null) return NotFound();
            ProductUpdateVM vm = new()
            {
                Name = existed.Name,
                CategoryId = existed.CategoryId,
                ImageURL = existed.ImageURL,
                Categories = await _context.Categories.ToListAsync(),
            };
            return View(vm);

        }
        [HttpPost]

        public async Task<IActionResult> Update(int id,ProductUpdateVM vm)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();
            if(vm.Photo is not null)
            {
                if (!vm.Photo.IsValidType(FileType.Image))
                {
                    vm.Categories = await _context.Categories.ToListAsync();
                    ModelState.AddModelError("Photo", "Photo should be image type");
                    return View(vm);
                }
                if (!vm.Photo.IsValidSize(5, FileSize.Megabyte))
                {
                    vm.Categories = await _context.Categories.ToListAsync();
                    ModelState.AddModelError("Photo", "Photo size can be less or equal 5 mb");
                    return View(vm);
                }
                existed.ImageURL.Delete(_env.WebRootPath, "assets", "galery");
                existed.ImageURL = await vm.Photo.CreateAsync(_env.WebRootPath, "assets", "galery");
            }
            existed.Name = vm.Name;
            existed.CategoryId = vm.CategoryId;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();
            existed.ImageURL.Delete(_env.WebRootPath,"assets","galery");
            _context.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



    }
}
