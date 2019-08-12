using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Tayko.co.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.FileProviders;
using Tayko.co.Data;

namespace Tayko.co.Controllers
{
    public class BlogController : Controller
    {
        // store all private variables for runtime operation
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly CommentDbContext _context;
        private readonly IHttpContextAccessor _accessor;

        public BlogController(
            IHostingEnvironment hostingEnvironment,
            CommentDbContext context,
            IHttpContextAccessor accessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
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

            // attempt to find comments related to article
            foundArticle.Comments = _context.Comments
                .Where(b => b.Article.Equals(foundArticle.Name))
                .ToList();

            // return Blog view with foundArticle model
            return View("Blog", foundArticle);
        }
        
        [HttpPost, ValidateAntiForgeryToken]
        [Route("Blog/Comment")]
        public IActionResult PostComment(Article model)
        {
            CommentModel newComment = new CommentModel
            {
                Article = model.CommentModel.Article,
                Text = model.CommentModel.Text,
                Author = model.CommentModel.Author,
                AuthorEmail = model.CommentModel.AuthorEmail,
                PostDate = DateTime.Now,
                PostIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString()
            };

            // attempt to validate comment specifiers
            TryValidateModel(newComment);

            // if valid model
            if (ModelState.IsValid)
            {
                // add to dbContext and save
                _context.Add(newComment);
                _context.SaveChanges();
            }

            // ViewData[""];

            // send user back to where they came from
            return Redirect(Request.Headers["Referer"]);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize]
        [Route("Blog/Delete")]
        public IActionResult Delete(Article model)
        {
            var comment = model.CommentModel;

            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }
            
            return Redirect(Request.Headers["Referer"]);
        }
    }
}