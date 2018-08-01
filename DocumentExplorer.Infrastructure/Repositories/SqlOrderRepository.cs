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
            await _context.AddRangeAsync(order.Files);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var orders = await _context.Orders.OrderBy(x=> x.CreationDate).ToListAsync();
            foreach(var order in orders)
            {
                var files = await _context.Files.Where(x=> x.OrderId==order.Id).ToListAsync();
                order.Files = files;
            }
            return orders;
        }

        public async Task<IEnumerable<Order>> GetAllByUser(string username)
        {
            var orders = await _context.Orders.Where(x => x.Owner1Name==username).OrderBy(x=> x.CreationDate).ToListAsync();
            foreach(var order in orders)
            {
                var files = await _context.Files.Where(x=> x.OrderId==order.Id).ToListAsync();
                order.Files = files;
            }
            return orders;
        }

        public async Task<Order> GetAsync(Guid id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(x=> x.Id == id);
            if(order != null)
                order.Files = await _context.Files.Where(x=> x.OrderId == id).ToListAsync();
            return order;
        }

        public async Task RemoveAsync(Order order)
        {
            _context.Remove(order);
            _context.RemoveRange(order.Files);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Update(order);
            _context.UpdateRange(order.Files);
            await _context.SaveChangesAsync();
        }
    }
}