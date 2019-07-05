using Microsoft.AspNetCore.Mvc;

namespace Tayko.co.Controllers
{
    public class ProjectsController : Controller
    {
        // GET
        public IActionResult Projects()
        {
            return View();
        }
    }
}