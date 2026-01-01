using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VilaManagement.Application.Common;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;
using VilaManagement.Web.ViewModels;

namespace VilaManagement.Web.Controllers
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
                var userRole = vmRegister.Role switch
                {
                    string role when !string.IsNullOrWhiteSpace(role) => role,
                    _ => AspNetUserRoleConstants.UserRole
                };

                await _userManager.AddToRoleAsync(user, userRole);
                await _signInManager.SignInAsync(user, isPersistent: false);

                return vmRegister.RedirectUrl switch
                {
                    string url when !string.IsNullOrWhiteSpace(url) => LocalRedirect(url),
                    _ => RedirectToAction(nameof(HomeController.Index), "Home")
                };
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
