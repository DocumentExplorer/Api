using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;

namespace DocumentExplorer.Core.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetAsync(string username);
        Task<User> GetAsync(Guid id);
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        Task RemoveAsync(User user);
        Task UpdateAsync(User user);
    }
}