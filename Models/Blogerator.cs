using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Tayko.co.Models
{
    public class Blogerator
    {
        private DirectoryInfo RootDirectory { get; set; }
        private string ContentFileName = "content.html";
        private string MetaConfigFileName = "meta.conf";

        public Blogerator(IHostingEnvironment hostingEnvironment)
        {
            RootDirectory = new DirectoryInfo(hostingEnvironment.ContentRootPath + "/Blog");
            
            BlogInitializer();
            
            BlogeratorStarted();
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
                Console.WriteLine($"{subDirectory.Name}");

                foreach (var file in articleDirectory.GetFiles())
                {
                    if (file.Name.Equals(ContentFileName))
                    {
                        Console.WriteLine($"\tcontent file: {file.Name}");
                    } else if (file.Name.Equals(MetaConfigFileName))
                    {
                        Console.WriteLine($"\tmeta file: {file.Name}");
                    }
                }

                foreach (var directory in articleDirectory.GetDirectories())
                {
                    if (directory.Name.Equals("resources"))
                    {
                        var resourceDirectory = new DirectoryInfo(directory.FullName);
                        Console.WriteLine($"\t{directory.Name}");

                        foreach (var resource in resourceDirectory.GetFiles())
                        {
                            Console.WriteLine($"\t\t{resource.Name}");
                        }
                    }
                }
            }
        }
    }
}