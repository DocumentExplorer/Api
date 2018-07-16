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
        private ISet<User> _users = new HashSet<User>
        {
            new User("usr1","7WOcXOeklv1dsBYVKVwQtKdY7Uyd0EmKgUE86jW5/Y9pNuv87+8QiA==",//secret123
            "oW2Mj3Mg2U6JQqYXXpKPc0L5s19Qrz+9PNB9VeVzIs/xvk10osHESw==","user"),
            new User("usr2","7WOcXOeklv1dsBYVKVwQtKdY7Uyd0EmKgUE86jW5/Y9pNuv87+8QiA==",
            "oW2Mj3Mg2U6JQqYXXpKPc0L5s19Qrz+9PNB9VeVzIs/xvk10osHESw==","user"),
            new User("usr3","7WOcXOeklv1dsBYVKVwQtKdY7Uyd0EmKgUE86jW5/Y9pNuv87+8QiA==",
            "oW2Mj3Mg2U6JQqYXXpKPc0L5s19Qrz+9PNB9VeVzIs/xvk10osHESw==","user"),
            new User("adm1","7WOcXOeklv1dsBYVKVwQtKdY7Uyd0EmKgUE86jW5/Y9pNuv87+8QiA==",
            "oW2Mj3Mg2U6JQqYXXpKPc0L5s19Qrz+9PNB9VeVzIs/xvk10osHESw==","admin"),
            new User("adm2","7WOcXOeklv1dsBYVKVwQtKdY7Uyd0EmKgUE86jW5/Y9pNuv87+8QiA==",
            "oW2Mj3Mg2U6JQqYXXpKPc0L5s19Qrz+9PNB9VeVzIs/xvk10osHESw==","admin"),
            new User("adm3","7WOcXOeklv1dsBYVKVwQtKdY7Uyd0EmKgUE86jW5/Y9pNuv87+8QiA==",
            "oW2Mj3Mg2U6JQqYXXpKPc0L5s19Qrz+9PNB9VeVzIs/xvk10osHESw==","admin")
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