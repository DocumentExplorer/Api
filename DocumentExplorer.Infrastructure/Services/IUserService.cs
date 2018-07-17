using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.DTO;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IUserService
    {
        Task RegisterAsync(string username, string password, string role);
        Task LoginAsync(string username, string password);
        Task<UserDto> GetAsync(string username);
        Task<UserDto> GetAsync(Guid id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task DeleteUser(Guid id);
        Task ChangePassword(Guid id, string password);
    }
}
