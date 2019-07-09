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
            _fileProvider = fileProvider;
            DirectoryContents = _fileProvider.GetDirectoryContents("/Blog");
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
        }
    }
}