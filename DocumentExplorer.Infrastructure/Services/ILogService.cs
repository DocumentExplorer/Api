using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Infrastructure.DTO;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface ILogService
    {
        Task<IEnumerable<LogDto>> GetLogsAsync();
        Task<LogDto> GetLogAsync(Guid id);
        Task AddLogAsync(string @event, Order order, string username);
        Task DeleteLogAsync(Guid id);
    }
}