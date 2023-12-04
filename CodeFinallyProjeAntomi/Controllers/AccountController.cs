
using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Migrations;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModel;
using CodeFinallyProjeAntomi.ViewModel.AccountVms;
using CodeFinallyProjeAntomi.ViewModels.BasketVM;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
//using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;

namespace CodeFinallyProjeAntomi.Controllers
{  
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly SmtpSetting _smtpSetting;

        public AccountController(AppDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, IConfiguration config, IOptions<SmtpSetting> smtpSetting)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _config = config;
            _smtpSetting = smtpSetting.Value;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                IsActive = true
            };

            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(appUser, "Member");

            string templateFullPath = Path.Combine(_env.WebRootPath, "Template", "Email.html");
            string Content = await System.IO.File.ReadAllTextAsync(templateFullPath);

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            string url = Url.Action("EmailConfirem", "Account", new { id = appUser.Id, token = token }, Request.Scheme, Request.Host.ToString());

            Content = Content.Replace("{{url}}", url);

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            mimeMessage.To.Add(MailboxAddress.Parse(appUser.Email));
            mimeMessage.Subject = "Email Confirem";
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = Content
            };


            using (SmtpClient client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpSetting.Host, _smtpSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpSetting.Email, _smtpSetting.Password);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            AppUser appUser = await _userManager.Users
                .Include(u=>u.Baskets.Where(b=>b.IsDeleted==false))
                .FirstOrDefaultAsync(u=>u.NormalizedEmail==loginVM.Email.Trim().ToUpperInvariant());

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View(loginVM);
            }

            //if (!appUser.EmailConfirmed)
            //{
            //    ModelState.AddModelError("", "Unverified Mail");
            //    return View(loginVM);
            //}

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, loginVM.RemindMe, true);

            if (signInResult.IsLockedOut)
            {
                int time = (appUser.LockoutEnd - DateTime.Now).Value.Minutes;

                ModelState.AddModelError("", $"Your account has been blocked for {time} minutes"); ;
                return View(loginVM);
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View(loginVM);
            }

            if(appUser.Baskets !=null && appUser.Baskets.Count() > 0)
            {
                List<BasketVMs> basketVMs = new List<BasketVMs>();
                foreach (Basket basket in appUser.Baskets)
                {
                    BasketVMs basketVM = new BasketVMs
                    {
                        Id = (int)basket.ProductID,
                        Count=basket.Count,
                    };
                    basketVMs.Add(basketVM);
                }
                string cookie=JsonConvert.SerializeObject(basketVMs);

                Response.Cookies.Append("basket", cookie);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> EmailConfirem(string id, string token)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null) return NotFound();

            //if(!appUser.IsActive)
            //{
            //    return BadRequest();
            //}

            //if (appUser.EmailConfirmed) return BadRequest();

            IdentityResult identityResult = await _userManager.ConfirmEmailAsync(appUser, token);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(nameof(Index));
            }

            await _signInManager.SignInAsync(appUser, true);

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            if (!ModelState.IsValid) return View(forgotPasswordVM);

            AppUser appUser = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);

            if (appUser == null) return NotFound();

            string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

            string templateFullPath = Path.Combine(_env.WebRootPath, "Template", "ResetPassword.html");

            string templateContent = await System.IO.File.ReadAllTextAsync(templateFullPath);

            string url = Url.Action("ResetPassword", "Account", new { appUser.Id, token }, Request.Scheme, Request.Host.ToString());

            templateContent = templateContent.Replace("{{reset-url}}", url);

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            mimeMessage.To.Add(MailboxAddress.Parse(appUser.Email));
            mimeMessage.Subject = "Reset Paswword";
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = templateContent
            };


            using (SmtpClient client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpSetting.Host, _smtpSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpSetting.Email, _smtpSetting.Password);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string token, ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid) return View(resetPasswordVM);

            if (string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError("", "Id is inCoorec");
                return View(resetPasswordVM);
            }
            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", "token is inCoorec");
                return View(resetPasswordVM);
            }

            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Appuser not found");
                return View(resetPasswordVM);
            }

            IdentityResult identityResult = await _userManager.ResetPasswordAsync(appUser, token, resetPasswordVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(resetPasswordVM);
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyAccount()
        {
            AppUser appUser = await _userManager.Users
           .Include(u => u.Address.Where(a => a.IsDeleted == false))
           .Include(u=>u.Order.Where(o=>o.IsDeleted==false)).ThenInclude(o=>o.OrderProducts.Where(op=>op.IsDeleted == false))
           .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            MyAccountVM myAccountVM = new MyAccountVM();
            myAccountVM.Addresses = appUser.Address;
            myAccountVM.profileAccountVM = new ProfileAccountVM
            {
                Name = appUser.Name,
                SurName = appUser.Surname,
                Email = appUser.Email,
                UserName = appUser.UserName,
            };

            myAccountVM.Orders = appUser.Order;

            return View(myAccountVM);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyProfileAccount(ProfileAccountVM profileAccountVM)
        {
            TempData["Tab"] = "Account";

            AppUser appUser = await _userManager.Users
            .Include(u => u.Address.Where(a => a.IsDeleted == false))
            .Include(u => u.Order.Where(a => a.IsDeleted == false)).ThenInclude(o=>o.OrderProducts)
            .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            MyAccountVM myAccountVM=new MyAccountVM();
            myAccountVM.Addresses = appUser.Address;
            myAccountVM.profileAccountVM = profileAccountVM;
            myAccountVM.Orders = appUser.Order;

            //if(!ModelState.IsValid)
            //{
            //    myAccountVM.profileAccountVM = profileAccountVM;
            //    return View("MyAccount", myAccountVM);
            //}


                

            if(appUser.NormalizedUserName !=profileAccountVM.UserName.Trim().ToUpperInvariant()) 
            {
              appUser.UserName = profileAccountVM.UserName;
            }
            if (appUser.NormalizedEmail != profileAccountVM.Email.Trim().ToUpperInvariant())
            {
              appUser.Email = profileAccountVM.Email;
            }
            appUser.Name= profileAccountVM.Name;
            appUser.Surname = profileAccountVM.SurName;

            IdentityResult identityResult=await _userManager.UpdateAsync(appUser);

            if(!identityResult.Succeeded)
            {
               foreach (IdentityError result in identityResult.Errors)
                {
                    ModelState.AddModelError("", result.Description);
                }
                return View("MyAccount",myAccountVM);
            }

            await _signInManager.SignInAsync(appUser, isPersistent: true);


            return View("MyAccount", myAccountVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddAddress(Address address)
        {
            TempData["Tab"] = "Address";

            AppUser appUser = await _userManager.Users
            .Include(u=>u.Address.Where(a=>a.IsDeleted==false))
          .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            MyAccountVM myAccountVM = new MyAccountVM();

            myAccountVM.profileAccountVM = new ProfileAccountVM
            {
                Name=appUser.Name,
                SurName=appUser.Surname,
                Email=appUser.Email,
                UserName=appUser.UserName,
            };

            myAccountVM.Addresses = appUser.Address;

            if (!ModelState.IsValid)
            {
                myAccountVM.Address = address;
                TempData["addreess"] = "true";
                return View("MyAccount", myAccountVM);
            }

            if(address.IsDefault==true )
            {
                if (appUser.Address != null && appUser.Address.Count() > 0)
                {
                    foreach (Address address1 in appUser.Address)
                    {
                        address1.IsDefault = false;
                    }
                }
            }
            else
            {
                if(appUser.Address == null && appUser.Address.Count() <= 0)
                {
                    address.IsDefault = true;
                }
            }

            address.UserID = appUser.Id;
            address.CreatedBy = appUser.Name + " " + appUser.Surname;
            address.CreatedAt = DateTime.Now;

            await _context.Address.AddAsync(address);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyAccount));
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> EditAdress(int? id)
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (id == null) return NotFound();

            Address address=await _context.Address.FirstOrDefaultAsync(e=>e.IsDeleted == false && e.Id==id && e.UserID==appUser.Id);

            if(address == null) return NotFound();

            return PartialView("_editAdressPArtial",address);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdress(Address address)
        {
            TempData["Tab"] = "Address";

            AppUser appUser = await _userManager.Users
            .Include(u => u.Address.Where(a => a.IsDeleted == false))
          .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            MyAccountVM myAccountVM = new MyAccountVM();

            myAccountVM.profileAccountVM = new ProfileAccountVM
            {
                Name = appUser.Name,
                SurName = appUser.Surname,
                Email = appUser.Email,
                UserName = appUser.UserName,
            };

            myAccountVM.Addresses = appUser.Address;

            if (!ModelState.IsValid)
            {
                myAccountVM.Address = address;
                TempData["editadress"] = "true";
                return View("MyAccount", myAccountVM);
            }

            Address dbAddress=appUser.Address.FirstOrDefault(a=>a.Id==address.Id);


            if (address.IsDefault == true)
            {
                if (appUser.Address != null && appUser.Address.Count() > 0)
                { 
                    foreach (Address address1 in appUser.Address)
                    {
                        address1.IsDefault = false;
                    }
                }
                dbAddress.IsDefault=true;
            }
            else
            {
                if (appUser.Address == null && appUser.Address.Count() <= 0)
                {
                    dbAddress.IsDefault = true;
                }

            }

            dbAddress.CompanyName=address.CompanyName;
            //dbAddress.County=address.County;
            dbAddress.Street=address.Street;
            dbAddress.Street2=address.Street2;
            dbAddress.Town=address.Town;
            dbAddress.State=address.State;
            dbAddress.PostalCode=address.PostalCode;
            dbAddress.Phone=address.Phone;
            dbAddress.OrderNotes=address.OrderNotes;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyAccount));
        }
    }
}
    