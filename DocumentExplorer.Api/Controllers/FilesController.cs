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
            var result = new
            {
                Location = $"files/{command.Id}"
            };
            return Json(result);
        }

        [Authorize]
        [HttpPut("{uploadId}")]
        public async Task<IActionResult> PutIntoLocationAsync([FromBody]PutIntoLocation command, Guid uploadId)
        {
            command.UploadId = uploadId;
            await DispatchAsync(command);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{orderId}/{fileType}")]
        public async Task<IActionResult> DeleteFile(Guid orderId, string fileType)
        {
            await _fileService.DeleteFileAsync(orderId, fileType, Role, Username);
            return NoContent();
        }

        [Authorize]
        [HttpGet("{id}/{fileType}")]
        public async Task<IActionResult> GetFileAsync(Guid id, string fileType)
        {
            var command = new GetFile
            {
                OrderId = id,
                FileType = fileType,
                CacheId = Guid.NewGuid()
            };
            await DispatchAsync(command);
            var result = Cache.Get(command.CacheId);
            if(result is MemoryStream fileStream)
                return new FileContentResult(fileStream.ToArray(),"application/pdf");
            throw new InvalidCastException();
        }


        [Authorize("admin")]
        [HttpGet("generate")]
        public async Task<IActionResult> GenerateAsync()
        {
            await _fileService.GenerateAsync();
            return NoContent();
        } 

    }
}