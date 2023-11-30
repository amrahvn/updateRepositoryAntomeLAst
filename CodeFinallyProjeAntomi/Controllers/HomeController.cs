using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.Services;
using CodeFinallyProjeAntomi.ViewModels.DetailVMs;
using CodeFinallyProjeAntomi.ViewModels.HomeVms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeFinallyProjeAntomi.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public  HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            HomeVM homeVM = new HomeVM
            {
                sliders = await _context.Sliders.Where(s => s.IsDeleted == false).ToListAsync(),
                categories = await _context.Categories
                .Include(p => p.Products)
                .Where(c => c.IsDeleted == false && c.IsMain == true).ToListAsync(),

                products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync(),

                NewArrival=await _context.Products.Where(p=>p.IsDeleted == false && p.NewArrival).OrderByDescending(p=>p.Id).Take(10).ToListAsync(),

                BestSeller = await _context.Products.Where(p => p.IsDeleted == false && p.BestSeller).OrderByDescending(p => p.Id).Take(10).ToListAsync(),

                Featured = await _context.Products.Where(p => p.IsDeleted == false && p.Featured).OrderByDescending(p => p.Id).Take(10).ToListAsync(),
            };

            return View(homeVM);
        }

    }
}
