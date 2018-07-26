using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;

namespace DocumentExplorer.Core.Repositories
{
    public interface ILogRepository
    {
        Task<Log> GetAsync(Guid id);
        Task<IEnumerable<Log>> GetAllAsync();
        Task AddAsync(Log log);
        Task RemoveAsync(Log log);
    }
}