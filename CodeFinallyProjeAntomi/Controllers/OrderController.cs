using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Enums;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModel.OrderVMs;
using CodeFinallyProjeAntomi.ViewModels.BasketVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeFinallyProjeAntomi.Controllers
{
    [Authorize(Roles = "Member")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Address.Where(a => a.IsDeleted == false && a.IsDefault))
                .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (appUser.Baskets == null || appUser.Baskets.Count() <= 0)
            {
                TempData["warning"] = "Select Product";

                return RedirectToAction("Index", "Product");
            }

            OrderVM orderVM = new OrderVM
            {
                Order = new Order
                {
                    County = appUser.Address.First().County,
                    Email = appUser.Email,
                    Name = appUser.Name,
                    Surnama = appUser.Surname,
                    CompanyName = appUser.Address.First().CompanyName,
                    Street = appUser.Address.First().Street,
                    Street2 = appUser.Address.First().Street2,
                    Town = appUser.Address.First().Town,
                    State = appUser.Address.First().State,
                    Phone = appUser.Address.First().Phone,
                    OrderNotes = appUser.Address.First().OrderNotes,
                    PostalCode=appUser.Address.First().PostalCode,
                },
                BasketVM= appUser.Baskets.Select(x => new BasketVMs
                {
                    Id = (int)x.ProductID,
                    Count=x.Count,
                    ExTag=x.Product.ExTag,
                    Image=x.Product.MainImage,
                    Price=x.Product.DisCountPrice>0 ? x.Product.DisCountPrice:x.Product.Price,
                    Title=x.Product.Title,
                }).ToList(),
            };
            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(Order order)
        {
            AppUser appUser = await _userManager.Users
               .Include(u => u.Address.Where(a => a.IsDeleted == false && a.IsDefault))
               .Include(u => u.Order)
               .Include(u => u.Baskets.Where(b => b.IsDeleted == false)).ThenInclude(b => b.Product)
               .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            OrderVM orderVM = new OrderVM
            {
                Order = order,
                BasketVM = appUser.Baskets.Select(x => new BasketVMs
                {
                    Id = (int)x.ProductID,
                    Count = x.Count,
                    ExTag = x.Product.ExTag,
                    Image = x.Product.MainImage,
                    Price = x.Product.DisCountPrice > 0 ? x.Product.DisCountPrice : x.Product.Price,
                    Title = x.Product.Title,
                }).ToList(),
            };

            if (!ModelState.IsValid) return View(orderVM);


            if (appUser.Baskets == null || appUser.Baskets.Count() <= 0)
            {
                TempData["warning"] = "Select Product";

                return RedirectToAction("Index", "Product");
            }

            List<OrderProduct>orderProducts=new List<OrderProduct>();

            foreach (Basket basket in appUser.Baskets)
            {
                basket.IsDeleted = true;

                OrderProduct orderProduct = new OrderProduct
                {
                    Count=basket.Count,
                    CreatedBy=appUser.Name + " "+appUser.Surname,
                    CreatedAt=DateTime.Now,
                    ExTag=basket.Product.ExTag,
                    Price=basket.Product.DisCountPrice>0?basket.Product.DisCountPrice:basket.Product.Price,
                    ProductID=basket.Product.Id,
                    Title=basket.Product.Title,
                };

                orderProducts.Add(orderProduct);
            }

            order.OrderProducts=orderProducts;
            order.Status = Status.Pending;
            order.No=appUser.Order !=null && appUser.Order.Count()>0?appUser.Order.OrderByDescending(o=>o.Id).FirstOrDefault().No+1:1;
            order.CreatedBy=appUser.Name+" "+appUser.Surname;
            order.UserId = appUser.Id;

            await _context.AddAsync(order);
            await _context.SaveChangesAsync();

            Response.Cookies.Append("basket", "");

            TempData["success"] = "Your order has been successfully registered";

            return RedirectToAction("Index", "Product");
        }
    }
}
 
 