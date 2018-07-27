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
    public class SqlOrderRepository : IOrderRepository, ISqlRepository
    {
        private readonly DocumentExplorerContext _context;

        public SqlOrderRepository(DocumentExplorerContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Order order)
        {
            await _context.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
            => await _context.Orders.ToListAsync();

        public async Task<IEnumerable<Order>> GetAllByUser(string username)
            => await _context.Orders.Where(x => x.Owner1Name==username).ToListAsync();

        public async Task<Order> GetAsync(Guid id)
            => await _context.Orders.SingleOrDefaultAsync(x=> x.Id == id);

        public async Task RemoveAsync(Order order)
        {
            _context.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}