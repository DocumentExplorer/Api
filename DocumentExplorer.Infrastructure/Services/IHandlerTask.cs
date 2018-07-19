using DocumentExplorer.Core.Domain;
using System;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IHandlerTask
    {
        IHandlerTask Always(Func<Task> always);
        IHandlerTask OnCustomErrorAsync(Func<DocumentExplorerException, Task> onCustomError, bool propagatedException = false, bool executeOnError = false);
        IHandlerTask OnCustomError(Action<DocumentExplorerException> onCustomError, bool propagatedException = false, bool executeOnError = false);
        IHandlerTask OnError(Func<Exception, Task> onError, bool propagatedException = false, bool executeOnError = false);

        IHandlerTask OnSuccess(Func<Task> onSuccess);

        IHandlerTask PropagateException();
        IHandlerTask DoNotPropagateException();
        IHandler Next();
        Task ExecuteAsync();
    }
}