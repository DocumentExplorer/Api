using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Repositories
{
    class MongoOrderRepository : IOrderRepository, IMongoRepository
    {
        private readonly IMongoDatabase _database;

        public MongoOrderRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task AddAsync(Order order)
            => await Orders.InsertOneAsync(order);

        public async Task<IEnumerable<Order>> GetAllAsync()
            => await Orders.Find(_ => true).ToListAsync();

        public async Task<Order> GetAsync(Guid id)
            => await Orders.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task RemoveAsync(Order order)
            => await Orders.DeleteOneAsync(x => x.Id == order.Id);

        public async Task UpdateAsync(Order order)
            => await Orders.ReplaceOneAsync(x => x.Id == order.Id, order);

        public async Task<IEnumerable<Order>> GetAllByUser(string username)
            => await Orders.Find(x => x.Owner1Name == username).ToListAsync();

        private IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
    }
}
