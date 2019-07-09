using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Tayko.co.Models
{
    public class Article
    {
        public string Name { get; private set; }
        public string Title { get; private set; }
        public DateTime Date { get; private set; }
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
                var regexDate = new Regex(
                    @"<label class=""blog date"">(.*?)<\/label>");
                var regexDescription = new Regex(
                    @"<p class=""blog intro"">(.*?)<\/p>", RegexOptions.Singleline);
                
                var titleMatch = regexTitle.Match(Contents);
                Title = titleMatch.Groups[1].Value;

                try
                {
                    var dateMatch = regexDate.Match(Contents);
                    Date = DateTime.Parse(dateMatch.Groups[1].Value);
                }
                catch (FormatException)
                {
                    Date = DateTime.MinValue;
                }
                

                var descriptionMatch = regexDescription.Match(Contents);
                Description = descriptionMatch.Groups[1].Value;
            }
            catch (FileLoadException e)
            {
                Title = "ARTICLE";
                Contents = "FAILED TO LOAD FILE";
            }
        }
    }
}