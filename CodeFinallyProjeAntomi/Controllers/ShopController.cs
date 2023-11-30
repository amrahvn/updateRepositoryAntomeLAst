using Microsoft.AspNetCore.Mvc;

namespace CodeFinallyProjeAntomi.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
