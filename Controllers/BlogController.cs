using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Tayko.co.Service;

namespace Tayko.co.Controllers
{
    public class BlogController : Controller
    {
        // store all private variables for runtime operation
        private readonly IWebHostEnvironment _hostingEnvironment;
        //private readonly CommentDbContext _context;
        private readonly IHttpContextAccessor _accessor;

        private readonly Blogerator _blogerator;

        public BlogController(
            IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor accessor,
            Blogerator blogerator)
        {
            _hostingEnvironment = hostingEnvironment;
            _accessor = accessor;
            _blogerator = blogerator;
        }

        public IActionResult LoadBlog(string article)
        {
            // if no article was passed
            if (article == null)
            {
                // return the BlogOverview view (FOR NOW IT IS SORTED)
                return View("BlogOverview", 
                    _blogerator.Posts.OrderByDescending(item => item.PostDate).ToList());
            }

            // todo: replace this with a convenience function from Blogerator
            // finds first article that matches the lambda
            var foundArticle = _blogerator.Posts.FirstOrDefault(
                x => (x.PostName == article)
            );

            // if article is not found
            if (foundArticle == null)
            {
                // go to error page
                return StatusCode(404);
            }

            foundArticle.PostCover = null;
            
            if (Directory.Exists(foundArticle.PostRoot.FullName + "/resources"))
            {
                foundArticle.PostCover = new DirectoryInfo(foundArticle.PostRoot.FullName + "/resources").GetFiles()
                    .FirstOrDefault(file => Path.GetFileNameWithoutExtension(file.Name) == "cover")
                    ?.Name;
            }

            if (foundArticle.PostCover == null)
            {
                foundArticle.PostCover = "https://source.unsplash.com/1600x900/?nature,mountains";
            }
            else
            {
                foundArticle.PostCover = foundArticle.PostName + "/" + foundArticle.PostCover;
            }
            
            // return Blog view with foundArticle model
            return View("Blog", foundArticle);
        }

        public IActionResult LoadBlogResource(string article, string resource)
        {
            // todo: replace this with a convenience function from Blogerator
            // finds first article that matches the lambda
            var foundArticle = _blogerator.Posts.FirstOrDefault(
                x => (x.PostName == article)
            );

            if (foundArticle == null) return NotFound();
            
            var provider = new FileExtensionContentTypeProvider();
            var resourceDirectory = foundArticle.PostRoot.FullName + "/resources";

            if (!Directory.Exists(resourceDirectory)) return NotFound();

            var resourceItem = foundArticle.PostRoot + "/resources/" + resource;
            if (!System.IO.File.Exists(resourceItem)) return NotFound();
            
            var resourceItemContent = System.IO.File.OpenRead(resourceItem);

            if (!provider.TryGetContentType(resourceItem, out var resourceItemType)) 
                resourceItemType = "application/octet-stream";
            
            return File(resourceItemContent, resourceItemType);
        }
    }
}