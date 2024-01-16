using BeFree.Areas.Admin.Models.Utilities.Enums;
using BeFree.Areas.Admin.ViewModels.Account;
using BeFree.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFree.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = new()
            {
                Name = vm.Name,
                Surname = vm.Surname,
                UserName = vm.UserName,
                Email = vm.Email,
                Gender = vm.Gender.ToString()
            };

            IdentityResult result = await _userManager.CreateAsync(user,vm.Password);

            if(!result.Succeeded)
            {
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index","Home",new { Area=""});

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home",new {Area = ""});
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(vm.UserNameOrEmail);
            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(vm.UserNameOrEmail);
                if (user is null)
                {
                    ModelState.AddModelError(string.Empty, "Password,email or username incorrect");
                    return View();
                }
            }

            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.IsRemember, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Login is not enable,please try later");
                return View();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Password,email or username incorrect");
                return View();
            }
            return RedirectToAction("Index", "Home", new {Area=""});
        }

        public async Task<IActionResult> CreateRole()
        {
            foreach(UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if(!(await _roleManager.RoleExistsAsync(role.ToString())))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString(),
                    });
                }
            }

            return RedirectToAction("Index", "Home", new { Area = "" });
        }


    }
}
