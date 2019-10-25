using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Tayko.co.Models
{
    public class BlogDataManager
    {
        //private readonly IHostingEnvironment _hostingEnvironment;
        private IDirectoryContents DirectoryContents { get; set; }
        public List<Article> Articles { get; set; }

        public BlogDataManager(IWebHostEnvironment hostingEnvironment)
        {
            
            DirectoryContents = hostingEnvironment.ContentRootFileProvider.GetDirectoryContents("/Blog");
            Articles = new List<Article>();

            foreach (var item in DirectoryContents)
            {
                if (!item.IsDirectory)
                {
                    Article article = new Article(
                        Path.GetFileNameWithoutExtension(item.PhysicalPath),
                        item.PhysicalPath
                    );

                    Articles.Add(article);
                }
            }
            Articles.Sort( (x, y) => 
                y.Date.CompareTo(x.Date));
        }
    }
}