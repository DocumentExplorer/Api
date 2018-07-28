using System;
using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Logs;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Infrastructure.Handlers.Logs
{
    public class SearchLogsHandler : ICommandHandler<SearchLogs>
    {
        private readonly IHandler _handler;
        private readonly ILogService _logService;
        private readonly IMemoryCache _cache;
        public SearchLogsHandler(IHandler handler, ILogService logService, IMemoryCache cache)
        {
            _cache = cache;
            _logService = logService;
            _handler = handler;
        }
        public async Task HandleAsync(SearchLogs command)
            => await _handler
            .Run(async ()=>
            {
                var logs = await _logService.GetLogsAsync(command.Event, 
                    command.Number, command.ClientCountry,
                    command.ClientIdentificationNumber,
                    command.BrokerCountry, 
                    command.BrokerIdentificationNumber,
                    command.Owner1Name, command.Username,
                    command.InvoiceNumber);
                    _cache.Set(command.CacheId, logs, TimeSpan.FromSeconds(10));
            })
            .OnCustomError(x=> throw new ServiceException(x.Code))
            .ExecuteAsync();
    }
}