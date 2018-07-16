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
    }
}
