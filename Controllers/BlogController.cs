using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Tayko.co.Controllers
{
    public class BlogController : Controller
    {
        // GET
        public IActionResult Article(string article)
        {
            if (article != null)
            {
                
                return View($"Articles/{article}");
            }
            
            return View("Articles");
        }
    }
}