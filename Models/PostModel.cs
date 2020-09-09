using System;
using System.IO;
using System.Security.Cryptography;

namespace Tayko.co.Models
{
    public class PostModel
    {
        public string PostAuthor { get; set; }
        public string PostName { get; set; }                         // obtained from directory name
        public string PostTitle { get; set; }                        // stored in meta
        public string PostContent { get; set; }                      // stored in content
        public string PostDescription { get; set; }                  // stored in meta
        public FileInfo PostStorageFile { get; set; }                // physical location of content file
        private string PostStorageFileHash { get; set; }
        public string PostCover { get; set; }
        public DirectoryInfo PostRoot { get; set; }                  // physical location of article root
        public DateTime PostDate { get; set; }                       // stored in meta
        
        public PostModel()
        {
            PostTitle = "";
            PostDescription = "";
            PostDate = DateTime.Now;
            PostAuthor = "";
            PostName = "";
            PostContent = "";
            PostStorageFile = null;
            PostStorageFileHash = "";
            PostRoot = null;
        }
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