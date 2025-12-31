using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Application.Common;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            LoginViewModel vmLogin = new LoginViewModel
            {
                RedirectUrl = returnUrl,
            };

            return View(vmLogin);
        }

        public async Task<IActionResult> Register()
        {
            bool adminRoleExists = await _roleManager.RoleExistsAsync(AspNetUserRoleConstants.AdminRole);
            if (!adminRoleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(AspNetUserRoleConstants.AdminRole));
                await _roleManager.CreateAsync(new IdentityRole(AspNetUserRoleConstants.UserRole));
            }

            RegisterViewModel vmRegister = new RegisterViewModel
            {
                RoleList = _roleManager.Roles.Select(x => new SelectListItem(x.Name, x.Id)).ToList(),
            };

            return View(vmRegister);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vmRegister)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Name = vmRegister.Name,
                Email = vmRegister.Email,
                PhoneNumber = vmRegister.PhoneNumber,
                NormalizedEmail = vmRegister.Email.ToUpper(),
                EmailConfirmed = true,
                UserName = vmRegister.Name,
                CreatedAt = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(user, vmRegister.Password);
            if (result.Succeeded)
            {
                string userRole = !string.IsNullOrWhiteSpace(vmRegister.Role)
                    ? vmRegister.Role : AspNetUserRoleConstants.UserRole;

                await _userManager.AddToRoleAsync(user, userRole);
                await _signInManager.SignInAsync(user, isPersistent: false);

                if (!string.IsNullOrWhiteSpace(vmRegister.RedirectUrl))
                    return LocalRedirect(vmRegister.RedirectUrl);
                else
                    return RedirectToAction("Home", nameof(HomeController.Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            vmRegister.RoleList = _roleManager.Roles.Select(x => new SelectListItem(x.Name, x.Id)).ToList();

            return View(vmRegister);
        }
    }
}
