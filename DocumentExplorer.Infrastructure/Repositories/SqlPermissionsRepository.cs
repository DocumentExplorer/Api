using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace DocumentExplorer.Infrastructure.Repositories
{
    public class SqlPermissionsRepository : ISqlRepository, IPermissionsRepository
    {
        private readonly DocumentExplorerContext _context;

        public SqlPermissionsRepository(DocumentExplorerContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Permissions permissions)
        {
            await _context.AddAsync(permissions);
            await _context.SaveChangesAsync();
        }

        public async Task<Permissions> GetAsync()
            => await _context.Permissions.FirstOrDefaultAsync();

        public async Task UpdateAsync(Permissions permissions)
        {
            _context.Update(permissions);
            await _context.SaveChangesAsync();
        }
    }
}