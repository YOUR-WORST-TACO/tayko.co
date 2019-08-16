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

        private readonly BlogDataManager _dataManager;

        public BlogController(
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor accessor,
            BlogDataManager dataManager)
        {
            _hostingEnvironment = hostingEnvironment;
            //_context = context;
            _accessor = accessor;
            _dataManager = dataManager;
        }

        [Route("/Blog/Image")]
        public IActionResult Image()
        {
            var file = _hostingEnvironment.ContentRootFileProvider.GetFileInfo( "/Blog/hollowknight.jpg");
            return File(file.CreateReadStream(), "image/jpeg");
        }
        
        public IActionResult LoadBlog(string article)
        {
            // get default root folder for hossting
            var provider = _hostingEnvironment.ContentRootFileProvider;

            // instantiate new BlogDataManager to load blogs

            // if no article was passed
            if (article == null)
            {
                // return the BlogOverview view
                return View("BlogOverview", _dataManager);
            }

            // finds first article that matches the lambda
            var foundArticle = _dataManager.Articles.FirstOrDefault(
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