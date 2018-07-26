using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.FileSystem
{
    public class FileSystemContext
    {
        private readonly FileSystemSettings _fileSystemSettings;
        public FileSystemContext(FileSystemSettings fileSystemSettings)
        {
            _fileSystemSettings = fileSystemSettings;
        }

        public async Task AddAsync(string source, string destination)
        {
            destination = ConnectWithRootPath(destination);
            string destinationDirectory = Path.GetDirectoryName(destination);
            if(!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            File.Move(source, destination);
            await Task.CompletedTask;
        }

        public async Task<MemoryStream> GetFileStream(string path)
        {
            path = ConnectWithRootPath(path);
            Stream fileStream = File.OpenRead(path);
            var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            await Task.CompletedTask;
            return memoryStream;
        }

        public async Task DeleteFile(string path)
        {
            path = ConnectWithRootPath(path);
            File.Delete(path);
            await Task.CompletedTask;
        }

        public async Task MoveFiles(IEnumerable<string> from, IEnumerable<string> to)
        {
            using(var e1 = from.GetEnumerator())
            using(var e2 = to.GetEnumerator())
            {
                while(e1.MoveNext() && e2.MoveNext())
                {
                    await AddAsync(ConnectWithRootPath(e1.Current), e2.Current);
                }
            }
            Directory.Delete(ConnectWithRootPath(Path.GetDirectoryName(from.First())));
        }

        public async Task DeleteDirectoryIfExists(string path)
        {
            path = ConnectWithRootPath(path);
            if(Directory.Exists(path))
            {
                Directory.Delete(path);
            }
            await Task.CompletedTask;
        }

        private string ConnectWithRootPath(string path)
        {
            return $"{_fileSystemSettings.RootDirectoryPath}/{path}";
        }
    }
}