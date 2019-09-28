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
        public string PostMeta { get; set; }                         // stored in content file
        public FileInfo PostStorageFile { get; set; }                // physical location of content file
        public DirectoryInfo PostRoot { get; set; }                  // physical location of article root
        public DirectoryInfo PostResourceDirectory { get; set; }     // physical location of post resources
        
        public string PostCacheContent { get; set; }                 // Stores the generated content from the content file

        public string GetContentMd5Hash()
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(PostStorageFile.FullName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
        }
        
        public void LoadCache(DirectoryInfo rootDirectory)
        {
            var currentPostHash = GetContentMd5Hash();
            
            var postCacheFile = Path.Combine(rootDirectory.FullName, ".cache", PostName + ".cache");
            var postHashFile = Path.Combine(rootDirectory.FullName, ".cache", PostName);

            if (File.Exists(postHashFile) && File.Exists(postCacheFile))
            {
                var previousPostHash = File.ReadAllText(postHashFile);
                if (previousPostHash != currentPostHash)
                {
                    File.WriteAllText(postHashFile, currentPostHash);
                    Console.WriteLine("Rebuilding Content Cache");
                    // Parse Content into Cache
                }
            }
            else
            {
                File.WriteAllText(postHashFile, currentPostHash);
                File.Create(postCacheFile);
                // Parse Content into Cache
            }
        }
    }
}