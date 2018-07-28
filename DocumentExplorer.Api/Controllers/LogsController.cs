using System;
using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Logs;
using DocumentExplorer.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Api.Controllers
{
    [Route("[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
        public LogsController(ICommandDispatcher commandDispatcher, IMemoryCache cache, ILogService logService) : base(commandDispatcher, cache)
        {
            _logService = logService;
        }

        [Authorize("admin")]
        [HttpPost]
        public async Task<IActionResult> GetLogsAsync([FromBody]SearchLogs command)
        {
            command.CacheId = Guid.NewGuid();
            await DispatchAsync(command);
            var result = Cache.Get(command.CacheId);
            return Json(result);
        }

        [Authorize("admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLogAsync(Guid id)
        {
            var logs = await _logService.GetLogAsync(id);
            return Json(logs);
        }

        [Authorize("admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(Guid id)
        {
            await _logService.DeleteLogAsync(id);
            return NoContent();
        }

    }
}