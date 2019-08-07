using System;
using System.IO;
using System.Linq;
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
            var provider = _hostingEnvironment.ContentRootFileProvider;

            BlogDataManager blogs = new BlogDataManager(provider);

            if (article == null)
            {
                return View("BlogOverview", blogs);
            }
            
            var foundArticle = blogs.Articles.FirstOrDefault(
                x => (x.Name == article)
            );

            if (foundArticle == null)
            {
                return StatusCode(404);
            }

            foundArticle.Comments = _context.Comments
                .Where(b => b.Article.Equals(foundArticle.Name))
                .ToList();

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

            TryValidateModel(newComment);

            if (ModelState.IsValid)
            {
                _context.Add(newComment);
                _context.SaveChanges();
            }

            //ViewData[""];

            return LocalRedirect($"/Blog/{newComment.Article}");
        }
    }
}