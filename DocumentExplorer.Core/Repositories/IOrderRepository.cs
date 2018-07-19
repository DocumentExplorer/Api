using DocumentExplorer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentExplorer.Core.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetAsync(int Id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task AddAsync(Order order);
        Task RemoveAsync(Order order);
        Task UpdateAsync(Order order);
    }
}
