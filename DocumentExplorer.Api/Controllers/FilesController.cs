using System;
using System.IO;
using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Files;
using DocumentExplorer.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Api.Controllers
{
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public FilesController(ICommandDispatcher commandDispatcher, IMemoryCache cache, IOrderService orderService) : base(commandDispatcher, cache)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> PostAsync(IFormFile file)
        {
            
            var command = new UploadFile();
            command.File = file;
            command.Id = Guid.NewGuid();
            await DispatchAsync(command);
            return Created($"files/{command.Id}", null);
        }

        [Authorize]
        [HttpPut("{uploadId}")]
        public async Task<IActionResult> PutIntoLocationAsync([FromBody]PutIntoLocation command, Guid uploadId)
        {
            command.UploadId = uploadId;
            var order = await _orderService.GetAsync(command.OrderId);
            if((!IsAuthorized(order.Owner1Name)) && (!IsAuthorized(order.Owner2Name)))
            {
                return StatusCode(403);
            }
            await DispatchAsync(command);
            return NoContent();
        }

    }
}