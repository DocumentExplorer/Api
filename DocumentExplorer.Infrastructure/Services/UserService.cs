using System;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;

namespace DocumentExplorer.Infrastructure.Services
{
    public class UserService : IUserService
    {
        public readonly IUserRepository _userRepository;
        public readonly IEncrypter _encrypter;

        public UserService(IUserRepository userRepository, IEncrypter encrypter)
        {
            _userRepository = userRepository;
            _encrypter = encrypter;
        }
        public async Task RegisterAsync(string username, string password, string role)
        {
            var user = await _userRepository.GetAsync(username);
            if(role != Roles.User && Roles.Admin != role)
            {
                throw new Exception($"Role {role} does not exist.");
            }
            if (user != null)
            {
                throw new Exception($"User with username: {username} already exist.");
            }
            var salt = _encrypter.GetSalt(password);
            var hash = _encrypter.GetHash(password,salt);
            user = new User(username, hash, salt, role);
            await _userRepository.AddAsync(user);
        }
    }
}