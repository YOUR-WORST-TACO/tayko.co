using System;
using System.IO;
using System.Security.Cryptography;

namespace Tayko.co.Models
{
    public class PostModel
    {
        public string PostTitle { get; set; }                        // stored in meta file
        public DateTime PostDate { get; set; }                       // stored in meta file
        public DateTime PostEditDate { get; set; }                   // stored in meta file
        public string PostAuthor { get; set; }
        public string PostName { get; set; }                         // obtained from directory name
        public string PostContent { get; set; }                      // stored in content file
        public FileInfo PostStorageFile { get; set; }                // physical location of content file
        public DirectoryInfo PostRoot { get; set; }                  // physical location of article root
        public DirectoryInfo PostResourceDirectory { get; set; }     // physical location of post resources
        
    }
}