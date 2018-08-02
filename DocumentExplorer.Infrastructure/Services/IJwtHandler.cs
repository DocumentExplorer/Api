using DocumentExplorer.Infrastructure.DTO;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IJwtHandler
    {
        JwtDto CreateToken(string email, string role);
    }
}
