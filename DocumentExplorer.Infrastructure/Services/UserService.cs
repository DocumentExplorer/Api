using System;
using System.Collections.Generic;
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

        public async Task<UserDto> GetAsync(Guid id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                throw new Exception($"User with id'{id}' does not exist.");
            }
            return _mapper.Map<User,UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<User>,List<UserDto>>(users);
        }
    }
}