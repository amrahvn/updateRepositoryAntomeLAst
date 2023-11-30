using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using Newtonsoft.Json;

namespace CodeFinallyProjeAntomi.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId, int? sortingCriteria, int currentPage = 1)
        {
            IQueryable<Product> products;

            if (categoryId.HasValue)
            {
                products = _context.Products
                    .Include(p => p.Category)
                    .Where(p => !p.IsDeleted && p.Category.Id == categoryId);
            }
            else
            {
                products = _context.Products
                    .Include(p => p.Category)
                    .Where(p => !p.IsDeleted);
            }

            ViewBag.SortingCriteria = sortingCriteria;

            switch (sortingCriteria)
            {
                case 1:
                    products = products.OrderByDescending(p => p.Id);
                    break;
                case 2:
                    products = products.OrderByDescending(p => p.Rating);
                    break;
                case 3:
                    products = products.OrderByDescending(p => p.CreatedAt);
                    break;
                case 4:
                    products = products.OrderBy(p => p.Price);
                    break;
                case 5:
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case 6:
                    products = products.OrderBy(p => p.Title);
                    break;
                default:
                    products = products.OrderByDescending(p => p.Id);
                    break;
            }

            ViewBag.SelectedCategory = categoryId;

            ViewBag.Categories = _context.Categories
                .Where(c => c.Children != null && c.Children.Any())
                .ToList();

            return View(PageNatedList<Product>.Create(products, currentPage, 12, 6));
        }

        public async Task<IActionResult> Search(string search,int? categoryId)
        {
            List<Product> products = null;

            if (categoryId != null && await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == categoryId))
            {
                products = await _context.Products
                    .Where(p => p.IsDeleted == false && p.CategoryId == (int)categoryId ||
                    (p.Title.ToLower().Contains(search.ToLower()) ||
                    p.BrandId != null ? p.Brand.Name.ToLower().Contains(search.ToLower()) : true)).ToListAsync();
            }
            else
            {
                products = await _context.Products
                    .Where(p => p.IsDeleted == false ||
                    (p.Title.ToLower().Contains(search.ToLower())||
                    p.BrandId != null ? p.Brand.Name.ToLower().Contains(search.ToLower()) : true)
                    || p.Category.Name.ToLower().Contains(search.ToLower())
                    ).ToListAsync();
            }
            return PartialView("_SearchPartial", products);
        }

        [HttpPost]
        public IActionResult Rate(int productId, int rating)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingRating = _context.UserRatings
                .FirstOrDefault(ur => ur.ProductId == productId && ur.UserId == userId);

            if (existingRating != null)
            {
                existingRating.Rating = rating;
            }
            else
            {
                var newRating = new UserRating
                {
                    ProductId = productId,
                    UserId = userId,
                    Rating = rating
                };

                _context.UserRatings.Add(newRating);
            }

            _context.SaveChanges();

            UpdateProductRating(productId);

            return Json(new { UpdatedRating = rating });
        }

        private void UpdateProductRating(int productId)
        {
            var product = _context.Products
                .Include(p => p.UserRatings)
                .FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                double averageRating = product.UserRatings.Average(ur => ur.Rating);
                product.Rating = (int)averageRating;

                _context.SaveChanges();
            }
        }

    }
}
