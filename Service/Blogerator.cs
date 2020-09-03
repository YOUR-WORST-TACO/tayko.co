using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using LibGit2Sharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Tayko.co.Models;

namespace Tayko.co.Service
{
    public class Blogerator
    {
        private DirectoryInfo RootDirectory { get; set; }
        private string ContentFileName = "content.md";

        private FileSystemWatcher BlogWatcher { get; set; }
        private FileSystemWatcher BlogContentWatcher { get; set; }

        private Giterator _giterator;

        private int RootDirectoryDepth { get; set; }

        public List<PostModel> Posts { get; set; }

        private Dictionary<string, DateTime> PostChangeTracker;

        public Blogerator(IWebHostEnvironment hostingEnvironment, Giterator giterator)
        {
            _giterator = giterator;
            
            RootDirectory = new DirectoryInfo(hostingEnvironment.ContentRootPath + "/Blog");
            RootDirectoryDepth = RootDirectory.FullName.Split(Path.DirectorySeparatorChar).Length - 1;
            
            Posts = new List<PostModel>();
            PostChangeTracker = new Dictionary<string, DateTime>();

            BlogInitializer();
        }

        private void InitializeWatchers()
        {
            BlogWatcher = new FileSystemWatcher
            {
                Path = RootDirectory.FullName,
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.DirectoryName
            };
            
            BlogWatcher.Deleted += OnBlogChanged;
            BlogWatcher.Renamed += OnBlogRenamed;

            BlogWatcher.EnableRaisingEvents = true;

            BlogContentWatcher = new FileSystemWatcher
            {
                Path = RootDirectory.FullName,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite
                               | NotifyFilters.FileName
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.CreationTime
            };

            BlogContentWatcher.Filter = "content.md";

            BlogContentWatcher.Changed += OnContentChanged;
            BlogContentWatcher.Created += OnContentChanged;

            BlogContentWatcher.EnableRaisingEvents = true;
        }

        private void OnBlogChanged(object source, FileSystemEventArgs e)
        {
            if (!e.Name.EndsWith('~') && !e.Name.Contains(".git"))
            {
                //Console.WriteLine($"File in BLOG: {e.FullPath} {e.ChangeType}");

                string[] changedDirectories = e.FullPath.Split(Path.DirectorySeparatorChar);
                if (changedDirectories.Length > RootDirectoryDepth)
                {
                    if (e.ChangeType == WatcherChangeTypes.Deleted)
                    {
                        Posts.RemoveAll(post => post.PostName == changedDirectories[RootDirectoryDepth + 1]);
                        Console.WriteLine($"Article: {changedDirectories[RootDirectoryDepth + 1]} has been deleted.");
                    }
                    //Console.WriteLine($"changed detected in article: {changedDirectories[RootDirectoryDepth+1]}");
                }

            }
        }

        private void OnBlogRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine($"BLOG: {e.OldFullPath} changed to {e.FullPath}");
        }
        
        private void OnContentChanged(object source, FileSystemEventArgs e)
        {
            if (!e.Name.EndsWith('~') && !e.Name.Contains(".git"))
            {
                DateTime lastStoredChange;
                var lastChange = File.GetLastWriteTime(e.FullPath);
                lastChange = new DateTime(
                    lastChange.Ticks - (lastChange.Ticks % TimeSpan.TicksPerSecond),
                    lastChange.Kind);
                
                if (!PostChangeTracker.TryGetValue(e.FullPath, out lastStoredChange))
                {
                    PostChangeTracker[e.FullPath] = lastChange;
                }

                if (!lastStoredChange.Equals(lastChange))
                {
                    //Console.WriteLine($"File Change {totalChanges++}: {e.FullPath} {e.ChangeType} \n\t Last Change {lastChange.Ticks} \n\t Last Stored Change  {lastStoredChange.Ticks}");
                    string[] changedDirectories = e.FullPath.Split(Path.DirectorySeparatorChar);
                    if (changedDirectories.Length > RootDirectoryDepth)
                    {
                        var currentPost = Posts.FirstOrDefault(post =>
                            post.PostName == changedDirectories[RootDirectoryDepth + 1]);

                        if (currentPost != null)
                        {
                            var loadedPost = LoadBlogPost(currentPost.PostRoot);

                            if (loadedPost != null)
                            {
                                currentPost.PostContent = loadedPost.PostContent;
                                currentPost.PostAuthor = loadedPost.PostAuthor;
                                currentPost.PostDate = loadedPost.PostDate;
                                currentPost.PostTitle = loadedPost.PostTitle;
                                currentPost.PostDescription = loadedPost.PostDescription;
                            }
                            else
                            {
                                Posts.RemoveAll(post => post.PostName == changedDirectories[RootDirectoryDepth + 1]);
                            }
                        }
                        else
                        {
                            var postDirectory = new DirectoryInfo(Path.GetDirectoryName(e.FullPath) ?? "");
                            var loadedPost = LoadBlogPost(postDirectory);
                            if (loadedPost != null)
                            {
                                Posts.Add(loadedPost);
                            }
                        }
                    }

                    PostChangeTracker[e.FullPath] = lastChange;
                }
            }
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

                string fileContents = null;

                for (int numTries = 0; numTries < 10; numTries++)
                {
                    try
                    {
                        fileContents = File.ReadAllText(contentFile.FullName);
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(50);
                    }
                }

                if (fileContents == null)
                {
                    Console.Write($"Unable to access content file: {postDirectory.Name}\n");
                    return null;
                }

                var storageFileSplit = new Regex(storageRegEx, RegexOptions.Singleline)
                    .Match(fileContents).Groups;

                if (storageFileSplit.Count == 6 && storageFileSplit["content"].Value != "")
                {

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
            }

            Console.Write($"Malformed Article Skipped: {postDirectory.Name}\n");
            
            return null;
        }

        private void BlogInitializer()
        {
            /*if (!Directory.Exists(RootDirectory.FullName + "/.cache"))
            {
                Directory.CreateDirectory(RootDirectory.FullName + "/.cache");
            }*/
            _giterator.UpdateBlogRepository(null);

            foreach (var subDirectory in RootDirectory.GetDirectories())
            {
                if (subDirectory.Name == ".git")
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

            InitializeWatchers();
            BlogeratorStarted();
        }

        public void GitInitializer()
        {
            try
            {
                Repository blogRepository = new Repository(RootDirectory.FullName);
                
                Commands.Fetch(blogRepository, "origin", new string[0], new FetchOptions(),null);

                var master = blogRepository.Branches["master"];
                PullOptions pullOptions = new PullOptions()
                {
                    MergeOptions = new MergeOptions()
                    {
                        FastForwardStrategy = FastForwardStrategy.Default
                    }
                };
                
                MergeResult mergeResult = Commands.Pull(
                    blogRepository,
                    new Signature("my name", "my email", DateTimeOffset.Now), // I dont want to provide these
                    pullOptions
                );
            }
            catch (RepositoryNotFoundException)
            {
                Repository.Clone(@"https://github.com/YOUR-WORST-TACO/tayko.co-blog.git", RootDirectory.FullName);
                //blogRepository = new Repository(RootDirectory.FullName);
            }
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