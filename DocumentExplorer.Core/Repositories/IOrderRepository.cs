﻿using DocumentExplorer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentExplorer.Core.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetAllByUser(string username);
        Task AddAsync(Order order);
        Task RemoveAsync(Order order);
        Task UpdateAsync(Order order);
    }
}
