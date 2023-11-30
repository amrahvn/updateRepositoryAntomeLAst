using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModels.DetailVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeFinallyProjeAntomi.Controllers
{
    public class DetailController : Controller
    {
        private readonly AppDbContext _context;

        public DetailController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if(id == null) return NotFound();

            Product? product = await _context.Products
                 .Include(p => p.productImages.Where(pi => pi.IsDeleted == false))
                 .Include(c => c.Category).Where(c => c.IsDeleted == false && c.Id == id)
                 .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return NotFound();  

            return View(product);
        }
    }
}
