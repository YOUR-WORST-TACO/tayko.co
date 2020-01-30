using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Tayko.co.Models;
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
            // get default root folder for hosting

            // instantiate new BlogDataManager to load blogs

            // if no article was passed
            if (article == null)
            {
                // return the BlogOverview view (FOR NOW IT IS SORTED)
                return View("BlogOverview", 
                    _blogerator.Posts.OrderByDescending(item => item.PostDate).ToList());
            }

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

            // return Blog view with foundArticle model
            return View("Blog", foundArticle);
        }

        public IActionResult LoadBlogResource(string article, string resource)
        {
            // finds first article that matches the lambda
            var foundArticle = _blogerator.Posts.FirstOrDefault(
                x => (x.PostName == article)
            );

            if (foundArticle?.PostResourceDirectory != null)
            {
                var provider = new FileExtensionContentTypeProvider();

                var resourceItem = foundArticle.PostResourceDirectory.FullName + "/" + resource;
                var resourceItemContent = System.IO.File.OpenRead(resourceItem);

                if (!provider.TryGetContentType(resourceItem, out var resourceItemType))
                {
                    resourceItemType = "application/octet-stream";
                }

                return File(resourceItemContent, resourceItemType);
            }

            return NotFound();
        }
    }
}