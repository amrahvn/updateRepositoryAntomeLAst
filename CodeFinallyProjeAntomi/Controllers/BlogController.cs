using CodeFinallyProjeAntomi.DataAccesLayer;
using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModel.CommentBLogVM;
using CodeFinallyProjeAntomi.ViewModel.CommentVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeFinallyProjeAntomi.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            IQueryable<Blog> blogs = _context.Blogs.Where(b => b.IsDeleted == false);

            var viewModel = new CommentBLogVM
            {
                Blogs = blogs.ToList(),
                Comments = _context.Comments.Include(a=>a.AppUser).Select(c => new CommentVM
                {
                    Id = c.Id,
                    CommentTime = c.CreatedAt,
                    CommentText = c.CommentText
                }).ToList()
            };

            return View("Index", viewModel);
        }





        [HttpGet]
        public IActionResult AddComment()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            //if (!ModelState.IsValid)
            //{ 
            //    return View("AddComment", comment);
            //}

            comment.DeletedAt = DateTime.Now;
            comment.CommentText = comment.CommentText;

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
