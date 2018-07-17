using DocumentExplorer.Infrastructure.DTO;
using System;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IJwtHandler
    {
        JwtDto CreateToken(string email, string role);
    }
}
