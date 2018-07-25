using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using MongoDB.Driver;

namespace DocumentExplorer.Infrastructure.Repositories
{
    public class MongoPermissionsRepository : IMongoRepository, IPermissionsRepository
    {
        private readonly IMongoDatabase _database;

        public MongoPermissionsRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task AddAsync(Permissions permissions)
            => await Permissions.InsertOneAsync(permissions);

        public async Task<Permissions> GetAsync()
            => await Permissions.Find(_ => true).SingleOrDefaultAsync();

        public async Task UpdateAsync(Permissions permissions)
            => await Permissions.ReplaceOneAsync(x=> x.Id == permissions.Id, permissions);

        private IMongoCollection<Permissions> Permissions => _database.GetCollection<Permissions>("Permissions");
    }
}