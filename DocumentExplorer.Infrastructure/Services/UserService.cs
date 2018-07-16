using System;
using System.Threading.Tasks;
using AutoMapper;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.DTO;

namespace DocumentExplorer.Infrastructure.Services
{
    public class UserService : IUserService
    {
        public readonly IUserRepository _userRepository;
        public readonly IEncrypter _encrypter;
        public readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IEncrypter encrypter, IMapper mapper)
        {
            _userRepository = userRepository;
            _encrypter = encrypter;
            _mapper = mapper;
        }

        public async Task LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetAsync(username);
            if(user == null)
            {
                throw new Exception("Invalid credentials.");
            }
            var salt = user.Salt;
            var hash = _encrypter.GetHash(password,salt);
            if(user.Password!=hash)
            {
                throw new Exception("Invalid credentials.");
            }
            return;
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

        public async Task<UserDto> GetAsync(string username)
        {
            var user = await _userRepository.GetAsync(username);
            if (user == null)
            {
                throw new Exception($"User '{username}' does not exist.");
            }
            return _mapper.Map<User,UserDto>(user);
        }
    }
}