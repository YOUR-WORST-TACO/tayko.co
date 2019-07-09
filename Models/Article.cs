using System.IO;
using System.Text.RegularExpressions;

namespace Tayko.co.Models
{
    public class Article
    {
        public string Name { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string FilePath { get; private set; }
        public string Contents { get; private set; }

        public Article(string name, string filePath)
        {
            Name = name;
            FilePath = filePath;

            try
            {
                Contents = File.ReadAllText(FilePath);

                var regexTitle = new Regex(
                    @"<h1 class=""blog title"">(.*?)<\/h1>");
                var regexDescription = new Regex(
                    @"<p class=""blog intro"">(.*?)<\/p>", RegexOptions.Singleline);

                var match = regexTitle.Match(Contents);
                Title = match.Groups[1].Value;

                var match2 = regexDescription.Match(Contents);
                Description = match2.Groups[1].Value;
            }
            catch (FileLoadException e)
            {
                Title = "ARTICLE";
                Contents = "FAILED TO LOAD FILE";
            }
        }
    }
}