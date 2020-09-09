using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LibGit2Sharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Tayko.co.Models;

namespace Tayko.co.Service
{
    public class Blogerator : IHostedService
    {
        private DirectoryInfo RootDirectory { get; set; }
        private string ContentFileName = "content.md";
        private Repository BlogRepository { get; set; }
        private string Remote { get; set; }
        
        private Timer _timer;
        private int _updateDelayCount;
        private int RootDirectoryDepth { get; set; }

        public List<PostModel> Posts { get; set; }

        public Blogerator(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            RootDirectory = new DirectoryInfo(hostingEnvironment.ContentRootPath + "/Blog");
            RootDirectoryDepth = RootDirectory.FullName.Split(Path.DirectorySeparatorChar).Length - 1;
            
            Posts = new List<PostModel>();
            _updateDelayCount = 0;

            Remote = configuration["BlogRepository"];
            
            BlogInitializer();
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                BlogeratorThread,
                null,
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(10)
            );
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void BlogeratorThread(object state)
        {
            if (_updateDelayCount < 5)
            {
                _updateDelayCount++;
            }
            else
            {
                UpdateBlogRepository();
                _updateDelayCount = 0;
            }
            
            ComputeChanges();
        }
        private void ComputeChanges()
        {
            var tempPosts = new List<PostModel>(Posts);
            string[] tempChildDirectories = Directory.GetDirectories(RootDirectory.FullName);

            // detect new folders
            foreach (var childDirectory in tempChildDirectories)
            {
                string childName = Path.GetFileName(childDirectory);

                if (tempPosts.All(post => post.PostRoot.FullName != childDirectory) && childName != ".git")
                {
                    var loadedPost = LoadBlogPost(new DirectoryInfo(childDirectory), false);
                    if (loadedPost != null)
                    {
                        Console.WriteLine($"Loading Post {loadedPost.PostName}");
                        Posts.Add(loadedPost);
                    }
                }
            }

            // detect missing folders
            foreach (var post in tempPosts)
            {
                if (!Directory.Exists(post.PostRoot.FullName))
                {
                    Console.WriteLine($"Deleting (v1) Post {post.PostName}");
                    Posts.Remove(post);
                }
            }
            
            // update things that were changed by previous checks
            tempPosts = new List<PostModel>(Posts);
            
            // compute file changes
            foreach (var post in tempPosts)
            {
                if (File.Exists(post.PostStorageFile.FullName))
                {
                    if (!post.UpdateHash()) continue;
                    var loadedPost = LoadBlogPost(post.PostRoot, false);

                    if (loadedPost != null)
                    {
                        Console.WriteLine($"Updating Post {post.PostName}");
                        post.PostContent = loadedPost.PostContent;
                        post.PostAuthor = loadedPost.PostAuthor;
                        post.PostDate = loadedPost.PostDate;
                        post.PostTitle = loadedPost.PostTitle;
                        post.PostDescription = loadedPost.PostDescription;
                    }
                    else
                    {
                        Console.WriteLine($"Deleting (v2) Post {post.PostName}");
                        Posts.Remove(post);
                    }
                }
                else
                {
                    Console.WriteLine($"Deleting (v3) Post {post.PostName}");
                    Posts.Remove(post);
                }
            }
        }

        public void UpdateBlogRepository()
        {
            try
            {
                BlogRepository = new Repository(RootDirectory.FullName);

                Commands.Fetch(BlogRepository, "origin", new string[0], new FetchOptions(), null);

                PullOptions pullOptions = new PullOptions()
                {
                    MergeOptions = new MergeOptions()
                    {
                        FastForwardStrategy = FastForwardStrategy.Default
                    }
                };

                // todo: track the merger result, or get rid of it
                // MergeResult mergeResult = Commands.Pull(
                Commands.Pull(
                    BlogRepository,
                    new Signature("my name", "my email", DateTimeOffset.Now), // I dont want to provide these
                    pullOptions
                );
            }
            catch (RepositoryNotFoundException)
            {
                if (Directory.Exists(RootDirectory.FullName))
                {
                    for (int numTries = 0; numTries < 10; numTries++)
                    {
                        try
                        {
                            Directory.Delete(RootDirectory.FullName, true);
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(50);
                        }
                    }
                }

                Repository.Clone(Remote, RootDirectory.FullName);
                BlogRepository = new Repository(RootDirectory.FullName);
                // Console.WriteLine("Created Blog Repo");
            }
            catch (LibGit2SharpException)
            {
                Console.WriteLine("Unspecified Error occured, ignoring");
            }
        }

        public PostModel LoadBlogPost(DirectoryInfo postDirectory, bool verbose = true)
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
                    if (verbose) Console.Write($"Unable to access content file: {postDirectory.Name}\n");
                    return null;
                }

                var storageFileSplit = new Regex(storageRegEx, RegexOptions.Singleline)
                    .Match(fileContents).Groups;

                if (storageFileSplit.Count == 6 && storageFileSplit["content"].Value != "")
                {

                    var markdown = new MarkdownSharp.Markdown();

                    var temporaryPost = new PostModel
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
                    };

                    temporaryPost.UpdateHash();

                    if (verbose) Console.Write($"Successfully Loaded Article: {postDirectory.Name}\n");
                    return temporaryPost;
                }
            }

            if (verbose) Console.Write($"Malformed Article Skipped: {postDirectory.Name}\n");
            
            return null;
        }

        private void BlogInitializer()
        {
            UpdateBlogRepository();
            
            foreach (var subDirectory in RootDirectory.GetDirectories())
            {
                if (subDirectory.Name == ".git")
                {
                    continue;
                }
                
                var loadedPost = LoadBlogPost(subDirectory);
                if (loadedPost != null)
                {
                    Posts.Add(loadedPost);
                }
            }
            
            BlogeratorStarted();
        }

        public void BlogeratorStarted()
        {
            Console.WriteLine("Blogerator successfully initialized!");
        }
    }
}