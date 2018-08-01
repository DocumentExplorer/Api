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
    public class SqlFileRepository : ISqlRepository, IFileRepository
    {
        private readonly DocumentExplorerContext _context;

        public SqlFileRepository(DocumentExplorerContext context)
        {
            _context = context;
        }

        public async Task AddAsync(File file)
        {
            await _context.AddAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<File>> GetAllAsync()
            => await _context.Files.ToListAsync();

        public async Task<File> GetAsync(Guid id)
            => await _context.Files.SingleOrDefaultAsync(x => x.Id == id);

        public async Task<List<File>> GetFilesContainingPath(string path)
            => await _context.Files.Where(x=> x.Path == path).ToListAsync();

        public async Task RemoveAsync(File file)
        {
            _context.Remove(file);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(File file)
        {
            _context.Update(file);
            await _context.SaveChangesAsync();
        }
    }
}