using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace DocumentExplorer.Infrastructure.Repositories
{
    public class SqlUserRepository : IUserRepository, ISqlRepository
    {
        private readonly DocumentExplorerContext _context;

        public SqlUserRepository(DocumentExplorerContext context)
        {
            _context = context;
        }
        public async Task AddAsync(User user)
        {
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
            => await _context.Users.ToListAsync();

        public async Task<User> GetAsync(string username)
            => await _context.Users.SingleOrDefaultAsync(x => x.Username == username);

        public async Task<User> GetAsync(Guid id)
            => await _context.Users.SingleOrDefaultAsync(x => x.Id == id);

        public async Task RemoveAsync(User user)
        {
            _context.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}