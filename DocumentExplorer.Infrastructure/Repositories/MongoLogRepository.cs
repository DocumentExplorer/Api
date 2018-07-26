using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using MongoDB.Driver;

namespace DocumentExplorer.Infrastructure.Repositories
{
    public class MongoLogRepository : IMongoRepository, ILogRepository
    {
        private readonly IMongoDatabase _database;

        public MongoLogRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task AddAsync(Log log)
            => await Logs.InsertOneAsync(log);

        public async Task<IEnumerable<Log>> GetAllAsync()
            => await Logs.Find(_ => true).ToListAsync();

        public async Task<Log> GetAsync(Guid id)
            => await Logs.Find(x => x.Id==id).FirstOrDefaultAsync();

        public async Task RemoveAsync(Log log)
            => await Logs.DeleteOneAsync(x=> x.Id == log.Id);

        private IMongoCollection<Log> Logs => _database.GetCollection<Log>("Logs");
    }
}