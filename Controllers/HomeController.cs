using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
            bool showMyFace = mode == "lyze";
            return View(showMyFace);
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