using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing.Drawing2D;

namespace CodeFinallyProjeAntomi.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders.Where(s => s.IsDeleted == false).ToListAsync();

            return View(sliders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (slider.File == null)
            {
                ModelState.AddModelError("File", "Min 1 file entery plese");
                return View(slider);
            }

            if (string.IsNullOrEmpty(slider.File.FileName))
            {
                ModelState.AddModelError("File", "File name is null or empty");
                return View(slider);
            }

            if (!slider.File.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("File", "File is wrong");
                return View(slider);
            }

            if ((slider.File.Length / 1024) > 400)
            {
                ModelState.AddModelError("File", "File must be 400kb");
                return View(slider);
            }

            string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + slider.File.FileName
            .Substring(slider.File.FileName.LastIndexOf('.'));

            string filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Slider", fileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await slider.File.CopyToAsync(fileStream);
            }

            slider.Image = fileName;


            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult>Update(int? id)
        {
            if(id == null) return BadRequest();

            Slider slider=await _context.Sliders
                .FirstOrDefaultAsync(s=>s.IsDeleted==false && s.Id==id);

            if(slider == null) return BadRequest();

            return View(slider);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, Slider slider)
        {
            if (id == null) return BadRequest();

            if (id != slider.Id) return BadRequest();

            Slider dbSlider = await _context.Sliders
                .FirstOrDefaultAsync(s => s.IsDeleted == false && s.Id == id);

            if (dbSlider == null) return NotFound();

            if(slider.File != null)
            {
                if ((slider.File.Length / 1024) > 300)
                {
                    ModelState.AddModelError("File", "File must be 300kb");
                    return View(slider);
                }

                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + slider.File.FileName
                    .Substring(slider.File.FileName.LastIndexOf('.'));

                string filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Slider", dbSlider.Image);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Slider", fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await slider.File.CopyToAsync(fileStream);
                }

                dbSlider.Image = fileName;
            }

            dbSlider.Title = slider.Title;
            dbSlider.Description = slider.Description;
            dbSlider.MainTitle = slider.MainTitle;
            dbSlider.SubTitle= slider.SubTitle;
            dbSlider.Link = slider.Link;
           
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Slider slider = await _context.Sliders
               .FirstOrDefaultAsync(s => s.IsDeleted == false && s.Id == id);

            if (slider == null) return BadRequest();

            slider.IsDeleted = true;
            slider.DeletedBy = "Amrah";
            slider.DeletedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
