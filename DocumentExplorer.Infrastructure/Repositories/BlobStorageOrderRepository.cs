using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.BlobStorage;
using DocumentExplorer.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<Order>> GetAllAsync()
            => _orderFolderNameGenerator.ListOfOrders(await _blobStorageContext.ListFoldersAsync());

        public async Task<Order> GetAsync(int id)
            => _orderFolderNameGenerator.ListOfOrders(await _blobStorageContext.ListFoldersAsync()).SingleOrDefault(x=> x.Id == id);

        public async Task RemoveAsync(Order order)
            => await _blobStorageContext.DeleteAsync(_orderFolderNameGenerator.OrderToName(order));
            

        public async Task UpdateAsync(Order order, string path)
        {
            var orderToDelete = await GetAsync(order.Id);
            await RemoveAsync(orderToDelete);
            await AddAsync(order, $"Uploads/{path}");
        }
    }
}
