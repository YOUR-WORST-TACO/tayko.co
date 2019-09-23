using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

namespace Tayko.co.Models
{
    public class Blogerator
    {
        private DirectoryInfo RootDirectory { get; set; }
        private string ContentFileName = "content.html";
        private string MetaConfigFileName = "meta.conf";

        public List<PostModel> Posts { get; set; }

        public Blogerator(IHostingEnvironment hostingEnvironment)
        {
            RootDirectory = new DirectoryInfo(hostingEnvironment.ContentRootPath + "/Blog");

            Posts = new List<PostModel>();

            /*PostModel test = new PostModel();
            test.PostDate = DateTime.Now;
            test.PostName = "Magic Name";
            test.PostTitle = "Magical thingy";
            test.PostEditDate = DateTime.MinValue;

            string json = JsonConvert.SerializeObject(test);

            PostModel test2 = JsonConvert.DeserializeObject<PostModel>(json);
            
            
            
            Console.Write(json + "\n");*/


            BlogInitializer();
        }

        public PostModel LoadBlogPost(DirectoryInfo postDirectory)
        {
            FileInfo contentFile = null;
            FileInfo metaFile = null;

            foreach (var file in postDirectory.GetFiles())
            {
                if (file.Name.Equals(ContentFileName))
                {
                    contentFile = file;
                }
                else if (file.Name.Equals(MetaConfigFileName))
                {
                    metaFile = file;
                }
            }

            if (contentFile != null && metaFile != null)
            {
                string metaFileContents = File.ReadAllText(metaFile.FullName);

                PostModel temporaryPost = null;

                try
                {
                    temporaryPost = JsonConvert.DeserializeObject<PostModel>(metaFileContents);
                }
                catch (JsonReaderException e)
                {
                    Console.Write($"Meta file for article: {postDirectory.Name} is invalid, skipping...\n {e.Message}");
                    return null;
                }

                temporaryPost.PostMetaFile = metaFile;
                temporaryPost.PostContentFile = contentFile;
                temporaryPost.PostName = postDirectory.Name;
                temporaryPost.PostContent = File.ReadAllText(contentFile.FullName);
                temporaryPost.PostRoot = postDirectory;

                temporaryPost.PostResourceDirectory = null;
                foreach (var directory in postDirectory.GetDirectories())
                {
                    if (directory.Name.Equals("resources"))
                    {
                        temporaryPost.PostResourceDirectory = new DirectoryInfo(directory.FullName);
                    }
                }

                if (PostCacheValid(temporaryPost))
                {
                }
                else
                {
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

            BlogeratorStarted();
        }

        public void BlogeratorStarted()
        {
            Console.WriteLine("Blogerator successfully initialized!");
        }

        public bool PostCacheValid(PostModel post)
        {
            var currentPostHash = post.GetContentMd5Hash();
            var postCacheFile = Path.Combine(RootDirectory.FullName, ".cache", post.PostName + ".cache");
            var postHashFile = Path.Combine(RootDirectory.FullName, ".cache", post.PostName);

            if (File.Exists(postHashFile) && File.Exists(postCacheFile))
            {
                var previousPostHash = File.ReadAllText(postHashFile);
                if (previousPostHash != currentPostHash)
                {
                    return true;
                }
            }

            return false;
        }

        public PostModel GetPost(string postName)
        {
            return null;
        }
    }
}