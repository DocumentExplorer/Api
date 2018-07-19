using System;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IHandlerTaskRunner
    {
        IHandlerTask Run(Func<Task> run);
    }
}