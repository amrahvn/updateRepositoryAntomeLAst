using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeFinallyProjeAntomi.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BLogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BLogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int currentPage = 1)
        {
            IQueryable<Blog> queries = _context.Blogs
               .OrderByDescending(b => b.Id);

            var model = PageNatedList<Blog>.Create(queries, currentPage, 5, 4);

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (blog == null) return BadRequest();

            if(!ModelState.IsValid)
            {
                return View(blog);
            }

            if (!blog.MainFile.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("MainFile", "MainFile not found");
                return View(blog);
            }

            if ((blog.MainFile.Length / 1024) > 400)
            {
                ModelState.AddModelError("MainFile", "MainFile must be 400kb");
                return View(blog); ;
            }
            string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + blog.MainFile.FileName
            .Substring(blog.MainFile.FileName.LastIndexOf('.'));

            string filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Blog", fileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await blog.MainFile.CopyToAsync(fileStream);
            }

            blog.Image = fileName;

            blog.Title = blog.Title.Trim();
            blog.news1 = blog.news1.Trim();
            blog.news2 = blog.news2.Trim();
            blog.news3 = blog.news3.Trim();
            blog.news4 = blog.news4.Trim();

            await _context.AddAsync(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest("id not found");

            Blog blog = await _context.Blogs
                .FirstOrDefaultAsync(b=>b.Id==id);

            if (blog == null) return NotFound();

            return View(blog);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Blog blog = await _context.Blogs
                .FirstOrDefaultAsync(b=>b.Id==id);

            if (blog == null) return NotFound();

            return View(blog);
        }

        [HttpPost]
        public async Task<IActionResult>Update(int? id,Blog blog)
        {
            if (id == null) return BadRequest();

            if (id != blog.Id) return BadRequest();

            if (!ModelState.IsValid) return View(blog);

            Blog dbblog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (dbblog == null) return NotFound();

            if(dbblog.MainFile != null)
            {
                if (blog.MainFile == null)
                {
                    ModelState.AddModelError("file", "File is requared");
                    return View(blog);
                }
                if ((blog.MainFile.Length / 1024) > 300)
                {
                    ModelState.AddModelError("File", "File must be 300kb");
                    return View(blog);
                }

                string fileName = DateTime.UtcNow.AddHours(4).ToString("yyyyMMddHHmmssfff") + blog.MainFile.FileName
                    .Substring(blog.MainFile.FileName.LastIndexOf('.'));

                string filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Blog", dbblog.Image);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                filePath = Path.Combine(_env.WebRootPath, "Assets", "Photos", "Blog", fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await blog.MainFile.CopyToAsync(fileStream);
                }
                dbblog.Image = fileName;
            }

            dbblog.Title = blog.Title?.Trim();
            dbblog.news1 = blog.news1?.Trim();
            dbblog.news2 = blog.news2?.Trim();
            dbblog.news3 = blog.news3?.Trim();
            dbblog.news4 = blog.news4?.Trim();
            dbblog.UpdatedBy = "Amrah";
            dbblog.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest("id not found");

            Blog blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (blog == null) return NotFound();

            blog.IsDeleted = true;
            blog.DeletedBy = "Amrah";
            blog.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
