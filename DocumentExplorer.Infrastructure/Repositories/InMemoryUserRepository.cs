using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;

namespace DocumentExplorer.Infrastructure.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private static ISet<User> _users = new HashSet<User>
        {
            new User("usr1","secret123",Guid.NewGuid().ToString("N"),Roles.User),
            new User("usr2","secret123",Guid.NewGuid().ToString("N"),Roles.User),
            new User("usr3","secret123",Guid.NewGuid().ToString("N"),Roles.User),
            new User("adm1","secret123",Guid.NewGuid().ToString("N"),Roles.Admin),
            new User("adm2","secret123",Guid.NewGuid().ToString("N"),Roles.Admin),
            new User("adm3","secret123",Guid.NewGuid().ToString("N"),Roles.Admin)
        };
        public async Task AddAsync(User user)
        {
            _users.Add(user);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
            => await Task.FromResult(_users);

        public async Task<User> GetAsync(string username)
            => await Task.FromResult(_users.SingleOrDefault(x => x.Username == username));

        public async Task<User> GetAsync(Guid id)
            => await Task.FromResult(_users.SingleOrDefault(x => x.Id == id));

        public async Task RemoveAsync(User user)
        {
            _users.Remove(user);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(User user)
        {
            await RemoveAsync(user);
            await AddAsync(user);
            await Task.CompletedTask;
        }
    }
}