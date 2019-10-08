using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tayko.co.Models;

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

            Response.Cookies.Append("LyzeCookie", "1", option);
            
            return View("Index");
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