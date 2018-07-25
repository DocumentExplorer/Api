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
        private readonly IFileService _fileService;
        public FilesController(ICommandDispatcher commandDispatcher, IMemoryCache cache, 
            IOrderService orderService, IFileService fileService) : base(commandDispatcher, cache)
        {
            _orderService = orderService;
            _fileService = fileService;
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
            if(!IsAuthorizedPlusComplementer(order.Owner1Name))
            {
                return StatusCode(403);
            }
            await DispatchAsync(command);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{uploadId}")]
        public async Task<IActionResult> DeleteFile(Guid uploadId)
        {
            var file = await _fileService.GetFile(uploadId);
            var order = await _orderService.GetAsync(file.OrderId);
            if(!IsAuthorizedPlusComplementer(order.Owner1Name))
            {
                return StatusCode(403);
            }
            await _fileService.DeleteFile(uploadId);
            return NoContent();
        }

    }
}