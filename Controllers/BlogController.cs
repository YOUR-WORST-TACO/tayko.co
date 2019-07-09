using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Tayko.co.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.FileProviders;

namespace Tayko.co.Controllers
{
    public class BlogController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public BlogController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        // GET
        public IActionResult LoadBlog(string article)
        {
            var provider = _hostingEnvironment.ContentRootFileProvider;

            BlogModel blogs = new BlogModel(provider);

            if (article != null)
            {
                var foundArticle = blogs.Articles.FirstOrDefault(
                    x => (x.Name == article)
                );

                if (foundArticle == null)
                {
                    return NotFound();
                }

                return View("Blog", foundArticle);
            }

            return View("BlogOverview", blogs);
        }
    }
}