using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.BlobStorage;
using DocumentExplorer.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Repositories
{
    public class BlobStorageOrderRepository : IOrderRepository, IBlobStorageRepository
    {
        private readonly BlobStorageContext _blobStorageContext;
        private readonly IOrderFolderNameGenerator _orderFolderNameGenerator;

        public BlobStorageOrderRepository(BlobStorageContext blobStorageContext, IOrderFolderNameGenerator orderFolderNameGenerator)
        {
            _blobStorageContext = blobStorageContext;
            _orderFolderNameGenerator = orderFolderNameGenerator;
        }

        public async Task AddAsync(Order order, string path)
            => await _blobStorageContext.UploadAsync(_orderFolderNameGenerator.OrderToName(order), $"Uploads/{path}");

        public Task<IEnumerable<Order>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Order order)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
