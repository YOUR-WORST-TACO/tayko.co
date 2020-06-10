using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Tayko.co.Models;

namespace Tayko.co.Service
{
    public class Blogerator
    {
        private DirectoryInfo RootDirectory { get; set; }
        private string ContentFileName = "content.md";
        private FileSystemWatcher BlogWatcher { get; set; }

        private int RootDirectoryDepth { get; set; }

        public List<PostModel> Posts { get; set; }

        public Blogerator(IWebHostEnvironment hostingEnvironment)
        {
            RootDirectory = new DirectoryInfo(hostingEnvironment.ContentRootPath + "/Blog");
            RootDirectoryDepth = RootDirectory.FullName.Split(Path.DirectorySeparatorChar).Length - 1;
            Posts = new List<PostModel>();

            BlogInitializer();
        }

        private void InitializeWatcher()
        {
            BlogWatcher = new FileSystemWatcher
            {
                Path = RootDirectory.FullName,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite
                               | NotifyFilters.FileName
                               | NotifyFilters.DirectoryName
            };

            BlogWatcher.Changed += OnChanged;
            BlogWatcher.Created += OnChanged;
            BlogWatcher.Deleted += OnChanged;
            BlogWatcher.Renamed += OnRenamed;

            BlogWatcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (!e.Name.EndsWith('~'))
            {
                Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
                
                string[] changedDirectories = e.FullPath.Split(Path.DirectorySeparatorChar);
                if (changedDirectories.Length > RootDirectoryDepth)
                {
                    Console.WriteLine($"changed detected in article: {changedDirectories[RootDirectoryDepth+1]}");
                }

            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine($"File: {e.OldFullPath} changed to {e.FullPath}");
        }

        public PostModel LoadBlogPost(DirectoryInfo postDirectory)
        {
            FileInfo contentFile = null;

            foreach (var file in postDirectory.GetFiles())
            {
                if (file.Name.Equals(ContentFileName))
                {
                    contentFile = file;
                }
            }

            if (contentFile != null)
            {
                string storageRegEx =
                    @"---.*title:(?<title>.*?)\s+author:(?<author>.*?)\s+postDate:(?<postDate>.*?)\s+description:(?<description>.*?)\s+---\s+(?<content>.*)";

                var storageFileSplit = new Regex(storageRegEx, RegexOptions.Singleline)
                    .Match(File.ReadAllText(contentFile.FullName)).Groups;

                var markdown = new MarkdownSharp.Markdown();

                PostModel temporaryPost = new PostModel
                {
                    PostStorageFile = contentFile,
                    PostContent = markdown.Transform(storageFileSplit["content"].Value),
                    PostTitle = storageFileSplit["title"].Value,
                    PostAuthor = storageFileSplit["author"].Value,
                    PostDescription = storageFileSplit["description"].Value,
                    PostName = postDirectory.Name,
                    PostRoot = postDirectory,
                    PostDate = DateTime.ParseExact(storageFileSplit["postDate"].Value, "yyyyMMdd",
                        System.Globalization.CultureInfo.InvariantCulture),
                    PostResourceDirectory = null
                };

                foreach (var directory in postDirectory.GetDirectories())
                {
                    if (directory.Name.Equals("resources"))
                    {
                        temporaryPost.PostResourceDirectory = new DirectoryInfo(directory.FullName);

                        temporaryPost.PostCover = temporaryPost.PostResourceDirectory.GetFiles()
                            .FirstOrDefault(file => Path.GetFileNameWithoutExtension(file.Name) == "cover");
                    }
                }

                Console.Write($"Successfully Loaded Article: {postDirectory.Name}\n");
                return temporaryPost;
            }
            else
            {
                Console.Write($"Article: {postDirectory.Name} is malformed, skipping!\n");
            }

            return null;
        }

        private void BlogInitializer()
        {
            if (!Directory.Exists(RootDirectory.FullName + "/.cache"))
            {
                Directory.CreateDirectory(RootDirectory.FullName + "/.cache");
            }

            foreach (var subDirectory in RootDirectory.GetDirectories())
            {
                if (subDirectory.Name == ".cache")
                {
                    continue;
                }

                var articleDirectory = new DirectoryInfo(subDirectory.FullName);

                var loadedPost = LoadBlogPost(subDirectory);
                if (loadedPost != null)
                {
                    Posts.Add(loadedPost);
                }
            }

            InitializeWatcher();
            BlogeratorStarted();
        }

        public void BlogeratorStarted()
        {
            Console.WriteLine("Blogerator successfully initialized!");
        }

        public PostModel GetPost(string postName)
        {
            return null;
        }
    }
}