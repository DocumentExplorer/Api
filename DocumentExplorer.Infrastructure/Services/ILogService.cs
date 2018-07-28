using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Infrastructure.DTO;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface ILogService
    {
        Task<IEnumerable<LogDto>> GetLogsAsync(string @event,
            int number, string clientCountry, 
            string clientIdentificationNumber, string brokerCountry,
            string brokerIdentificationNumber, string Owner1Name,
            string username, int invoiceNumber);
        Task<LogDto> GetLogAsync(Guid id);
        Task AddLogAsync(string @event, Order order, 
            string username, DateTime dateTime=default(DateTime));
        Task DeleteLogAsync(Guid id);
    }
}