using System;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Tayko.co.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class PostModel
    {
        [JsonProperty]
        public string PostTitle { get; set; }                        // stored in meta file
        [JsonProperty]
        public DateTime PostDate { get; set; }                       // stored in meta file
        [JsonProperty]
        public DateTime PostEditDate { get; set; }                   // stored in meta file
        
        public string PostName { get; set; }                         // obtained from directory name
        public string PostContent { get; set; }                      // stored in content file
        public FileInfo PostContentFile { get; set; }                // physical location of content file
        public FileInfo PostMetaFile { get; set; }                   // physical location of meta file
        public DirectoryInfo PostRoot { get; set; }                  // physical location of article root
        public DirectoryInfo PostResourceDirectory { get; set; }     // physical location of post resources
        
        public string PostCacheContent { get; set; }                 // Stores the generated content from the content file

        public string GetContentMd5Hash()
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(PostContentFile.FullName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
        }
    }
}