using Microsoft.AspNetCore.Mvc;

namespace CodeFinallyProjeAntomi.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
