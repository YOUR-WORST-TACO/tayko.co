using System;
using System.IO;

namespace Tayko.co.Models
{
    public class PostModel
    {
        public string PostName { get; set; }                         // obtained from directory name
        public string PostTitle { get; set; }                        // stored in meta.conf
        public string PostContent { get; set; }                      // stored in meta.conf
        public DateTime PostDate { get; set; }                       // stored in meta.conf
        public DateTime PostEditDate { get; set; }                   // stored in meta.conf
        public FileInfo PostContentFile { get; set; }                // physical location of content file
        public FileInfo PostMetaFile { get; set; }                   // physical location of meta file
        public DirectoryInfo PostRoot { get; set; }                  // physical location of article root
        public DirectoryInfo PostResourceDirectory { get; set; }     // physical location of post resources
    }
}