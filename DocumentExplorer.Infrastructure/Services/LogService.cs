using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.DTO;
using DocumentExplorer.Infrastructure.Extensions;

namespace DocumentExplorer.Infrastructure.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;
        private readonly IMapper _mapper;

        public LogService(ILogRepository logRepository, IMapper mapper)
        {
            _logRepository = logRepository;
            _mapper = mapper;
        }
        public async Task AddLogAsync(string @event, Order order, string username)
        {
            var log = new Log(@event, order, username);
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

        public async Task<IEnumerable<LogDto>> GetLogsAsync()
        {
            var logs = await _logRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<LogDto>>(logs);
        }
    }
}