using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Repositories
{
    public class MongoUserRepository : IUserRepository, IMongoRepository
    {
        private readonly IMongoDatabase _database;

        public MongoUserRepository(IMongoDatabase database)
        {
            _database = database;
        }
        public async Task AddAsync(User user)
            => await Users.InsertOneAsync(user);

        public async Task<IEnumerable<User>> GetAllAsync()
            => await Users.Find(_ => true).ToListAsync();

        public async Task<User> GetAsync(string username)
            => await Users.Find(x => x.Username == username).FirstOrDefaultAsync();

        public async Task<User> GetAsync(Guid id)
            => await Users.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task RemoveAsync(User user)
            => await Users.DeleteOneAsync(x => x.Id == user.Id);

        public async Task UpdateAsync(User user)
            => await Users.ReplaceOneAsync(x => x.Id == user.Id, user);

        private IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    }
}
