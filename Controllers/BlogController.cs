using System.IO;
using System.Linq;
using Tayko.co.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.FileProviders;

namespace Tayko.co.Controllers
{
    public class BlogController : Controller
    {
        // GET
        public IActionResult LoadBlog(string article)
        {
            var provider = new PhysicalFileProvider(ApplicationEnvironment.ApplicationBasePath);

            BlogModel blogs = new BlogModel(provider);

            if (article != null)
            {
                var foundArticle = blogs.Articles.FirstOrDefault(
                    x => (Path.GetFileNameWithoutExtension(x.FilePath) == article)
                );

                if (foundArticle == null)
                {
                    return NotFound();
                }

                return View("Blog", foundArticle);
            }

            return View("Articles", blogs);
        }
    }
}