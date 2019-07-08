using Microsoft.AspNetCore.Mvc;

namespace Tayko.co.Controllers
{
    public class ErrorController : Controller
    {
        // GET
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}