using BeFree.DAL;
using BeFree.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFree.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            HomeVM vm = new()
            {
                Categories = await _context.Categories.Include(c => c.Products).ToListAsync(),
                Employees = await _context.Employees.Include(e => e.Position).ToListAsync(),
                Positions = await _context.Positions.Include(p => p.Employees).ToListAsync(),
                Products = await _context.Products.Include(p => p.Category).ToListAsync(),
            };
            return View(vm);
        }
    }
}
