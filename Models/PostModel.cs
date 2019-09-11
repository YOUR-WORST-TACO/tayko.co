using System;
using System.IO;
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
    }
}