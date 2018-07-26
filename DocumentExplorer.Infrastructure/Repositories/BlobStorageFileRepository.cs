using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.BlobStorage;
using DocumentExplorer.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Repositories
{
    public class BlobStorageOrderRepository  : IRealFileRepository, IBlobStorageRepository
    {
        private readonly BlobStorageContext _blobStorageContext;

        public BlobStorageOrderRepository(BlobStorageContext blobStorageContext)
        {
            _blobStorageContext = blobStorageContext;
        }


        public async Task AddAsync(string source, string destination)
            => await _blobStorageContext.UploadAsync(destination, source);

        public async Task<MemoryStream> GetAsync(string source)
            => await _blobStorageContext.DownloadAsync(source);

        public async Task<IEnumerable<string>> GetDirectoriesAsync(string path)
        {
            await Task.CompletedTask;
            return new List<string>();
        }

        public async Task<DateTime> GetDirectoryCreationDateAsync(string path)
        {
            await Task.CompletedTask;
            return DateTime.UtcNow;
        }

        public async Task<IEnumerable<string>> GetFilesPathsAsync(string path)
        {
            await Task.CompletedTask;
            return new List<string>();
        }

        public async Task RemoveAsync(string path)
            => await _blobStorageContext.DeleteAsync(path);

        public async Task RemoveDirectoryIfExists(string path)
            => await Task.CompletedTask;

        public async Task UpdateFileNames(IEnumerable<string> from, IEnumerable<string> to)
            => await _blobStorageContext.UpdateFileNames(from, to);
    }
}
