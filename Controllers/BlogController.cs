using Microsoft.AspNetCore.Mvc;

namespace Tayko.co.Controllers
{
    public class BlogController : Controller
    {
        // GET
        public IActionResult Blogs()
        {
            return View();
        }
    }
}