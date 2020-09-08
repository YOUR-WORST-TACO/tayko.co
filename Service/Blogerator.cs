using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LibGit2Sharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Tayko.co.Models;

namespace Tayko.co.Service
{
    public enum BlogChangeFlag
    {
        Create,
        Modify,
        Delete
    }
    public class Blogerator : IHostedService
    {
        private DirectoryInfo RootDirectory { get; set; }
        private string ContentFileName = "content.md";

        private FileSystemWatcher BlogWatcher { get; set; }
        private FileSystemWatcher BlogContentWatcher { get; set; }
        private Repository BlogRepository { get; set; }
        private string Remote { get; set; }
        
        private Timer _timer;
        private int _updateDelayCount;

        private static int counter;

        private int RootDirectoryDepth { get; set; }

        public List<PostModel> Posts { get; set; }

        public Blogerator(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            counter++;
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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

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
            
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
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
                    var loadedPost = LoadBlogPost(new DirectoryInfo(childDirectory));
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
                    Console.WriteLine($"Deleting(1) Post {post.PostName}");
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
                    var loadedPost = LoadBlogPost(post.PostRoot);

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
                        Console.WriteLine($"Deleting(2) Post {post.PostName}");
                        Posts.Remove(post);
                    }
                }
                else
                {
                    Console.WriteLine($"Deleting(3) Post {post.PostName}");
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

                var master = BlogRepository.Branches["master"];
                PullOptions pullOptions = new PullOptions()
                {
                    MergeOptions = new MergeOptions()
                    {
                        FastForwardStrategy = FastForwardStrategy.Default
                    }
                };

                MergeResult mergeResult = Commands.Pull(
                    BlogRepository,
                    new Signature("my name", "my email", DateTimeOffset.Now), // I dont want to provide these
                    pullOptions
                );
                Console.WriteLine("Updated Blog Repo");
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
                Console.WriteLine("Created Blog Repo");
            }
            catch (LibGit2SharpException)
            {
                Console.WriteLine("Unspecified Error occured, ignoring");
            }
        }
        
        /*
        private void OnBlogChanged(object source, FileSystemEventArgs e)
        {
            if (!e.Name.EndsWith('~') && !e.Name.Contains(".git"))
            {
                //Console.WriteLine($"File in BLOG: {e.FullPath} {e.ChangeType}");
                _lockMutex.WaitOne();
                
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
                _lockMutex.ReleaseMutex();
            }
        }
        
        private void OnContentChanged(object source, FileSystemEventArgs e)
        {
            if (!e.Name.EndsWith('~') && !e.Name.Contains(".git"))
            {
                _lockMutex.WaitOne(4000);
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
                _lockMutex.ReleaseMutex();
            }
        }*/

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

                    temporaryPost.UpdateHash();

                    // todo: remove the PostCover and replace it in the Blog Controller
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
            UpdateBlogRepository();
            
            // todo: Move outside of Blogerator
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