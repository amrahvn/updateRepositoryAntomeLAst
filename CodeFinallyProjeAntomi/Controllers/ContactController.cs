
using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeFinallyProjeAntomi.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddMessage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMessage(ContactUs contactUs)
        {
            if (contactUs == null) return BadRequest();

            if(!ModelState.IsValid)
            {
                return View(nameof(Index));
            }

            if (contactUs.Name == null)
            {
                ModelState.AddModelError("Name","Name not a null");
                return View(contactUs);
            }

            ContactUs contactUs1= new ContactUs
            {
                Name = contactUs.Name,
                Email = contactUs.Email,
                Subject = contactUs.Subject,
                Message= contactUs.Message,
            };

            await _context.ContactUs.AddAsync(contactUs1);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
