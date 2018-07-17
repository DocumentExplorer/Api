using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Commands
{
    public interface ICommandHandler<T> where T : ICommand
    {
        Task HandleAsync(T command);
    }
}