using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Users;

namespace DocumentExplorer.Infrastructure.Handlers.Users
{
    public class LoginHandler : ICommandHandler<Login>
    {
        public LoginHandler()
        {
            
        }
        public async Task HandleAsync(Login command)
        {
            throw new System.NotImplementedException();
        }
    }
}