using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tayko.co.Controllers
{
    public class HomeController : Controller
    {
        // all methods in this class just return default matching views.
        public IActionResult Index(string mode)
        {
            return View();
        }

        public IActionResult Lyze()
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddYears(1);
            option.IsEssential = true;

            Response.Cookies.Append("LyzeCookie", "1", option);
            
            return RedirectToAction("Index");
        }

        public IActionResult In()
        {
            return Redirect("https://www.linkedin.com/in/stephen-tafoya-045a9aa0/");
        }
        
        public IActionResult Code()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Projects(string tail)
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}