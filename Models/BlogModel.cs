using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Tayko.co.Models
{
    public class BlogModel
    {
        private readonly IFileProvider _fileProvider;
        public IDirectoryContents DirectoryContents { get; set; }
        public List<Article> Articles { get; set; }

        public BlogModel(IFileProvider fileProvider)
        {
            Console.Write("\n\n\n\n Running through BlogModel \n\n\n\n\n");
            _fileProvider = fileProvider;
            DirectoryContents = _fileProvider.GetDirectoryContents("/Blog");
            Articles = new List<Article>();

            foreach (var item in DirectoryContents)
            {
                if (!item.IsDirectory)
                {
                    Console.WriteLine(item.PhysicalPath);
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