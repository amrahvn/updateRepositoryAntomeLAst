using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeFinallyProjeAntomi.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int currentPage = 1)
        {
            IQueryable<ContactUs> queries = _context.ContactUs
               .Where(b => b.IsDeleted == false)
               .OrderByDescending(b => b.Id);
               
            var model = PageNatedList<ContactUs>.Create(queries, currentPage, 5, 4);

            return View(model);
        }

        public async Task<IActionResult> Message(int? id)
        {
            if (id == null) return BadRequest();

            ContactUs contactUs=await _context.ContactUs
                 .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if(contactUs == null) return BadRequest();

            return View(contactUs);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            ContactUs contactUs = await _context.ContactUs
                 .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (contactUs == null) return BadRequest();

            contactUs.IsDeleted = true;
            contactUs.DeletedBy = "Amrah";
            contactUs.DeletedAt= DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
