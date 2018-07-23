using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.BlobStorage;
using DocumentExplorer.Infrastructure.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Repositories
{
    public class BlobStorageOrderRepository  : IRealFileRepository, IBlobStorageRepository
    {
        private readonly BlobStorageContext _blobStorageContext;
        private readonly IOrderFolderNameGenerator _orderFolderNameGenerator;

        public BlobStorageOrderRepository(BlobStorageContext blobStorageContext, IOrderFolderNameGenerator orderFolderNameGenerator)
        {
            _blobStorageContext = blobStorageContext;
            _orderFolderNameGenerator = orderFolderNameGenerator;
        }


        public async Task AddAsync(string source, string destination)
            => await _blobStorageContext.UploadAsync(destination, source);

        public async Task GetAsync(string source, string destination)
            => await _blobStorageContext.DownloadAsync(source,destination);

        public async Task RemoveAsync(string path)
            => await _blobStorageContext.DeleteAsync(path);
    }
}
