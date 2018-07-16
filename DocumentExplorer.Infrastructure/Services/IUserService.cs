using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IUserService
    {
        Task RegisterAsync(string username, string password, string role);
    }
}
