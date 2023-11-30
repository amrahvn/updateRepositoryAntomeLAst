using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CodeFinallyProjeAntomi.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin")]
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;

        public SettingController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Setting> settings =await _context.Settings.Where(s=>s.IsDeleted==false).ToListAsync();

            return View(settings);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Setting setting)
        {
            if(setting.Key == null)
            {
                ModelState.AddModelError("Key", "Key is important");
                return View(setting);
            }
            if (setting.Value == null)
            {
                ModelState.AddModelError("Value", "Value is important");
                return View(setting);
            }

            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Setting setting=await _context.Settings.FirstOrDefaultAsync(s=>s.Id == id);

            if (setting == null) return BadRequest();

            return View(setting);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id,Setting setting)
        {
            if (id == null) return BadRequest();

            if (id != setting.Id) return BadRequest();

            Setting dbSetting = await _context.Settings
                .FirstOrDefaultAsync(s=>s.Id == id);

            if (dbSetting == null) return NotFound();

            dbSetting.Key = setting.Key;
            dbSetting.Value = setting.Value;
           

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Setting setting = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);

            if (setting == null) return BadRequest();

            setting.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
