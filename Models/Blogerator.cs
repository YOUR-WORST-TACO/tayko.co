using System;
using System.Collections.Generic;
using System.IO;
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

        public void BlogeratorStarted()
        {
            Console.WriteLine("Blogerator successfully initialized!");
        }
        
        private void BlogInitializer()
        {
            foreach (var subDirectory in RootDirectory.GetDirectories())
            {
                var articleDirectory = new DirectoryInfo(subDirectory.FullName);

                FileInfo contentFile = null;
                FileInfo metaFile = null;

                foreach (var file in articleDirectory.GetFiles())
                {
                    if (file.Name.Equals(ContentFileName))
                    {
                        contentFile = file;
                    } else if (file.Name.Equals(MetaConfigFileName))
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
                        Console.Write($"Meta file for article: {articleDirectory.Name} is invalid, skipping...\n {e.Message}");
                        continue;
                    }

                    temporaryPost.PostMetaFile = metaFile;
                    temporaryPost.PostContentFile = contentFile;
                    temporaryPost.PostName = articleDirectory.Name;
                    temporaryPost.PostContent = File.ReadAllText(contentFile.FullName);
                    temporaryPost.PostRoot = articleDirectory;

                    temporaryPost.PostResourceDirectory = null;
                    foreach (var directory in articleDirectory.GetDirectories())
                    {
                        if (directory.Name.Equals("resources"))
                        {
                            temporaryPost.PostResourceDirectory = new DirectoryInfo(directory.FullName);
                        }
                    }   
                    Console.Write($"Successfully Loaded Article: {articleDirectory.Name}\n");
                    Posts.Add(temporaryPost);
                }
                else
                {
                    Console.Write($"Article: {articleDirectory.Name} is malformed, skipping!\n");
                }
            }
            BlogeratorStarted();
        }
    }
}