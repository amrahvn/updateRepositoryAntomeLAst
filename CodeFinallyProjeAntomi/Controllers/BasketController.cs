using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModels.BasketVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CodeFinallyProjeAntomi.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null) return BadRequest("Id is not be null");

            //Product product = await _context.Products
                //.Include(p=>p.productImages.Where(pi=>pi.IsDeleted==false))
                //.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (!await _context.Products.AnyAsync(p=>p.IsDeleted==false && p.Id==id)) return NotFound("Id is InCorrect");

            string? basket = Request.Cookies["basket"];

            List<BasketVMs> basketVMs = null;

            if (!string.IsNullOrEmpty(basket))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVMs>>(basket);

                if (basketVMs.Exists(b => b.Id == id))
                {
                    basketVMs.Find(b => b.Id == id).Count += 1;
                }
                else
                {
                    BasketVMs basketVM = new BasketVMs
                    {
                        Id = (int)id,
                        Count=1
                    };

                    basketVMs.Add(basketVM);
                }
            }
            else
            {
                BasketVMs basketVM = new BasketVMs
                {
                    Id = (int) id,
                    Count=1 
                };

                basketVMs = new List<BasketVMs> { basketVM };

                
            }
             basket= JsonConvert.SerializeObject(basketVMs);

            Response.Cookies.Append("basket", basket);

            if(User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser appUser=await _userManager.Users
                    .Include(x => x.Baskets.Where(x=>x.IsDeleted==false))
                    .FirstOrDefaultAsync(u=>u.UserName==User.Identity.Name);

                 if(appUser!=null && appUser.Baskets != null && appUser.Baskets.Count()>0)
                {
                    Basket basketUser = appUser.Baskets.FirstOrDefault(b => b.ProductID == id);

                    if(basketUser != null)
                    {
                        basketUser.Count = basketVMs.FirstOrDefault(b => b.Id == id).Count;
                    }
                    else
                    {
                        Basket basketUsernew=new Basket
                        {
                           AppUserID=appUser.Id,
                           ProductID=id,
                           Count = basketVMs.FirstOrDefault(b => b.Id == id).Count
                        };
                        await _context.Baskets.AddAsync(basketUsernew);
                    }
                }
                else
                {
                    Basket basketUsernew = new Basket
                    {
                        AppUserID = appUser.Id,
                        ProductID = id,
                        Count = basketVMs.FirstOrDefault(b => b.Id == id).Count
                    };
                    await _context.Baskets.AddAsync(basketUsernew);
                }
                    await _context.SaveChangesAsync();
            }
          

            foreach (BasketVMs basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);

                basketVM.Title = product.Title;
                basketVM.Image = product.MainImage;
                basketVM.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;
                basketVM.ExTag = product.ExTag;
                basketVM.DiscountPrice = product.DisCountPrice;
            }

            return PartialView("_BasketPArtial", basketVMs);
        }

        public async Task<IActionResult> Delete(int id)
        {
            string basket = Request.Cookies["basket"];

            if (string.IsNullOrEmpty(basket))
            {
                return RedirectToAction("Index");
            }

            List<BasketVMs> basketVMs = JsonConvert.DeserializeObject<List<BasketVMs>>(basket);

            BasketVMs itemToRemove = basketVMs.FirstOrDefault(b => b.Id == id);

            if (itemToRemove != null)
            {
                basketVMs.Remove(itemToRemove);

                basket = JsonConvert.SerializeObject(basketVMs);
                Response.Cookies.Append("basket", basket);

                if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
                {
                    AppUser appUser = await _userManager.Users
                        .Include(x => x.Baskets.Where(x => x.IsDeleted == false))
                        .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

                    if (appUser != null && appUser.Baskets != null && appUser.Baskets.Count() > 0)
                    {
                        Basket basketUser = appUser.Baskets.FirstOrDefault(b => b.ProductID == id);

                        if (basketUser != null)
                        {
                            _context.Baskets.Remove(basketUser);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
