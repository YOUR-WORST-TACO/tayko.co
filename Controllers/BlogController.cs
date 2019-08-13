using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Tayko.co.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.FileProviders;
//using Tayko.co.Data;

namespace Tayko.co.Controllers
{
    public class BlogController : Controller
    {
        // store all private variables for runtime operation
        private readonly IHostingEnvironment _hostingEnvironment;
        //private readonly CommentDbContext _context;
        private readonly IHttpContextAccessor _accessor;

        public BlogController(
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor accessor)
        {
            _hostingEnvironment = hostingEnvironment;
            //_context = context;
            _accessor = accessor;
        }
        
        public IActionResult LoadBlog(string article)
        {
            // get default root folder for hossting
            var provider = _hostingEnvironment.ContentRootFileProvider;

            // instantiate new BlogDataManager to load blogs
            BlogDataManager blogs = new BlogDataManager(provider);

            // if no article was passed
            if (article == null)
            {
                // return the BlogOverview view
                return View("BlogOverview", blogs);
            }
            
            // finds first article that matches the lambda
            var foundArticle = blogs.Articles.FirstOrDefault(
                x => (x.Name == article)
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
    }
}