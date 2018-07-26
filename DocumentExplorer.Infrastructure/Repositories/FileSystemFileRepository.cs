using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.FileSystem;

namespace DocumentExplorer.Infrastructure.Repositories
{
    public class FileSystemFileRepository : IRealFileRepository, IFileSystemRepository
    {
        private readonly FileSystemContext _context;
        public FileSystemFileRepository(FileSystemContext context)
        {
            _context = context;
        }
        public async Task AddAsync(string source, string destination)
            => await _context.AddAsync(source, destination);

        public async Task<MemoryStream> GetAsync(string source)
            => await _context.GetFileStream(source);

        public async Task RemoveAsync(string path)
            => await _context.DeleteFile(path);

        public async Task RemoveDirectoryIfExists(string path)
            => await _context.DeleteDirectoryIfExists(path);

        public async Task UpdateFileNames(IEnumerable<string> from, IEnumerable<string> to)
            => await _context.MoveFiles(from, to);
    }
}