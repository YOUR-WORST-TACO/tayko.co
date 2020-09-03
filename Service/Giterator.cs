using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LibGit2Sharp;
using Microsoft.Extensions.Hosting;

namespace Tayko.co.Service
{
    public class Giterator : IHostedService
    {
        private Timer _timer;

        private Repository BlogRepository { get; set; }
        private DirectoryInfo RootDirectory { get; set; }

        public bool Initialized;

        public Giterator(IHostEnvironment hostingEnvironment)
        {
            RootDirectory = new DirectoryInfo(hostingEnvironment.ContentRootPath + "/Blog");
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting Giterator");
            _timer = new Timer(
                UpdateBlogRepository,
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(1)
            );
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void UpdateBlogRepository(object state)
        {
            Console.WriteLine("Updated Blog Repo");
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
                Initialized = true;
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

                Repository.Clone(@"https://github.com/YOUR-WORST-TACO/tayko.co-blog.git", RootDirectory.FullName);
                BlogRepository = new Repository(RootDirectory.FullName);
                Initialized = true;
            }
            catch (LibGit2SharpException)
            {
                Console.WriteLine("Unspecified Error occured, ignoring");
            }
        }
    }
}