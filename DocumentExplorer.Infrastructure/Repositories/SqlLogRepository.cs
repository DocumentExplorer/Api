using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace DocumentExplorer.Infrastructure.Repositories
{
    public class SqlLogRepository : ISqlRepository, ILogRepository
    {

        private readonly DocumentExplorerContext _context;

        public SqlLogRepository(DocumentExplorerContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Log log)
        {
            await _context.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Log>> GetAllAsync()
            => await _context.Logs.OrderBy(x => x.Date).ToListAsync();

        public async Task<Log> GetAsync(Guid id)
            => await _context.Logs.SingleOrDefaultAsync(x => x.Id == id);

        public async Task RemoveAsync(Log log)
        {
            _context.Remove(log);
            await _context.SaveChangesAsync();
        }
    }
}