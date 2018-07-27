using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.DTO;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Extensions;

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
                throw new ServiceException(Exceptions.ErrorCodes.InvalidCredentials);
            }
            var salt = user.Salt;
            var hash = _encrypter.GetHash(password,salt);
            if(user.Password!=hash)
            {
                throw new ServiceException(Exceptions.ErrorCodes.InvalidCredentials);
            }
            return;
        }

        public async Task RegisterAsync(string username, string password, string role)
        {
            var user = await _userRepository.GetAsync(username);
            if (user != null)
            {
                throw new ServiceException(Exceptions.ErrorCodes.UsernameInUse);
            }
            var salt = _encrypter.GetSalt(password);
            var hash = _encrypter.GetHash(password,salt);
            user = new User(username, hash, salt, role);
            await _userRepository.AddAsync(user);
            
        }

        public async Task<UserDto> GetAsync(string username)
        {
            var user = await _userRepository.GetOrFailAsync(username);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetAsync(Guid id)
        {
            var user = await _userRepository.GetOrFailAsync(id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await _userRepository.GetOrFailAsync(id);
            await _userRepository.RemoveAsync(user);
        }

        public async Task ChangePassword(Guid id, string password)
        {
            var user = await _userRepository.GetOrFailAsync(id);
            var salt = _encrypter.GetSalt(password);
            var hash = _encrypter.GetHash(password,salt);
            user.SetPassword(hash,salt);
            await _userRepository.UpdateAsync(user);
        }
    }
}