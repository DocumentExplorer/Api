using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using MongoDB.Driver;

namespace DocumentExplorer.Infrastructure.Repositories
{
    public class MongoFileRepository : IFileRepository, IMongoRepository
    {
        private readonly IMongoDatabase _database;

        public MongoFileRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task AddAsync(File file)
            => await Files.InsertOneAsync(file);

        public async Task<File> GetAsync(Guid id)
            => await Files.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task RemoveAsync(File file)
            => await Files.DeleteOneAsync(x => x.Id == file.Id);

        public async Task UpdateAsync(File file)
            => await Files.ReplaceOneAsync(x => x.Id == file.Id, file);

        private IMongoCollection<File> Files => _database.GetCollection<File>("File");
    }
}