using System;
using System.IO;
using System.Security.Cryptography;

namespace Tayko.co.Models
{
    public class PostModel
    {
        public string PostTitle { get; set; }                        // stored in meta
        public string PostDescription { get; set; }                  // stored in meta
        public DateTime PostDate { get; set; }                       // stored in meta
        public FileInfo PostCover { get; set; }
        public string PostAuthor { get; set; }
        public string PostName { get; set; }                         // obtained from directory name
        public string PostContent { get; set; }                      // stored in content
        public FileInfo PostStorageFile { get; set; }                // physical location of content file
        public DirectoryInfo PostRoot { get; set; }                  // physical location of article root
        public DirectoryInfo PostResourceDirectory { get; set; }     // physical location of post resources
        public string PostStorageFileHash { get; set; }
        public bool UpdateHash()
        {
            if (!File.Exists(PostStorageFile.FullName)) return false;
            
            using var md5 = MD5.Create();
            using var hashStream = File.OpenRead(PostStorageFile.FullName);
            
            string fileHash = BitConverter.ToString(md5.ComputeHash(hashStream)).Replace("-", "").ToLowerInvariant();

            if (fileHash == PostStorageFileHash) return false;

            PostStorageFileHash = fileHash;
            return true;
        }
    }
    
    
}