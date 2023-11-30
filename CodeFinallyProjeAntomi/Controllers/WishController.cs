using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModel.WishVMs;
using CodeFinallyProjeAntomi.ViewModels.BasketVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CodeFinallyProjeAntomi.Controllers
{
    public class WishController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public WishController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Wish> wishes = await _context.Wishes
                .Where(p => p.IsDeleted == false)
                .ToListAsync();

            List<WishVM> wishVMs = new List<WishVM>();

            foreach (var wish in wishes)
            {
                WishVM wishVM = new WishVM
                {
                    Id = wish.Id,
                    Count = wish.Count  
                };

                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == wish.ProductID);
                if (product != null)
                {
                    wishVM.Title = product.Title;
                    wishVM.Image = product.MainImage;
                    wishVM.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;

                    wishVMs.Add(wishVM);
                }
            }

            ViewData["WishList"] = wishVMs;
            TempData["WishList"] = wishVMs;

            return View();
        }

        public async Task<IActionResult> AddWish(int? id)
        {
            if (id == null) return BadRequest("Id is not be null");

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id))
                return NotFound("Id is InCorrect");

            string? wishList = Request.Cookies["wishList"];

            List<WishVM> wishVMs = null;

            if (!string.IsNullOrEmpty(wishList))
            {
                wishVMs = JsonConvert.DeserializeObject<List<WishVM>>(wishList);

                if (wishVMs.Exists(w => w.Id == id))
                {
                    wishVMs.Find(w => w.Id == id).Count += 1;
                }
                else
                {
                    WishVM wishVM = new WishVM
                    {
                        Id = (int)id,
                        Count = 1
                    };

                    wishVMs.Add(wishVM);
                }
            }
            else
            {
                WishVM wishVM = new WishVM
                {
                    Id = (int)id,
                    Count = 1
                };

                wishVMs = new List<WishVM> { wishVM };
            }

            wishList = JsonConvert.SerializeObject(wishVMs);
            Response.Cookies.Append("wishList", wishList);

            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser appUser = await _userManager.Users
                    .Include(x => x.Wishes.Where(x => x.IsDeleted == false))
                    .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                if (appUser != null)
                {
                    if (appUser.Wishes == null)
                    {
                        appUser.Wishes = new List<Wish>();
                    }

                    Wish wishUser = appUser.Wishes.FirstOrDefault(w => w.ProductID == id);

                    if (wishUser != null)
                    {
                        wishUser.Count = wishVMs.FirstOrDefault(w => w.Id == id).Count;
                    }
                    else
                    {
                        List<Wish> userWishes = appUser.Wishes.ToList();

                        Wish newWish = new Wish
                        {
                            AppUserID = appUser.Id,
                            ProductID = id,
                            Count = wishVMs.FirstOrDefault(w => w.Id == id).Count
                        };
                        userWishes.Add(newWish);

                        appUser.Wishes = userWishes;
                    }

                    await _context.SaveChangesAsync();
                }
            }

            foreach (WishVM wishVM in wishVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == wishVM.Id);

                wishVM.Title = product.Title;
                wishVM.Image = product.MainImage;
                wishVM.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;
                wishVM.ExTag = product.ExTag;
                wishVM.DiscountPrice = product.DisCountPrice;
            }

            return PartialView("_WishListPartial", wishVMs);
        }

        public async Task<IActionResult> Delete(int id)
        {
            Wish wish = await _context.Wishes.FindAsync(id);

            if (wish == null)
            {
                return NotFound();
            }

            _context.Wishes.Remove(wish);
            await _context.SaveChangesAsync();

            string wishList = Request.Cookies["wishList"];

            if (!string.IsNullOrEmpty(wishList))
            {
                List<WishVM> wishVMs = JsonConvert.DeserializeObject<List<WishVM>>(wishList);
                WishVM wishToRemove = wishVMs.FirstOrDefault(w => w.Id == id);

                if (wishToRemove != null)
                {
                    wishVMs.Remove(wishToRemove);
                    wishList = JsonConvert.SerializeObject(wishVMs);
                    Response.Cookies.Append("wishList", wishList);
                }
            }
            return RedirectToAction("Index");
        }

    }
}
