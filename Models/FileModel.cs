using Microsoft.Extensions.FileProviders;

namespace Tayko.co.Models
{
    public class FileModel
    {
        private readonly IFileProvider _fileProvider;
        private readonly string _path;
        
        public IDirectoryContents DirectoryContents { get; private set; }

        public FileModel(IFileProvider fileProvider, string path)
        {
            _fileProvider = fileProvider;
            _path = path;
            DirectoryContents = _fileProvider.GetDirectoryContents(_path);

            foreach (var item in DirectoryContents)
            {
                
            }
        }
        
    }
}