using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.DTO;
using DocumentExplorer.Infrastructure.Extensions;

namespace DocumentExplorer.Infrastructure.Services
{
    public class LogService : ILogService, IService
    {
        private readonly ILogRepository _logRepository;
        private readonly IMapper _mapper;

        public LogService(ILogRepository logRepository, IMapper mapper)
        {
            _logRepository = logRepository;
            _mapper = mapper;
        }
        public async Task AddLogAsync(string @event, Order order, string username, DateTime date=default(DateTime))
        {
            var log = new Log(@event, order, username, date);
            await _logRepository.AddAsync(log);
        }

        public async Task DeleteLogAsync(Guid id)
        {
            var log = await _logRepository.GetOrFailAsync(id);
            await _logRepository.RemoveAsync(log);
        }

        public async Task<LogDto> GetLogAsync(Guid id)
        {
            var log = await _logRepository.GetOrFailAsync(id);
            return _mapper.Map<LogDto>(log);
        }

        public async Task<IEnumerable<LogDto>> GetLogsAsync(string @event, int number, string clientCountry, 
            string clientIdentificationNumber, string brokerCountry, string brokerIdentificationNumber, 
            string Owner1Name, string username, int invoiceNumber)
        {
            var querry = await _logRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(@event)) querry = querry.Where(x => x.Event.Contains(@event));
            if (number!=0) querry = querry.Where(x=> x.Number.ToString().Contains(number.ToString()));
            if (!string.IsNullOrEmpty(clientCountry)) querry = querry.Where(x => x.ClientCountry.Contains(clientCountry));
            if (!string.IsNullOrEmpty(clientIdentificationNumber)) querry = querry.Where(x => x.ClientIdentificationNumber.Contains(clientIdentificationNumber));
            if (!string.IsNullOrEmpty(brokerCountry)) querry = querry.Where(x => x.BrokerCountry.Contains(brokerCountry));
            if (!string.IsNullOrEmpty(brokerIdentificationNumber)) querry = querry.Where(x => x.BrokerIdentificationNumber.Contains(brokerIdentificationNumber));
            if (!string.IsNullOrEmpty(Owner1Name)) querry = querry.Where(x => x.Owner1Name.Contains(Owner1Name));
            if (!string.IsNullOrEmpty(username)) querry = querry.Where(x => x.Username.Contains(username));
            if (invoiceNumber!=0) querry = querry.Where(x => x.InvoiceNumber.ToString().Contains(invoiceNumber.ToString()));
            return _mapper.Map<IEnumerable<LogDto>>(querry);
        }
    }
}