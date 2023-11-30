using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CodeFinallyProjeAntomi.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin")]
    public class BrandController : Controller
    {
       
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int currentPage=1)
        {
            IQueryable<Brand> queries = _context.Brands
                .Include(b=>b.products.Where(p=>p.IsDeleted==false))
                .Where(b => b.IsDeleted == false)
                .OrderByDescending(b=>b.Id)
                ;

          var model=  PageNatedList<Brand>.Create(queries, currentPage, 5, 4);

            return View(model);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest("id not found");

            Brand brand=await _context.Brands
                .Include(b=>b.products.Where(b=>b.IsDeleted==false))
                .FirstOrDefaultAsync(b=>b.IsDeleted == false && b.Id==id);

            if(brand == null) return NotFound();

            return View(brand);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (!ModelState.IsValid) return View(brand);

            if(await _context.Brands.AnyAsync(b=>b.IsDeleted==false && b.Name.ToLower() == brand.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"{brand.Name} Already Exist");
                return View(brand);
            }

            brand.Name=brand.Name.Trim();

            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if(brand == null) return NotFound();

            return View(brand);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int? id,Brand brand)
        {
            if(id==null) return BadRequest();

            if(id !=brand.Id) return BadRequest();

            if(!ModelState.IsValid) return View(brand);

            Brand dbbrand = await _context.Brands
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);


            if(dbbrand == null) return NotFound();

            if (await _context.Brands.AnyAsync(b => b.IsDeleted == false && b.Name.ToLower() == brand.Name.Trim().ToLower() && b.Id!=dbbrand.Id))
            {
                ModelState.AddModelError("Name", $"{brand.Name} Already Exist");
                return View(brand);
            }

            dbbrand.Name=brand.Name.Trim();
            dbbrand.UpdatedBy = "Amrah";
            dbbrand.UpdatedAt= DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest("id not found");

            Brand brand = await _context.Brands
                .Include(b => b.products.Where(b => b.IsDeleted == false))
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (brand == null) return NotFound();

            return View(brand);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBrand(int? id)
        {
            if (id == null) return BadRequest("id not found");

            Brand brand = await _context.Brands
                .Include(b => b.products.Where(b => b.IsDeleted == false))
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (brand == null) return NotFound();

            brand.IsDeleted=true;
            brand.DeletedBy = "Amrah";
            brand.UpdatedAt= DateTime.Now;

            if(brand.products != null !=null && brand.products.Count() > 0)
            {
                foreach (Product product in brand.products)
                {
                    product.BrandId = null;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
