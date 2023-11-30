using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Interfaces;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModel.WishVMs;
using CodeFinallyProjeAntomi.ViewModels.BasketVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel;

namespace CodeFinallyProjeAntomi.Services
{
    public class LayoutService:IlayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public LayoutService(AppDbContext context, IHttpContextAccessor contextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public async Task<List<BasketVMs>> GetBasketsAsync()
        {
            List<BasketVMs> basketVMs = null;

          

            if(_contextAccessor.HttpContext.User.Identity.IsAuthenticated && _contextAccessor.HttpContext.User.IsInRole("Member"))
            {
                AppUser appUser=await _userManager.Users
                    .Include(u=>u.Baskets.Where(u=>u.IsDeleted==false))
                    .ThenInclude(b=>b.Product)
                    .FirstOrDefaultAsync(u=>u.UserName ==_contextAccessor.HttpContext.User.Identity.Name);

                 basketVMs=new List<BasketVMs>();
                
                if(appUser.Baskets !=null && appUser.Baskets.Count()> 0)
                {
                    foreach (Basket basket in appUser.Baskets)
                    {
                        BasketVMs basketVM = new BasketVMs
                        {
                            Id = (int)basket.ProductID,
                            Count=basket.Count,
                            ExTag=basket.Product.ExTag,
                            Price=basket.Product.DisCountPrice >0? basket.Product.DisCountPrice:basket.Product.Price,
                            Image=basket.Product.MainImage,
                            Title=basket.Product.Title,
                        };

                        basketVMs.Add(basketVM);
                    }
                }

                string cookie=JsonConvert.SerializeObject(basketVMs);
                _contextAccessor.HttpContext.Response.Cookies.Append("basket", cookie);
            }
            else
            {
                string cookie = _contextAccessor.HttpContext.Request.Cookies["basket"];

                if (!string.IsNullOrEmpty(cookie))
                {
                    basketVMs = JsonConvert.DeserializeObject<List<BasketVMs>>(cookie);
                }
                else
                {
                    basketVMs = new List<BasketVMs>();
                }
                foreach (BasketVMs basketVM in basketVMs)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);

                    basketVM.Title = product.Title;
                    basketVM.Image = product.MainImage;
                    basketVM.Price = product.DisCountPrice > 0 ? product.DisCountPrice : product.Price;
                    basketVM.ExTag = product.ExTag;
                }
            }


            return basketVMs;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            List<Category> categories = await _context.Categories
                .Include(c=>c.Children.Where(ch=>ch.IsDeleted==false))
                .Where(c=>c.IsDeleted==false && c.IsMain==true).ToListAsync();

            return categories;
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Dictionary<string,string> settings =await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);

            return settings;
        }

        public async Task<List<WishVM>> GetWishListAsync()
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

            return wishVMs;
        }
    }
}
