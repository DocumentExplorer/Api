using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Services
{
    public class Handler : IHandler, IService
    {
        private readonly ISet<IHandlerTask> _handlerTasks = new HashSet<IHandlerTask>();

        public async Task ExecuteAllAsync()
        {
            foreach(var handlerTask in _handlerTasks)
            {
                await handlerTask.ExecuteAsync();
            }
            _handlerTasks.Clear();
        }

        public IHandlerTask Run(Func<Task> run)
        {
            var handlerTask = new HandlerTask(this, run);
            _handlerTasks.Add(handlerTask);

            return handlerTask;
        }

        public IHandlerTaskRunner Validate(Func<Task> validate)
            => new HandlerTaskRunner(this, validate, _handlerTasks);
    }
}
