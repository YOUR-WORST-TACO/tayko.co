using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Tayko.co.Models;

namespace Tayko.co.Service
{
    public class Blogerator
    {
        private DirectoryInfo RootDirectory { get; set; }
        private string ContentFileName = "content.md";

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

            foreach (var file in postDirectory.GetFiles())
            {
                if (file.Name.Equals(ContentFileName))
                {
                    contentFile = file;
                }
            }

            if (contentFile != null)
            {
                string storageRegEx = @"---.*title:(?<title>.*?)\r\nauthor:(?<author>.*?)\r\npostDate:(?<postDate>.*?)\r\neditDate:(?<editDate>.*?)\r\n---\r\n(?<content>.*)";
                
                var storageFileSplit = new Regex(storageRegEx, RegexOptions.Singleline )
                    .Match(File.ReadAllText(contentFile.FullName)).Groups;

                PostModel temporaryPost = new PostModel
                {
                    PostStorageFile = contentFile,
                    PostContent = storageFileSplit["content"].Value,
                    PostTitle = storageFileSplit["title"].Value,
                    PostAuthor = storageFileSplit["author"].Value,
                    PostName = postDirectory.Name,
                    PostRoot = postDirectory,
                    PostResourceDirectory = null
                };

                foreach (var directory in postDirectory.GetDirectories())
                {
                    if (directory.Name.Equals("resources"))
                    {
                        temporaryPost.PostResourceDirectory = new DirectoryInfo(directory.FullName);
                    }
                }

                temporaryPost.LoadCache(RootDirectory);

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

        public PostModel GetPost(string postName)
        {
            return null;
        }
    }
}