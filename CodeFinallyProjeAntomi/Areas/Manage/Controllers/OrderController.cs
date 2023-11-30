using CodeFinallyProjeAntomi.Areas.Manage.ViewModels.OrderVms;
using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Enums;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeFinallyProjeAntomi.Areas.Manage.Controllers
{
    //[Authorize(Roles =("Admin,SuperAdmin"))]
    [Area("Manage")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int currentPage=1)
        {
            IQueryable<Order> queries = _context.Orders
                .Include(o=>o.OrderProducts.Where(op=>op.IsDeleted==false))
                .Where(o=>o.IsDeleted==false);

            return View(PageNatedList<Order>.Create(queries,currentPage,4,6));
        }

        public IActionResult Detail(int? id)
        {
            if(id==null) { return BadRequest(); }

            Order order = _context.Orders
                .Include(o=>o.User)
                .Include(o=>o.OrderProducts.Where(op=>op.IsDeleted==false)).ThenInclude(op=>op.Product)
                .FirstOrDefault(o=>o.IsDeleted==false && o.Id==id);

            if(order==null) return NotFound();  

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Update(OrderVm orderVm)
        {
            Order orderDb = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderProducts.Where(op => op.IsDeleted == false)).ThenInclude(op => op.Product)
                .FirstOrDefault(o => o.IsDeleted == false && o.Id == orderVm.Id);

            if (orderDb == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View("Detail", orderDb);
            }

            orderDb.Comment = orderVm.Comment;
            orderDb.Status = orderVm.Status;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
