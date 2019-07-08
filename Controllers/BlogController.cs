using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Tayko.co.Controllers
{
    public class BlogController : Controller
    {
        // GET
        public IActionResult Article(string article)
        {
            
            if (article != null)
            {
                return View($"Articles/{article}");
            }

            string[,] articles =
            {
                {
                    "articles-how-to",
                    "Articles How To",
                    "How to make articles, an insightful look at article creation"
                },
                {
                    "how-to-article",
                    "How to Article",
                    "Another take on making articles, not as insightful, but could incite rage and anger"
                },
                {
                    "my-test-article",
                    "My Test Article",
                    "A horrid example of an article, do not look at this"
                }
            };

            ViewBag.ArticleBag = articles;
            
            return View("Articles");
        }
    }
}