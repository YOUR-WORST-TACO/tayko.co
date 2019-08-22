using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Tayko.co.Models
{
    public class Blogerator
    {
        private IFileInfo RootDirectory { get; set; }

        public Blogerator(IHostingEnvironment hostingEnvironment)
        {
            RootDirectory = hostingEnvironment.ContentRootFileProvider.GetFileInfo("/Blog");

            /*foreach (var item in DirectoryContents)
            {
                if (item.IsDirectory)
                {
                    Console.WriteLine($"Directory: {item.Name}");
                    foreach (var item2 in hostingEnvironment.ContentRootFileProvider.GetDirectoryContents(
                        $"/Blog/${item.Name}"))
                    {
                        Console.WriteLine($"\t File: {item2.Name}");
                        if (!item2.IsDirectory)
                        {
                            
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"File: {item.Name}");
                }
            }*/
        }
    }
}