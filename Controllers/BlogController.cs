using System.IO;
using Tayko.co.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.FileProviders;

namespace Tayko.co.Controllers
{
    public class BlogController : Controller
    {
        // GET
        public IActionResult Article(string article)
        {
            if (article != null)
            {
                ViewData["article-url"] = article;

                return View("Blog");
            }

            var provider = new PhysicalFileProvider(ApplicationEnvironment.ApplicationBasePath);

            BlogModel blogs = new BlogModel(provider);

            return View("Articles", blogs);
        }
    }
}