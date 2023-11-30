
using CodeFinallyProjeAntomi.Areas.Manage.ViewModels.AccountVMs;
using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModel.AccountVms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace CodeFinallyProjeAntomi.Areas.Manage.Controllers
{
    [Area("manage")]
    
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(AppDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager = null)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View(loginVM);
            }

            //if(await _userManager.CheckPasswordAsync(appUser, loginVM.Password))
            //{
            //    ModelState.AddModelError("", "Email or Password is incorrect");
            //    return View(loginVM);
            //}

            if (!appUser.IsActive)
            {
                return Unauthorized();
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, true, true);

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View(loginVM);
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account is blocked");
                return View(loginVM);
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [Authorize(Roles ="SuperAdmin,Admin")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            ProfileVM profileVM = new ProfileVM
            {
                Name = appUser.Name,
                SurName=appUser.Surname,
                Email=appUser.Email,
                UserName=appUser.UserName,
            };

            return View(profileVM);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            if(!ModelState.IsValid) return View(profileVM);

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (appUser.NormalizedUserName !=profileVM.UserName.Trim().ToUpperInvariant())
            {
                appUser.UserName = profileVM.UserName.Trim();
            }

            if (appUser.NormalizedEmail != profileVM.Email.Trim().ToUpperInvariant())
            {
                appUser.Email=profileVM.Email.Trim();
            }
           
            appUser.Name = profileVM.Name;
            appUser.Surname = profileVM.SurName;

            IdentityResult identityResult= await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(profileVM);
            }

            if(!string.IsNullOrWhiteSpace(profileVM.CurrentPassword))
            {
                if(!await _userManager.CheckPasswordAsync(appUser, profileVM.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "CurrentPassword is inCorect");
                    return View(profileVM);
                }

                string token=await _userManager.GeneratePasswordResetTokenAsync(appUser);

                identityResult= await _userManager.ResetPasswordAsync(appUser, token, profileVM.NewPassword);

                if(!identityResult.Succeeded)
                {
                    foreach (IdentityError identityError in identityResult.Errors)
                    {
                        ModelState.AddModelError("", identityError.Description);
                    }
                    return View(profileVM);
                }
            }

            await _signInManager.SignInAsync(appUser, isPersistent: true);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> ChangeRole()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            RoleVM roleVM = new RoleVM
            {
                Roles = appUser.Roles,
            };

            return View(roleVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(RoleVM roleVM)
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            appUser.Roles = roleVM.Roles;

            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(roleVM);
            }

            await _signInManager.SignInAsync(appUser, isPersistent: true);

            return RedirectToAction("Index", "Dashboard");
        }



        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Index));
        }

        #region 

        //[HttpGet]
        //public async Task<IActionResult> CreateRole()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));

        //    return Ok("Role created");
        //}

        //[HttpGet]
        //public async Task<IActionResult> CreateSuperAdmin()
        //{
        //    AppUser appUser = new AppUser()
        //    {
        //        UserName = "Amrah123",
        //        Email = "amrah123@gmail.com"
        //    };

        //    IdentityResult result = await _userManager.CreateAsync(appUser, " ");


        //    if (result.Succeeded)
        //    {
        //        await _userManager.AddToRoleAsync(appUser, "SuperAdmin");
        //        return Ok("Super Admin Yarandi");
        //    }
        //    else
        //    {

        //        return BadRequest("Error");
        //    }
        //}


        //[HttpGet]
        //public async Task<IActionResult> CreateSuperAdmin()
        //{
        //    AppUser appUser = new AppUser()
        //    {
        //        UserName = "Amrah123",
        //        Email="amrah123@gmail.com"

        //    };

        //    await _userManager.CreateAsync(appUser, "Amrah123");
        //    await _userManager.AddToRoleAsync(appUser, "SuperAdmin");

        //    return Ok("Super Admin Yarandi");
        //}



        //[HttpGet]
        //public async Task<IActionResult> DeleteRoles()
        //{
        //    // "SuperAdmin", "Admin", "Member" rollerini silebilirsiniz
        //    string[] roleNames = { "SuperAdmin", "Admin", "Member" };

        //    foreach (var roleName in roleNames)
        //    {
        //        // Rolü bul
        //        var role = await _roleManager.FindByNameAsync(roleName);

        //        if (role != null)
        //        {
        //            // Rolü sil
        //            await _roleManager.DeleteAsync(role);
        //        }
        //    }
        //    return Ok("Roller silindi");
        //}
        #endregion

    }

}
